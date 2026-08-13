// EXPERIMENT (McProtoNet.Next) — measurement 2: what the extra copies on the send
// path cost. Requirement source: docs/design/transport-requirements-2026-08-10.md,
// "Потроха Output" — does the write door keep its own pooled buffer, or hand out the
// pipe's memory directly.
//
// Four arms, all ending in the same pipe, all writing the same frame
// (VarInt id + body):
//
//   current  — the shape the send door had until 2026-08-13: a fresh
//              MinecraftPrimitiveWriter → GetWrittenMemory (copy 1) → rented buffer to
//              prepend the VarInt id (copy 2) → frame into the pipe (copy 3). Kept as the
//              baseline the next arm is measured against; nothing takes this path now.
//   cached   — what SendAsync<T> does today: the writer is rented from the thread-local
//              cache and its WrittenMemory goes out directly, so copy 1 and the writer's
//              own allocation are gone. Copies 2 and 3 stay: prepending the id and the
//              frame into the pipe are the frame layer's business, not the writer's.
//   staging  — the proposed default: one pooled IBufferWriter is the body, and the
//              frame layer copies it into the pipe once (copy 1).
//   direct   — the lower bound: the body is written straight into the pipe, with the
//              frame length computed in advance. Zero copies. Only expressible with
//              compression off — with compression on the body must exist whole before
//              it can be compressed, so this arm is not run there.

using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using McProtoNet.Next.Wire;
using McProtoNet.Serialization;

namespace McProtoNet.Next.Measurements;

internal static class SendCost
{
    private const int PacketId = 0x12;

    public static void Run()
    {
        Console.WriteLine("== Замер 2: цена лишних копий на отправке ==");
        Console.WriteLine("(один кадр = VarInt(id) + тело; всё синхронно, в одну трубу)");
        Console.WriteLine();

        Measure("сжатие выкл, тело 16 Б", threshold: -1, bodySize: 16, iterations: 300_000);
        Measure("сжатие выкл, тело 32 КиБ", threshold: -1, bodySize: 32 * 1024, iterations: 20_000);
        Measure("сжатие вкл (порог 256), тело 32 КиБ", threshold: 256, bodySize: 32 * 1024, iterations: 3_000);
    }

    private static void Measure(string title, int threshold, int bodySize, int iterations)
    {
        Console.WriteLine(title);

        byte[] body = new byte[bodySize];
        for (int i = 0; i < body.Length; i++)
            body[i] = (byte)(i * 31 + 7);

        RunArm("прежний писатель (3 копии)", threshold, body, iterations, Current);
        RunArm("писатель из кэша (2 копии)", threshold, body, iterations, Cached);
        RunArm("промежуточный буфер (1 копия)", threshold, body, iterations, Staging);
        RunArm("WireWriter поверх буфера (1 копия)", threshold, body, iterations, WireOverStaging);
        if (threshold < 0)
            RunArm("прямо в трубу (0 копий)", threshold, body, iterations, Direct);
        else
            Console.WriteLine("  прямо в трубу        : неприменимо — сжатию нужно целое тело");

        Console.WriteLine();
    }

