using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Pipelines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

public class CryptoPipeBlackBoxTests
{
    private const int MaxLength = 1024 * 1024;
    private const int MaxChunk = 70_000;

    private static byte[] MakeKey(Random rng)
    {
        var key = new byte[PacketCipher.SharedSecretLength];
        rng.NextBytes(key);
        return key;
    }

    private static IBufferedCipher Reference(byte[] key, bool forEncryption)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(key), key));
        return cipher;
    }

    private static byte[] ReferenceTransform(byte[] key, bool forEncryption, ReadOnlySpan<byte> input)
    {
        var cipher = Reference(key, forEncryption);
        var output = new byte[input.Length];
        int written = cipher.ProcessBytes(input.ToArray(), 0, input.Length, output, 0);
        written += cipher.DoFinal(output, written);
        Assert.Equal(input.Length, written);
        return output;
    }

    private static byte[] ExpectedWire(byte[] key, byte[] plain, int plainPrefix)
    {
        var wire = new byte[plain.Length];
        plain.AsSpan(0, plainPrefix).CopyTo(wire);
        ReferenceTransform(key, true, plain.AsSpan(plainPrefix)).CopyTo(wire.AsSpan(plainPrefix));
        return wire;
    }

    private static int PickLength(Random rng, int seed) => (seed % 4) switch
    {
        0 => 0,
        1 => MaxLength,
        2 => rng.Next(1, 4096),
        _ => rng.Next(0, MaxLength + 1),
    };

    private static async Task<byte[]> DrainAsync(PipeReader reader, CancellationToken ct)
    {
        var sink = new ArrayBufferWriter<byte>();
        while (true)
        {
            var result = await reader.ReadAsync(ct);
            foreach (var segment in result.Buffer)
            {
                sink.Write(segment.Span);
            }

            reader.AdvanceTo(result.Buffer.End);
            if (result.IsCompleted)
            {
                return sink.WrittenSpan.ToArray();
            }
        }
    }

    private static async Task WriteRandomlyAsync(
        CryptoPipeWriter writer, byte[] plain, int enableAt, byte[] key, Random rng, CancellationToken ct)
    {
        int offset = 0;
        bool enabled = false;
        while (offset < plain.Length || !enabled)
        {
            if (!enabled && offset >= enableAt)
            {
                await writer.FlushAsync(ct);
                writer.EnableEncryption(key);
                enabled = true;
                continue;
            }

            int limit = enabled ? plain.Length - offset : enableAt - offset;
            int chunk = Math.Min(limit, rng.Next(1, MaxChunk + 1));
            if (chunk == 0)
            {
                continue;
            }

            int step = 0;
            while (step < chunk)
            {
                bool useMemory = rng.Next(2) == 0;
                int hint = rng.Next(0, 3) == 0 ? 0 : rng.Next(1, chunk - step + 1);
                int room;
                if (useMemory)
                {
                    var memory = writer.GetMemory(hint);
                    room = Math.Min(memory.Length, chunk - step);
                    plain.AsSpan(offset + step, room).CopyTo(memory.Span);
                }
                else
                {
                    var span = writer.GetSpan(hint);
                    room = Math.Min(span.Length, chunk - step);
                    plain.AsSpan(offset + step, room).CopyTo(span);
                }

                writer.Advance(room);
                step += room;
            }

            offset += chunk;
            if (rng.Next(3) == 0)
            {
                await writer.FlushAsync(ct);
            }
        }

        await writer.FlushAsync(ct);
        writer.Complete();
    }

    private static async Task<byte[]> ReadRandomlyAsync(
        CryptoPipeReader reader, int enableAt, byte[] key, Random rng, CancellationToken ct)
    {
        var sink = new ArrayBufferWriter<byte>();
        long consumedTotal = 0;
        bool enabled = false;
        while (true)
        {
            if (!enabled && consumedTotal == enableAt)
            {
                reader.EnableEncryption(key);
                enabled = true;
            }

            var result = await reader.ReadAsync(ct);
            var buffer = result.Buffer;
            long available = buffer.Length;
            long take;
            if (!enabled)
            {
                take = Math.Min(available, enableAt - consumedTotal);
            }
            else
            {
                take = rng.Next(4) == 0 ? available : rng.NextInt64(0, available + 1);
                take = Math.Max(take, available - 4096);
            }

            var consumedSlice = buffer.Slice(0, take);
            foreach (var segment in consumedSlice)
            {
                sink.Write(segment.Span);
            }

            consumedTotal += take;
            var consumedPos = consumedSlice.End;
            var examinedPos = enabled ? buffer.End : consumedPos;
            reader.AdvanceTo(consumedPos, examinedPos);

            if (result.IsCompleted && take == available)
            {
                if (!enabled && consumedTotal == enableAt)
                {
                    reader.EnableEncryption(key);
                    enabled = true;
                    continue;
                }

                reader.Complete();
                return sink.WrittenSpan.ToArray();
            }
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    public async Task Writer_ShouldMatchReferenceWire_ForRandomChunksAndFlushes(int seed)
    {
        var ct = TestContext.Current.CancellationToken;
        var rng = new Random(seed);
        var key = MakeKey(rng);
        int length = PickLength(rng, seed);
        var plain = new byte[length];
        rng.NextBytes(plain);
        int enableAt = rng.Next(0, length + 1);

        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);

        var writeTask = WriteRandomlyAsync(writer, plain, enableAt, key, rng, ct);
        var wire = await DrainAsync(pipe.Reader, ct);
        await writeTask;

        Assert.Equal(ExpectedWire(key, plain, enableAt), wire);
    }

    [Theory]
    [InlineData(21)]
    [InlineData(22)]
    [InlineData(23)]
    [InlineData(24)]
    [InlineData(25)]
    [InlineData(26)]
    [InlineData(27)]
    [InlineData(28)]
    [InlineData(29)]
    [InlineData(30)]
    [InlineData(31)]
    [InlineData(32)]
    public async Task Reader_ShouldRecoverPlaintext_FromReferenceWire_ForRandomChunksAndAdvances(int seed)
    {
        var ct = TestContext.Current.CancellationToken;
        var rng = new Random(seed);
        var key = MakeKey(rng);
        int length = PickLength(rng, seed);
        var plain = new byte[length];
        rng.NextBytes(plain);
        int enableAt = rng.Next(0, length + 1);
        var wire = ExpectedWire(key, plain, enableAt);

        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);

        var feed = Task.Run(async () =>
        {
            int offset = 0;
            while (offset < wire.Length)
            {
                int chunk = Math.Min(wire.Length - offset, rng.Next(1, MaxChunk + 1));
                await pipe.Writer.WriteAsync(wire.AsMemory(offset, chunk), ct);
                offset += chunk;
            }

            await pipe.Writer.CompleteAsync();
        }, ct);

        var recovered = await ReadRandomlyAsync(reader, enableAt, key, new Random(seed + 1000), ct);
        await feed;

        Assert.Equal(plain, recovered);
    }

    [Theory]
    [InlineData(41)]
    [InlineData(42)]
    [InlineData(43)]
    [InlineData(44)]
    [InlineData(45)]
    [InlineData(46)]
    [InlineData(47)]
    [InlineData(48)]
    public async Task RoundTrip_WriterToReader_ThroughBackpressuredPipe(int seed)
    {
        var ct = TestContext.Current.CancellationToken;
        var rng = new Random(seed);
        var key = MakeKey(rng);
        int length = PickLength(rng, seed);
        var plain = new byte[length];
        rng.NextBytes(plain);
        int enableAt = rng.Next(0, length + 1);

        var pipe = new Pipe(new PipeOptions(
            pauseWriterThreshold: 16 * 1024,
            resumeWriterThreshold: 8 * 1024,
            useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        using var reader = new CryptoPipeReader(pipe.Reader);

        var writeTask = Task.Run(() => WriteRandomlyAsync(writer, plain, enableAt, key, new Random(seed + 1), ct), ct);
        var recovered = await ReadRandomlyAsync(reader, enableAt, key, new Random(seed + 2), ct);
        await writeTask;

        Assert.Equal(plain, recovered);
    }

    [Fact]
    public async Task Reader_ShouldWaitForNewData_WhenEverythingExaminedButNotConsumed()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(7));
        var plain = new byte[3000];
        new Random(8).NextBytes(plain);
        var wire = ReferenceTransform(key, true, plain);

        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        await pipe.Writer.WriteAsync(wire.AsMemory(0, 1000), ct);
        var first = await reader.ReadAsync(ct);
        Assert.Equal(1000, first.Buffer.Length);
        Assert.Equal(plain.AsSpan(0, 1000).ToArray(), first.Buffer.ToArray());
        reader.AdvanceTo(first.Buffer.GetPosition(400), first.Buffer.End);

        var pending = reader.ReadAsync(ct);
        Assert.False(pending.IsCompleted);

        await pipe.Writer.WriteAsync(wire.AsMemory(1000, 1000), ct);
        var second = await pending;
        Assert.Equal(1600, second.Buffer.Length);
        Assert.Equal(plain.AsSpan(400, 1600).ToArray(), second.Buffer.ToArray());
        reader.AdvanceTo(second.Buffer.GetPosition(100), second.Buffer.End);

        var pending2 = reader.ReadAsync(ct);
        Assert.False(pending2.IsCompleted);
        await pipe.Writer.WriteAsync(wire.AsMemory(2000, 1000), ct);
        await pipe.Writer.CompleteAsync();
        var third = await pending2;
        Assert.Equal(plain.AsSpan(500).ToArray(), third.Buffer.ToArray());
        reader.AdvanceTo(third.Buffer.End);

        var last = await reader.ReadAsync(ct);
        Assert.True(last.IsCompleted);
        Assert.Equal(0, last.Buffer.Length);
        reader.AdvanceTo(last.Buffer.End);
        reader.Complete();
    }

    [Fact]
    public async Task Reader_ShouldReturnImmediately_WhenConsumedEqualsExaminedBeforeEnd()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(9));
        var plain = new byte[100];
        new Random(10).NextBytes(plain);

        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);
        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, plain), ct);

        var first = await reader.ReadAsync(ct);
        var mid = first.Buffer.GetPosition(30);
        reader.AdvanceTo(mid, mid);

        var again = reader.ReadAsync(ct);
        Assert.True(again.IsCompleted);
        var second = await again;
        Assert.Equal(plain.AsSpan(30).ToArray(), second.Buffer.ToArray());
        reader.AdvanceTo(second.Buffer.End);
        reader.Complete();
    }

    [Fact]
    public async Task Reader_TryRead_ShouldReflectAvailability_AndDecrypt()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(11));
        var plain = new byte[500];
        new Random(12).NextBytes(plain);

        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        Assert.False(reader.TryRead(out _));

        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, plain), ct);
        Assert.True(reader.TryRead(out var result));
        Assert.Equal(plain, result.Buffer.ToArray());
        reader.AdvanceTo(result.Buffer.End);

        Assert.False(reader.TryRead(out _));
        pipe.Writer.Complete();
        Assert.True(reader.TryRead(out var final));
        Assert.True(final.IsCompleted);
        Assert.Equal(0, final.Buffer.Length);
        reader.AdvanceTo(final.Buffer.End);
        reader.Complete();
    }

    [Fact]
    public async Task Reader_CancelPendingRead_ShouldYieldCanceledResult_AndStayUsable()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(13));
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        var pending = reader.ReadAsync(ct);
        Assert.False(pending.IsCompleted);
        reader.CancelPendingRead();
        var canceled = await pending;
        Assert.True(canceled.IsCanceled);
        Assert.False(canceled.IsCompleted);
        reader.AdvanceTo(canceled.Buffer.Start, canceled.Buffer.End);

        var plain = new byte[64];
        new Random(14).NextBytes(plain);
        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, plain), ct);
        var next = await reader.ReadAsync(ct);
        Assert.False(next.IsCanceled);
        Assert.Equal(plain, next.Buffer.ToArray());
        reader.AdvanceTo(next.Buffer.End);

        reader.CancelPendingRead();
        var preCanceled = await reader.ReadAsync(ct);
        Assert.True(preCanceled.IsCanceled);
        reader.AdvanceTo(preCanceled.Buffer.Start, preCanceled.Buffer.End);
        reader.Complete();
    }

    [Fact]
    public async Task Reader_ShouldThrowOperationCanceled_WhenTokenIsCanceled()
    {
        var key = MakeKey(new Random(15));
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        using var cts = new CancellationTokenSource();
        var pending = reader.ReadAsync(cts.Token);
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await pending);

        var plain = new byte[16];
        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, plain), TestContext.Current.CancellationToken);
        var next = await reader.ReadAsync(TestContext.Current.CancellationToken);
        Assert.Equal(plain, next.Buffer.ToArray());
        reader.AdvanceTo(next.Buffer.End);
        reader.Complete();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Reader_ShouldDeliverEveryByte_ThenReportCompletion_WhenInnerCompletes(bool encrypted)
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(16));
        var plain = new byte[70_001];
        new Random(17).NextBytes(plain);
        var wire = encrypted ? ReferenceTransform(key, true, plain) : plain;

        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        if (encrypted)
        {
            reader.EnableEncryption(key);
        }

        await pipe.Writer.WriteAsync(wire, ct);
        pipe.Writer.Complete();

        var sink = new ArrayBufferWriter<byte>();
        bool completedSeen = false;
        while (!completedSeen)
        {
            var result = await reader.ReadAsync(ct);
            long take = Math.Min(result.Buffer.Length, 1234);
            var slice = result.Buffer.Slice(0, take);
            foreach (var segment in slice)
            {
                sink.Write(segment.Span);
            }

            reader.AdvanceTo(slice.End, result.Buffer.End);
            completedSeen = result.IsCompleted && take == result.Buffer.Length;
        }

        Assert.Equal(plain, sink.WrittenSpan.ToArray());
        reader.Complete();
    }

    [Fact]
    public async Task Writer_FlushAsync_ShouldObeyBackpressure_AndDeliverCiphertext()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(18));
        var plain = new byte[8192];
        new Random(19).NextBytes(plain);

        var pipe = new Pipe(new PipeOptions(
            pauseWriterThreshold: 1024,
            resumeWriterThreshold: 512,
            useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write(plain);
        var flush = writer.FlushAsync(ct);
        Assert.False(flush.IsCompleted);
        await Task.Delay(50, ct);
        Assert.False(flush.IsCompleted);

        var drain = Task.Run(() => DrainAsync(pipe.Reader, ct), ct);
        var result = await flush;
        Assert.False(result.IsCanceled);
        writer.Complete();
        var wire = await drain;
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
    }

    [Fact]
    public async Task Writer_CancelPendingFlush_ShouldReportCanceled_AndKeepDataFlowing()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(20));
        var plain = new byte[4096];
        new Random(21).NextBytes(plain);

        var pipe = new Pipe(new PipeOptions(
            pauseWriterThreshold: 1024,
            resumeWriterThreshold: 512,
            useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write(plain);
        var flush = writer.FlushAsync(ct);
        Assert.False(flush.IsCompleted);
        writer.CancelPendingFlush();
        var canceled = await flush;
        Assert.True(canceled.IsCanceled);

        writer.Complete();
        var wire = await DrainAsync(pipe.Reader, ct);
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
    }

    [Fact]
    public async Task Writer_CancelPendingFlush_BeforeFlush_ShouldCancelNextFlush()
    {
        var ct = TestContext.Current.CancellationToken;
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(MakeKey(new Random(22)));

        writer.CancelPendingFlush();
        writer.Write(new byte[10]);
        var result = await writer.FlushAsync(ct);
        Assert.True(result.IsCanceled);

        var next = await writer.FlushAsync(ct);
        Assert.False(next.IsCanceled);
        writer.Complete();
        Assert.Equal(10, (await DrainAsync(pipe.Reader, ct)).Length);
    }

    [Fact]
    public async Task Writer_CompleteWithException_ShouldFaultReader()
    {
        var ct = TestContext.Current.CancellationToken;
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(MakeKey(new Random(23)));

        writer.Write(new byte[100]);
        var boom = new IOException("boom");
        writer.Complete(boom);

        var thrown = await Assert.ThrowsAsync<IOException>(async () => await pipe.Reader.ReadAsync(ct));
        Assert.Same(boom, thrown);
    }

    [Fact]
    public async Task Writer_Complete_ShouldFlushPendingBytes_AndCompleteReader()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(24));
        var plain = new byte[777];
        new Random(25).NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write(plain);
        writer.Complete();

        var wire = await DrainAsync(pipe.Reader, ct);
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
    }

    [Fact]
    public async Task Writer_CompleteAsync_ShouldFlushPendingBytes_AndCompleteReader()
    {
        var ct = TestContext.Current.CancellationToken;
        var key = MakeKey(new Random(26));
        var plain = new byte[5000];
        new Random(27).NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write(plain);
        await writer.CompleteAsync();

        var wire = await DrainAsync(pipe.Reader, ct);
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Writer_UnflushedBytes_ShouldCountAdvancedBytes_WhenSupported(bool encrypted)
    {
        var ct = TestContext.Current.CancellationToken;
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        if (encrypted)
        {
            writer.EnableEncryption(MakeKey(new Random(28)));
        }

        if (!writer.CanGetUnflushedBytes)
        {
            return;
        }

        Assert.Equal(0, writer.UnflushedBytes);
        writer.Write(new byte[10]);
        Assert.Equal(10, writer.UnflushedBytes);
        writer.Write(new byte[10_000]);
        Assert.Equal(10_010, writer.UnflushedBytes);
        await writer.FlushAsync(ct);
        Assert.Equal(0, writer.UnflushedBytes);

        var result = await pipe.Reader.ReadAsync(ct);
        Assert.Equal(10_010, result.Buffer.Length);
        pipe.Reader.AdvanceTo(result.Buffer.End);
        writer.Complete();
    }

    [Fact]
    public void Writer_EnableEncryption_Twice_ShouldThrow()
    {
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(MakeKey(new Random(29)));
        Assert.True(writer.EncryptionEnabled);
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(MakeKey(new Random(30))));
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(PacketCipher.CreateEncryptor(MakeKey(new Random(31)))));
    }

    [Fact]
    public void Writer_EnableEncryption_AfterDispose_ShouldThrow()
    {
        var pipe = new Pipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.Dispose();
        Assert.ThrowsAny<InvalidOperationException>(() => writer.EnableEncryption(MakeKey(new Random(32))));
    }

    [Fact]
    public void Writer_EnableEncryption_WithUnflushedPlaintext_ShouldThrow()
    {
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.Write(new byte[3]);
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(MakeKey(new Random(33))));
        Assert.False(writer.EncryptionEnabled);
    }

    [Fact]
    public void Writer_EnableEncryption_WithBadKeyLength_ShouldThrow()
    {
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        Assert.ThrowsAny<ArgumentException>(() => writer.EnableEncryption(new byte[15]));
        Assert.ThrowsAny<ArgumentException>(() => writer.EnableEncryption(new byte[32]));
        Assert.False(writer.EncryptionEnabled);
    }

    [Fact]
    public void Reader_EnableEncryption_Twice_ShouldThrow()
    {
        var pipe = new Pipe();
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(MakeKey(new Random(34)));
        Assert.True(reader.EncryptionEnabled);
        Assert.Throws<InvalidOperationException>(() => reader.EnableEncryption(MakeKey(new Random(35))));
        Assert.Throws<InvalidOperationException>(() => reader.EnableEncryption(PacketCipher.CreateDecryptor(MakeKey(new Random(36)))));
    }

    [Fact]
    public void Reader_EnableEncryption_AfterDispose_ShouldThrow()
    {
        var pipe = new Pipe();
        var reader = new CryptoPipeReader(pipe.Reader);
        reader.Dispose();
        Assert.ThrowsAny<InvalidOperationException>(() => reader.EnableEncryption(MakeKey(new Random(37))));
    }

    [Fact]
    public async Task Reader_EnableEncryption_WithOutstandingRead_ShouldThrow()
    {
        var ct = TestContext.Current.CancellationToken;
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        await pipe.Writer.WriteAsync(new byte[10], ct);
        var result = await reader.ReadAsync(ct);
        Assert.Throws<InvalidOperationException>(() => reader.EnableEncryption(MakeKey(new Random(38))));
        Assert.False(reader.EncryptionEnabled);
        reader.AdvanceTo(result.Buffer.End);
        reader.Complete();
    }

    [Fact]
    public void Reader_EnableEncryption_WithBadKeyLength_ShouldThrow()
    {
        var pipe = new Pipe();
        using var reader = new CryptoPipeReader(pipe.Reader);
        Assert.ThrowsAny<ArgumentException>(() => reader.EnableEncryption(new byte[15]));
        Assert.False(reader.EncryptionEnabled);
    }

    [Fact]
    public async Task Reader_AfterDispose_ReadAsync_ShouldThrow()
    {
        var pipe = new Pipe();
        var reader = new CryptoPipeReader(pipe.Reader);
        reader.Dispose();
        await Assert.ThrowsAnyAsync<InvalidOperationException>(async () => await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.ThrowsAny<InvalidOperationException>(() => reader.TryRead(out _));
    }

    [Fact]
    public async Task Reader_AfterDispose_ReadAsync_ShouldThrow_WhenEncrypted()
    {
        var pipe = new Pipe();
        var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(MakeKey(new Random(39)));
        reader.Dispose();
        await Assert.ThrowsAnyAsync<InvalidOperationException>(async () => await reader.ReadAsync(TestContext.Current.CancellationToken));
        Assert.ThrowsAny<InvalidOperationException>(() => reader.TryRead(out _));
    }

    private static byte[] Body(Random rng, int size)
    {
        var body = new byte[size];
        rng.NextBytes(body);
        return body;
    }

    [Theory]
    [InlineData(-1, 101)]
    [InlineData(64, 102)]
    [InlineData(0, 103)]
    [InlineData(256, 104)]
    public async Task PacketPipeline_EndToEnd_EncryptionSwitchedOnMidStream(int compressionThreshold, int seed)
    {
        var ct = TestContext.Current.CancellationToken;
        var rng = new Random(seed);
        var key = MakeKey(rng);
        const int count = 300;
        int enableAfter = rng.Next(0, count);
        var ids = new int[count];
        var bodies = new byte[count][];
        for (int i = 0; i < count; i++)
        {
            ids[i] = rng.Next(0, 128);
            int size = rng.Next(4) switch
            {
                0 => 0,
                1 => rng.Next(1, 32),
                2 => rng.Next(32, 600),
                _ => rng.Next(600, 20_000),
            };
            bodies[i] = Body(rng, size);
        }

        var pipe = new Pipe(new PipeOptions(
            pauseWriterThreshold: 32 * 1024,
            resumeWriterThreshold: 16 * 1024,
            useSynchronizationContext: false));
        using var writer = new MinecraftPacketPipeWriter(pipe.Writer);
        using var reader = new MinecraftPacketPipeReader(pipe.Reader);
        writer.CompressionThreshold = compressionThreshold;
        reader.CompressionThreshold = compressionThreshold;

        var peerReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var writeTask = Task.Run(async () =>
        {
            for (int i = 0; i < count; i++)
            {
                if (i == enableAfter + 1)
                {
                    await writer.FlushAsync(ct);
                    await peerReady.Task.WaitAsync(ct);
                    writer.EnableEncryption(key);
                }

                writer.WritePacket(ids[i], bodies[i]);
                if (rng.Next(3) == 0)
                {
                    await writer.FlushAsync(ct);
                }
            }

            await writer.FlushAsync(ct);
            writer.Complete();
        }, ct);

        int index = 0;
        try
        {
            await foreach (var packet in reader.ReadPacketsAsync(ct))
            {
                Assert.True(index < count, "more packets than written");
                Assert.Equal(ids[index], packet.Id);
                Assert.Equal(bodies[index], packet.Data.ToArray());
                if (index == enableAfter)
                {
                    reader.EnableEncryption(key);
                    peerReady.SetResult();
                }

                index++;
            }
        }
        catch
        {
            peerReady.TrySetResult();
            pipe.Reader.Complete();
            throw;
        }
        finally
        {
            await writeTask;
        }

        Assert.Equal(count, index);
    }

    [Fact]
    public async Task PacketPipeline_ReadPacketAsync_EncryptedFromStart_WithCompression()
    {
        var ct = TestContext.Current.CancellationToken;
        var rng = new Random(200);
        var key = MakeKey(rng);
        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var writer = new MinecraftPacketPipeWriter(pipe.Writer);
        using var reader = new MinecraftPacketPipeReader(pipe.Reader);
        writer.CompressionThreshold = 32;
        reader.CompressionThreshold = 32;
        writer.EnableEncryption(key);
        reader.EnableEncryption(key);

        var small = Body(rng, 10);
        var big = Body(rng, 40_000);
        writer.WritePacket(1, small);
        writer.WritePacket(2, big);
        writer.WritePacket(3, ReadOnlySpan<byte>.Empty);
        await writer.FlushAsync(ct);

        var p1 = await reader.ReadPacketAsync(ct);
        Assert.Equal(1, p1.Id);
        Assert.Equal(small, p1.Data.ToArray());
        var p2 = await reader.ReadPacketAsync(ct);
        Assert.Equal(2, p2.Id);
        Assert.Equal(big, p2.Data.ToArray());
        var p3 = await reader.ReadPacketAsync(ct);
        Assert.Equal(3, p3.Id);
        Assert.Equal(0, p3.Data.Length);

        writer.Complete();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await reader.ReadPacketAsync(ct));
    }
}
