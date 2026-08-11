// EXPERIMENT (McProtoNet.Next) — shows the WireWriter prototype on live code:
// the owner's own handshake example, written through ONE writer into TWO different sinks,
// proving the writer holds no buffer of its own.
//
//   dotnet run --project McProtoNet/experiments/NextApi -c Release -- wire

using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Next.Measurements;
using McProtoNet.Serialization; // WriteVarInt(this IBufferWriter<byte>, int)

namespace McProtoNet.Next.Wire;

internal static class WireWriterDemo
{
    // The owner's example from another project:
    //   await connection.WriteAsync(_handshake, token);
    //   await connection.Stream.WriteVarIntAsync(2097151, token);
    //   await connection.WriteAsync(EmptyArray, token);
    // Here it is one packet body, written by hand, no per-byte awaits.
    private const int ProtocolVersion = 772;
    private const int LoginIntent = 2;

    public static async Task RunAsync()
    {
        Console.WriteLine("== Прототип WireWriter: один писатель, два приёмника ==");
        Console.WriteLine();

        // Sink A: a pooled staging buffer. The writer's bytes land here; later one copy
        // goes to the pipe. This is the measured default.
        using var staging = new PooledBufferWriter(64);
        var writer = new WireWriter(staging);
        WriteHandshake(writer, "mc.example.com", 25565);
        Console.WriteLine($"в буфер из пула : {staging.WrittenSpan.Length} Б  {Hex(staging.WrittenSpan)}");

        // Sink B: the pipe directly. SAME writer instance, rebound. The bytes go straight
        // toward the wire, no staging buffer at all.
        var pipe = new Pipe();
        writer.Reset(pipe.Writer);
        WriteHandshake(writer, "mc.example.com", 25565);
        await pipe.Writer.FlushAsync().ConfigureAwait(false);
        ReadResult read = await pipe.Reader.ReadAsync().ConfigureAwait(false);
        Console.WriteLine($"прямо в трубу   : {read.Buffer.Length} Б  {Hex(read.Buffer.ToArray())}");
        pipe.Reader.AdvanceTo(read.Buffer.End);

        Console.WriteLine();
        Console.WriteLine("Тот же экземпляр писателя, тот же результат — своей памяти у него нет.");
        Console.WriteLine("Один сломанный кадр из примера владельца — просто «написал что хотел»:");
        Console.WriteLine();

        // The broken frame: declared length one byte longer than the body that follows,
        // assembled with the same writer. No special API — just bytes.
        using var broken = new PooledBufferWriter(16);
        var brokenWriter = new WireWriter(broken);
        brokenWriter.WriteVarInt(2097151);           // объявленная длина — врёт
        brokenWriter.WriteBuffer(ReadOnlySpan<byte>.Empty); // тела нет
        Console.WriteLine($"ломаный кадр    : {broken.WrittenSpan.Length} Б  {Hex(broken.WrittenSpan)}");
    }

    private static void WriteHandshake(WireWriter writer, string host, ushort port)
    {
        writer.WriteVarInt(ProtocolVersion);
        writer.WriteString(host);
        writer.WriteUnsignedShort(port);
        writer.WriteVarInt(LoginIntent);
    }

    private static string Hex(ReadOnlySpan<byte> bytes)
    {
        var chars = new char[bytes.Length * 3];
        int i = 0;
        foreach (byte b in bytes)
        {
            chars[i++] = ToHex(b >> 4);
            chars[i++] = ToHex(b & 0xF);
            chars[i++] = ' ';
        }
        return new string(chars, 0, Math.Max(0, i - 1));
    }

    private static char ToHex(int nibble) => (char)(nibble < 10 ? '0' + nibble : 'a' + nibble - 10);
}