    private static void RunArm(
        string name, int threshold, byte[] body, int iterations, Action<Arm> arm)
    {
        using var context = new Arm(threshold);

        for (int i = 0; i < 200; i++) // прогрев: JIT, пулы, компрессор
        {
            context.Body = body;
            arm(context);
            context.Drain();
        }

        context.Body = body;
        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            arm(context);
            context.Drain();
        }
        stopwatch.Stop();
        long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        double nanoseconds = stopwatch.Elapsed.TotalNanoseconds / iterations;
        double allocPerOp = allocated / (double)iterations;
        Console.WriteLine($"  {name,-30}: {nanoseconds,9:N0} нс/кадр   {allocPerOp,8:N0} Б аллокаций/кадр");
    }

    // ------------------------------------------------------------------ arms

    private static void Current(Arm arm)
    {
        // Тело собирается своим писателем, который владеет своим буфером.
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteBuffer(arm.Body);

        using MemoryOwner<byte> owner = writer.GetWrittenMemory();     // копия 1
        ReadOnlyMemory<byte> bodyMemory = owner.Memory;

        byte[] rented = ArrayPool<byte>.Shared.Rent(5 + bodyMemory.Length);
        try
        {
            int headerLength = PacketId.GetVarIntLength(rented.AsSpan());
            bodyMemory.Span.CopyTo(rented.AsSpan(headerLength));       // копия 2
            arm.Frames.WriteFrame(rented.AsSpan(0, headerLength + bodyMemory.Length), default); // копия 3
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }

        arm.Flush();
    }

    private static void Cached(Arm arm)
    {
        // Тот же кадр, что и в прежнем плече, но писатель берётся из потокового кэша и
        // тело уходит как WrittenMemory: ни писателя, ни его буфера, ни копии 1.
        MinecraftPrimitiveWriter writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            writer.WriteBuffer(arm.Body);
            ReadOnlyMemory<byte> bodyMemory = writer.WrittenMemory;    // копии нет

            byte[] rented = ArrayPool<byte>.Shared.Rent(5 + bodyMemory.Length);
            try
            {
                int headerLength = PacketId.GetVarIntLength(rented.AsSpan());
                bodyMemory.Span.CopyTo(rented.AsSpan(headerLength));   // копия 1
                arm.Frames.WriteFrame(rented.AsSpan(0, headerLength + bodyMemory.Length), default); // копия 2
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }

            arm.Flush();
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }

    private static void Staging(Arm arm)
    {
        PooledBufferWriter staging = arm.Staging;
        staging.Reset();
        staging.WriteVarInt(PacketId);
        staging.Write(arm.Body);

        arm.Frames.WriteFrame(staging.WrittenSpan, default);           // копия 1
        arm.Flush();
    }

    private static void WireOverStaging(Arm arm)
    {
        // The task-14 shape: the writer owns no buffer, it writes THROUGH the pooled sink.
        // Same body, same one copy into the pipe — this arm exists to show the writer
        // wrapper costs nothing over the raw staging buffer.
        PooledBufferWriter staging = arm.Staging;
        staging.Reset();
        WireWriter wire = arm.Wire;
        wire.Reset(staging);
        wire.WriteVarInt(PacketId);
        wire.WriteBuffer(arm.Body);

        arm.Frames.WriteFrame(staging.WrittenSpan, default);
        arm.Flush();
    }

    private static void Direct(Arm arm)
    {
        PipeWriter output = arm.Pipe.Writer;
        int frameLength = PacketId.GetVarIntLength() + arm.Body.Length;

        output.WriteVarInt(frameLength);
        output.WriteVarInt(PacketId);
        output.Write(arm.Body);

        arm.Flush();
    }

    // ------------------------------------------------------------------ context

    private sealed class Arm : IDisposable
    {
        public Arm(int threshold)
        {
            Pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
            Frames = new FrameWriter(Pipe.Writer) { CompressionThreshold = threshold };
            Staging = new PooledBufferWriter(64 * 1024);
            Wire = new WireWriter(Staging);
        }

        public Pipe Pipe { get; }
        public FrameWriter Frames { get; }
        public PooledBufferWriter Staging { get; }
        public WireWriter Wire { get; }
        public byte[] Body { get; set; } = [];

        public void Flush()
        {
            ValueTask<FlushResult> flush = Pipe.Writer.FlushAsync();
            if (!flush.IsCompleted)
                flush.AsTask().GetAwaiter().GetResult();
            else
                _ = flush.Result;
        }

        public void Drain()
        {
            if (Pipe.Reader.TryRead(out ReadResult result))
                Pipe.Reader.AdvanceTo(result.Buffer.End);
        }

        public void Dispose()
        {
            Frames.Dispose();
            Staging.Dispose();
            Pipe.Writer.Complete();
            Pipe.Reader.Complete();
        }
    }
}
