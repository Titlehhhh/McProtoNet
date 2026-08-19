using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Cryptography;
using McProtoNet.Net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

public class CryptoPipeWriterBlackBoxTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    private readonly ITestOutputHelper _output;

    public CryptoPipeWriterBlackBoxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static byte[] RandomKey(Random random)
    {
        var key = new byte[PacketCipher.SharedSecretLength];
        random.NextBytes(key);
        return key;
    }

    private static byte[] RandomBytes(Random random, int count)
    {
        var data = new byte[count];
        random.NextBytes(data);
        return data;
    }

    private static IBufferedCipher Reference(byte[] key, bool forEncryption)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(key), key));
        return cipher;
    }

    private static byte[] ReferenceTransform(byte[] key, bool forEncryption, ReadOnlySpan<byte> data)
    {
        var cipher = Reference(key, forEncryption);
        var output = new byte[data.Length];
        int written = cipher.ProcessBytes(data.ToArray(), 0, data.Length, output, 0);
        written += cipher.DoFinal(output, written);
        Assert.Equal(data.Length, written);
        return output;
    }

    private static byte[] Wire(byte[] key, byte[] plain, int prefix)
    {
        var wire = new byte[plain.Length];
        plain.AsSpan(0, prefix).CopyTo(wire);
        ReferenceTransform(key, true, plain.AsSpan(prefix)).CopyTo(wire.AsSpan(prefix));
        return wire;
    }

    private static Pipe NewPipe(long pauseWriterThreshold = 0, long resumeWriterThreshold = 0)
    {
        return new Pipe(new PipeOptions(
            pauseWriterThreshold: pauseWriterThreshold,
            resumeWriterThreshold: resumeWriterThreshold,
            useSynchronizationContext: false));
    }

    private static async Task<byte[]> DrainAsync(PipeReader reader, CancellationToken ct, Random? random = null)
    {
        var collected = new ArrayBufferWriter<byte>();
        while (true)
        {
            var result = await reader.ReadAsync(ct);
            var buffer = result.Buffer;
            long take = random is null || result.IsCompleted
                ? buffer.Length
                : (long)(random.NextDouble() * buffer.Length);
            var slice = buffer.Slice(0, take);
            foreach (var segment in slice)
            {
                collected.Write(segment.Span);
            }

            reader.AdvanceTo(slice.End, buffer.End);
            if (result.IsCompleted && take == buffer.Length)
            {
                return collected.WrittenSpan.ToArray();
            }

            if (random is not null && random.Next(4) == 0)
            {
                await Task.Yield();
            }
        }
    }

    private static void WriteAll(PipeWriter writer, ReadOnlySpan<byte> data)
    {
        data.CopyTo(writer.GetSpan(data.Length));
        writer.Advance(data.Length);
    }

    private static int PickHint(Random random)
    {
        return random.Next(6) switch
        {
            0 => 0,
            1 => 0,
            2 => random.Next(1, 17),
            3 => random.Next(1, 4097),
            4 => random.Next(4097, 70_001),
            _ => random.Next(70_001, 200_001),
        };
    }

    public static IEnumerable<object[]> Seeds()
    {
        for (int seed = 1; seed <= 30; seed++)
        {
            yield return [seed];
        }
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task Stream_RandomSizeHints_PartialAndZeroAdvances_UnderBackpressure_MatchesReference(int seed)
    {
        _output.WriteLine($"seed={seed}");
        var random = new Random(seed);
        var key = RandomKey(random);
        var plain = RandomBytes(random, random.Next(1, 400_000));
        int prefix = random.Next(4) == 0 ? 0 : random.Next(plain.Length + 1);
        long pause = random.Next(3) switch { 0 => 64, 1 => 4096, _ => 64 * 1024 };

        var pipe = NewPipe(pause, pause / 2);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        using var writer = new CryptoPipeWriter(pipe.Writer);

        var drain = Task.Run(() => DrainAsync(pipe.Reader, cts.Token, new Random(seed + 500)), cts.Token);

        int position = 0;
        long sinceFlush = 0;
        bool enabled = false;
        while (true)
        {
            if (!enabled && position >= prefix)
            {
                if (writer.UnflushedBytes > 0)
                {
                    await writer.FlushAsync(cts.Token);
                    sinceFlush = 0;
                }

                writer.EnableEncryption(key);
                enabled = true;
                Assert.True(writer.EncryptionEnabled);
                Assert.Equal(0, writer.UnflushedBytes);
            }

            if (position == plain.Length)
            {
                break;
            }

            int limit = enabled ? plain.Length - position : prefix - position;
            int hint = PickHint(random);
            int available;
            int take;
            if (random.Next(2) == 0)
            {
                var memory = writer.GetMemory(hint);
                Assert.True(memory.Length >= Math.Max(hint, 1));
                available = memory.Length;
                take = Math.Min(limit, random.Next(3) == 0 ? 0 : random.Next(available + 1));
                plain.AsSpan(position, take).CopyTo(memory.Span);
            }
            else
            {
                var span = writer.GetSpan(hint);
                Assert.True(span.Length >= Math.Max(hint, 1));
                available = span.Length;
                take = Math.Min(limit, random.Next(3) == 0 ? 0 : random.Next(available + 1));
                plain.AsSpan(position, take).CopyTo(span);
            }

            writer.Advance(take);
            position += take;
            sinceFlush += take;
            Assert.Equal(sinceFlush, writer.UnflushedBytes);

            if (random.Next(4) == 0)
            {
                var result = await writer.FlushAsync(cts.Token);
                Assert.False(result.IsCompleted);
                Assert.False(result.IsCanceled);
                sinceFlush = 0;
                Assert.Equal(0, writer.UnflushedBytes);
            }
        }

        await writer.FlushAsync(cts.Token);
        writer.Complete();
        byte[] wire = await drain;

        Assert.Equal(Wire(key, plain, prefix), wire);
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task Stream_GetMemoryAgainWithoutAdvance_KeepsOnlyAdvancedBytes(int seed)
    {
        _output.WriteLine($"seed={seed}");
        var random = new Random(seed + 100);
        var key = RandomKey(random);
        var plain = RandomBytes(random, random.Next(1, 100_000));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        int position = 0;
        while (position < plain.Length)
        {
            int rerequests = random.Next(4);
            for (int i = 0; i < rerequests; i++)
            {
                var junk = writer.GetSpan(PickHint(random));
                junk.Fill(0xAB);
            }

            var span = writer.GetSpan(PickHint(random));
            int take = Math.Min(plain.Length - position, random.Next(span.Length + 1));
            plain.AsSpan(position, take).CopyTo(span);
            writer.Advance(take);
            position += take;
            if (random.Next(5) == 0)
            {
                await writer.FlushAsync(Ct);
            }
        }

        await writer.CompleteAsync();
        var wire = await DrainAsync(pipe.Reader, Ct);
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
    }

    [Fact]
    public async Task ZeroLength_GetMemoryAndAdvance_ProduceNoBytes_AndKeepStream()
    {
        var key = RandomKey(new Random(3));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);

        Assert.True(writer.GetMemory(0).Length > 0);
        Assert.True(writer.GetSpan(0).Length > 0);
        Assert.True(writer.GetMemory().Length > 0);
        writer.Advance(0);
        Assert.Equal(0, writer.UnflushedBytes);
        var emptyFlush = await writer.FlushAsync(Ct);
        Assert.False(emptyFlush.IsCompleted);
        Assert.False(emptyFlush.IsCanceled);
        Assert.False(pipe.Reader.TryRead(out _));

        writer.EnableEncryption(key);
        Assert.True(writer.GetMemory(0).Length > 0);
        writer.Advance(0);
        writer.Advance(0);
        Assert.Equal(0, writer.UnflushedBytes);
        await writer.FlushAsync(Ct);
        Assert.False(pipe.Reader.TryRead(out _));

        byte[] plain = RandomBytes(new Random(4), 333);
        WriteAll(writer, plain);
        writer.Advance(0);
        Assert.Equal(plain.Length, writer.UnflushedBytes);
        await writer.FlushAsync(Ct);
        writer.Complete();

        Assert.Equal(ReferenceTransform(key, true, plain), await DrainAsync(pipe.Reader, Ct));
    }

    [Fact]
    public async Task EnableEncryption_RejectedForPendingPlaintext_LeavesWriterAndCipherUsable()
    {
        var key = RandomKey(new Random(5));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        var cipher = new RecordingCipher(PacketCipher.CreateEncryptor(key));
        byte[] head = RandomBytes(new Random(6), 50);
        byte[] tail = RandomBytes(new Random(7), 2000);

        WriteAll(writer, head);
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(cipher));
        Assert.False(writer.EncryptionEnabled);
        Assert.Equal(0, cipher.TransformedBytes);
        Assert.False(cipher.Disposed);
        Assert.Equal(head.Length, writer.UnflushedBytes);

        await writer.FlushAsync(Ct);
        writer.EnableEncryption(cipher);
        Assert.True(writer.EncryptionEnabled);
        WriteAll(writer, tail);
        await writer.FlushAsync(Ct);
        writer.Complete();

        byte[] wire = await DrainAsync(pipe.Reader, Ct);
        Assert.Equal(head, wire.AsSpan(0, head.Length).ToArray());
        Assert.Equal(ReferenceTransform(key, true, tail), wire.AsSpan(head.Length).ToArray());
    }

    [Fact]
    public async Task EnableEncryption_RejectedSecondTime_DoesNotDisturbRunningCipher()
    {
        var key = RandomKey(new Random(8));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        byte[] plain = RandomBytes(new Random(9), 5000);

        WriteAll(writer, plain.AsSpan(0, 1000));
        var second = new RecordingCipher(PacketCipher.CreateEncryptor(RandomKey(new Random(10))));
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(second));
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(RandomKey(new Random(11))));
        Assert.Equal(1000, writer.UnflushedBytes);
        WriteAll(writer, plain.AsSpan(1000));
        await writer.FlushAsync(Ct);
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(second));
        writer.Complete();

        Assert.Equal(ReferenceTransform(key, true, plain), await DrainAsync(pipe.Reader, Ct));
        Assert.Equal(0, second.TransformedBytes);
    }

    [Fact]
    public async Task EnableEncryption_WithUnflushedBytesOnLowerPipe_NeverMisalignsWire()
    {
        var key = RandomKey(new Random(12));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        byte[] head = RandomBytes(new Random(13), 40);
        byte[] tail = RandomBytes(new Random(14), 400);
        WriteAll(pipe.Writer, head);

        bool enabled;
        try
        {
            writer.EnableEncryption(key);
            enabled = true;
        }
        catch (InvalidOperationException)
        {
            enabled = false;
        }

        if (!enabled)
        {
            await writer.FlushAsync(Ct);
            writer.EnableEncryption(key);
        }

        WriteAll(writer, tail);
        await writer.FlushAsync(Ct);
        writer.Complete();

        byte[] wire = await DrainAsync(pipe.Reader, Ct);
        Assert.Equal(head, wire.AsSpan(0, head.Length).ToArray());
        Assert.Equal(ReferenceTransform(key, true, tail), wire.AsSpan(head.Length).ToArray());
    }

    [Fact]
    public void UnflushedBytes_WhenLowerPipeDoesNotSupportIt_ReportsNotSupported()
    {
        var lower = new NoUnflushedBytesWriter(NewPipe().Writer);
        using var writer = new CryptoPipeWriter(lower);

        Assert.False(writer.CanGetUnflushedBytes);
        Assert.Throws<NotSupportedException>(() => writer.UnflushedBytes);

        writer.EnableEncryption(RandomKey(new Random(15)));
        WriteAll(writer, new byte[10]);
        Assert.False(writer.CanGetUnflushedBytes);
        Assert.Throws<NotSupportedException>(() => writer.UnflushedBytes);
    }

    [Fact]
    public async Task FlushAsync_TokenCanceledWhilePaused_Throws_KeepsBytes_AndStaysUsable()
    {
        var key = RandomKey(new Random(16));
        var pipe = NewPipe(pauseWriterThreshold: 1024, resumeWriterThreshold: 512);
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        byte[] first = RandomBytes(new Random(17), 3000);
        byte[] second = RandomBytes(new Random(18), 700);

        WriteAll(writer, first);
        using var cts = new CancellationTokenSource();
        var flush = writer.FlushAsync(cts.Token);
        Assert.False(flush.IsCompleted);
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await flush);

        Assert.Equal(0, writer.UnflushedBytes);
        WriteAll(writer, second);
        Assert.Equal(second.Length, writer.UnflushedBytes);
        var drain = Task.Run(() => DrainAsync(pipe.Reader, Ct), Ct);
        await writer.FlushAsync(Ct);
        writer.Complete();

        byte[] wire = await drain;
        Assert.Equal(ReferenceTransform(key, true, [.. first, .. second]), wire);
    }

    [Fact]
    public async Task FlushAsync_AfterReaderCompleted_ReportsCompleted_WithoutThrowing()
    {
        var key = RandomKey(new Random(19));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        pipe.Reader.Complete();
        WriteAll(writer, new byte[100]);
        var result = await writer.FlushAsync(Ct);
        Assert.True(result.IsCompleted);

        WriteAll(writer, new byte[100]);
        var again = await writer.FlushAsync(Ct);
        Assert.True(again.IsCompleted);
        writer.Complete();
    }

    [Fact]
    public async Task FlushAsync_PassesLowerFlushResultThrough()
    {
        var lower = new ScriptedFlushWriter(NewPipe().Writer)
        {
            NextResult = new FlushResult(isCanceled: true, isCompleted: true),
        };
        using var writer = new CryptoPipeWriter(lower);
        writer.EnableEncryption(RandomKey(new Random(20)));
        WriteAll(writer, new byte[10]);

        var result = await writer.FlushAsync(Ct);
        Assert.True(result.IsCanceled);
        Assert.True(result.IsCompleted);
        Assert.Equal(1, lower.FlushCalls);
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task FlushAsync_LowerFlushThrowsOnce_RetrySendsEveryByteExactlyOnce(int seed)
    {
        _output.WriteLine($"seed={seed}");
        var random = new Random(seed + 200);
        var key = RandomKey(random);
        var plain = RandomBytes(random, random.Next(1, 60_000));
        var pipe = NewPipe();
        var lower = new ScriptedFlushWriter(pipe.Writer);
        using var writer = new CryptoPipeWriter(lower);
        writer.EnableEncryption(key);

        int position = 0;
        int failures = 0;
        while (position < plain.Length)
        {
            int chunk = Math.Min(plain.Length - position, random.Next(1, 5000));
            WriteAll(writer, plain.AsSpan(position, chunk));
            position += chunk;
            if (random.Next(3) != 0)
            {
                continue;
            }

            if (random.Next(2) == 0)
            {
                lower.ThrowOnNextFlush = new IOException("socket reset");
                await Assert.ThrowsAsync<IOException>(async () => await writer.FlushAsync(Ct));
                failures++;
                Assert.Equal(0, writer.UnflushedBytes - lower.UnflushedBytes);
            }

            await writer.FlushAsync(Ct);
        }

        await writer.FlushAsync(Ct);
        writer.Complete();
        byte[] wire = await DrainAsync(pipe.Reader, Ct);
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
        _output.WriteLine($"failures={failures}");
    }

    [Fact]
    public async Task Complete_Twice_IsIdempotent_AndReaderSeesCleanEnd()
    {
        var key = RandomKey(new Random(21));
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        byte[] plain = RandomBytes(new Random(22), 77);
        WriteAll(writer, plain);

        writer.Complete();
        writer.Complete();
        writer.Complete(new IOException("late"));
        await writer.CompleteAsync();

        byte[] wire = await DrainAsync(pipe.Reader, Ct);
        Assert.Equal(ReferenceTransform(key, true, plain), wire);
    }

    [Fact]
    public async Task CompleteAsync_WithException_FaultsReader_AndDropsPending()
    {
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(RandomKey(new Random(23)));
        WriteAll(writer, new byte[64]);
        var boom = new TimeoutException("boom");

        await writer.CompleteAsync(boom);

        var seen = await Assert.ThrowsAsync<TimeoutException>(async () => await pipe.Reader.ReadAsync(Ct));
        Assert.Same(boom, seen);
    }

    [Fact]
    public void GetSpan_AfterComplete_Throws()
    {
        var pipe = NewPipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(RandomKey(new Random(24)));
        WriteAll(writer, new byte[5]);
        writer.Complete();

        Assert.Throws<InvalidOperationException>(() => writer.GetSpan(1));
        Assert.Throws<InvalidOperationException>(() => writer.GetMemory(1));
    }

    [Fact]
    public async Task Dispose_AfterComplete_DoesNotThrow_AndReaderStillSeesData()
    {
        var key = RandomKey(new Random(25));
        var pipe = NewPipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        byte[] plain = RandomBytes(new Random(26), 1234);
        WriteAll(writer, plain);
        await writer.CompleteAsync();

        writer.Dispose();
        writer.Dispose();

        Assert.Equal(ReferenceTransform(key, true, plain), await DrainAsync(pipe.Reader, Ct));
    }

    [Fact]
    public async Task Dispose_MidWrite_RejectsAdvanceAndFlush_AndLowerPipeStaysUsable()
    {
        var key = RandomKey(new Random(27));
        var pipe = NewPipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        byte[] flushed = RandomBytes(new Random(28), 300);
        WriteAll(writer, flushed);
        await writer.FlushAsync(Ct);

        var span = writer.GetSpan(100);
        span[..50].Fill(1);
        writer.Dispose();

        Assert.ThrowsAny<Exception>(() => writer.Advance(50));
        Assert.ThrowsAny<Exception>(() => writer.GetSpan(1));
        await Assert.ThrowsAnyAsync<Exception>(async () => await writer.FlushAsync(Ct));

        byte[] extra = [9, 8, 7];
        await pipe.Writer.WriteAsync(extra, Ct);
        pipe.Writer.Complete();
        byte[] wire = await DrainAsync(pipe.Reader, Ct);
        Assert.Equal(ReferenceTransform(key, true, flushed), wire.AsSpan(0, flushed.Length).ToArray());
        Assert.Equal(extra, wire.AsSpan(flushed.Length).ToArray());
    }

    [Fact]
    public async Task Dispose_WhileFlushPausedOnBackpressure_FlushStillCompletes_WithIntactCiphertext()
    {
        var key = RandomKey(new Random(29));
        var pipe = NewPipe(pauseWriterThreshold: 512, resumeWriterThreshold: 256);
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        byte[] plain = RandomBytes(new Random(30), 4000);
        WriteAll(writer, plain);

        var flush = writer.FlushAsync(Ct);
        Assert.False(flush.IsCompleted);
        writer.Dispose();

        var drain = Task.Run(() => DrainAsync(pipe.Reader, Ct), Ct);
        var result = await flush.AsTask().WaitAsync(Timeout, Ct);
        Assert.False(result.IsCanceled);
        pipe.Writer.Complete();
        Assert.Equal(ReferenceTransform(key, true, plain), await drain);
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task Cipher_SeesEveryByteExactlyOnce_NeverEmpty_AndIsDisposedWithWriter(int seed)
    {
        _output.WriteLine($"seed={seed}");
        var random = new Random(seed + 300);
        var key = RandomKey(random);
        var plain = RandomBytes(random, random.Next(0, 50_000));
        var cipher = new RecordingCipher(PacketCipher.CreateEncryptor(key));
        var pipe = NewPipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(cipher);

        int position = 0;
        while (position < plain.Length)
        {
            var span = writer.GetSpan(PickHint(random));
            int take = Math.Min(plain.Length - position, random.Next(span.Length + 1));
            plain.AsSpan(position, take).CopyTo(span);
            writer.Advance(take);
            position += take;
            Assert.True(cipher.TransformedBytes <= position);
            if (random.Next(3) == 0)
            {
                await writer.FlushAsync(Ct);
                Assert.Equal(position, cipher.TransformedBytes);
            }
        }

        writer.Complete();
        Assert.Equal(plain.Length, cipher.TransformedBytes);
        Assert.False(cipher.SawEmptyCall);
        Assert.Equal(ReferenceTransform(key, true, plain), await DrainAsync(pipe.Reader, Ct));

        writer.Dispose();
        Assert.True(cipher.Disposed);
    }

    [Fact]
    public async Task Poisoned_ByLowerAdvance_CompleteAsyncAndDisposeAreSafe_EnableEncryptionThrows()
    {
        var pipe = NewPipe();
        var lower = new ScriptedFlushWriter(pipe.Writer) { ThrowOnNextAdvance = new IOException("lower advance") };
        var writer = new CryptoPipeWriter(lower);
        writer.EnableEncryption(RandomKey(new Random(31)));
        WriteAll(writer, new byte[20]);

        var cause = await Assert.ThrowsAsync<IOException>(async () => await writer.FlushAsync(Ct));
        Assert.Equal(0, writer.UnflushedBytes);
        Assert.True(writer.CanGetUnflushedBytes);
        var poisoned = Assert.Throws<InvalidOperationException>(() => writer.GetSpan(1));
        Assert.Same(cause, poisoned.InnerException);
        Assert.Same(cause, Assert.Throws<InvalidOperationException>(() => writer.Advance(1)).InnerException);
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(RandomKey(new Random(32))));
        try
        {
            writer.CancelPendingFlush();
        }
        catch (InvalidOperationException)
        {
        }

        await writer.CompleteAsync();
        Assert.Same(cause, await Assert.ThrowsAsync<IOException>(async () => await pipe.Reader.ReadAsync(Ct)));
        writer.Dispose();
        Assert.ThrowsAny<Exception>(() => writer.GetSpan(1));
    }

    [Fact]
    public async Task Poisoned_AdvanceZero_ThrowsLikeEveryOtherEntryPoint()
    {
        var pipe = NewPipe();
        var lower = new ScriptedFlushWriter(pipe.Writer) { ThrowOnNextAdvance = new IOException("lower advance") };
        using var writer = new CryptoPipeWriter(lower);
        writer.EnableEncryption(RandomKey(new Random(36)));
        WriteAll(writer, new byte[20]);
        var cause = await Assert.ThrowsAsync<IOException>(async () => await writer.FlushAsync(Ct));

        Assert.Same(cause, Assert.Throws<InvalidOperationException>(() => writer.Advance(0)).InnerException);
    }

    [Fact]
    public async Task Poisoned_ByCipherOnAdvanceOrFlush_NeverLeaksPartialCiphertext()
    {
        var key = RandomKey(new Random(33));
        var pipe = NewPipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        var cipher = new ThrowAfterCipher(PacketCipher.CreateEncryptor(key), failAfter: 100);
        writer.EnableEncryption(cipher);
        byte[] good = RandomBytes(new Random(34), 100);
        WriteAll(writer, good);
        await writer.FlushAsync(Ct);

        Exception? failure = null;
        try
        {
            WriteAll(writer, new byte[5000]);
            await writer.FlushAsync(Ct);
        }
        catch (Exception e)
        {
            failure = e;
        }

        Assert.IsType<ThrowAfterCipher.BoomException>(failure);
        Assert.Equal(0, pipe.Writer.UnflushedBytes);

        var collected = new List<byte>();
        while (pipe.Reader.TryRead(out var available))
        {
            collected.AddRange(available.Buffer.ToArray());
            pipe.Reader.AdvanceTo(available.Buffer.End);
        }

        Assert.Equal(ReferenceTransform(key, true, good), collected.ToArray());
        writer.Complete();

        var readFailure = await Assert.ThrowsAsync<ThrowAfterCipher.BoomException>(
            async () => await pipe.Reader.ReadAsync(Ct));
        Assert.Same(failure, readFailure);
        writer.Dispose();
    }

    [Theory]
    [InlineData(-1, 401)]
    [InlineData(128, 402)]
    public async Task PacketPipeline_FaultingCipher_ReaderSeesIntactPrefixThenCause(int compressionThreshold, int seed)
    {
        _output.WriteLine($"seed={seed}");
        var random = new Random(seed);
        var key = RandomKey(random);
        var pipe = NewPipe();
        using var writer = new MinecraftPacketPipeWriter(pipe.Writer);
        using var reader = new MinecraftPacketPipeReader(pipe.Reader);
        writer.CompressionThreshold = compressionThreshold;
        reader.CompressionThreshold = compressionThreshold;
        int failAfter = random.Next(200, 20_000);
        writer.EnableEncryption(new ThrowAfterCipher(PacketCipher.CreateEncryptor(key), failAfter));
        reader.EnableEncryption(key);

        var bodies = new List<byte[]>();
        int flushedPackets = 0;
        Exception? failure = null;
        while (failure is null)
        {
            var body = RandomBytes(random, random.Next(0, 3000));
            try
            {
                writer.WritePacket(random.Next(0, 100), body);
                bodies.Add(body);
                await writer.FlushAsync(Ct);
                flushedPackets = bodies.Count;
            }
            catch (Exception e)
            {
                failure = e;
            }
        }

        Assert.IsType<ThrowAfterCipher.BoomException>(failure);
        Assert.Throws<InvalidOperationException>(() => writer.WritePacket(1, ReadOnlySpan<byte>.Empty));

        for (int index = 0; index < flushedPackets; index++)
        {
            var packet = await reader.ReadPacketAsync(Ct);
            Assert.Equal(bodies[index], packet.Data.ToArray());
        }

        writer.Complete();
        var seen = await Assert.ThrowsAnyAsync<Exception>(async () => await reader.ReadPacketAsync(Ct));
        Assert.True(ReferenceEquals(seen, failure) || ReferenceEquals(seen.InnerException, failure), seen.ToString());
    }

    [Fact]
    public async Task ConcurrentMisuse_DoesNotHang()
    {
        var pipe = NewPipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(RandomKey(new Random(35)));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(TimeSpan.FromSeconds(20));
        var ct = cts.Token;

        var drain = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    var result = await pipe.Reader.ReadAsync(ct);
                    pipe.Reader.AdvanceTo(result.Buffer.End);
                    if (result.IsCompleted)
                    {
                        return;
                    }
                }
            }
            catch
            {
            }
        }, ct);

        var workers = Enumerable.Range(0, 4).Select(worker => Task.Run(async () =>
        {
            var random = new Random(worker);
            try
            {
                for (int i = 0; i < 2000 && !ct.IsCancellationRequested; i++)
                {
                    var span = writer.GetSpan(random.Next(1, 600));
                    int take = random.Next(span.Length + 1);
                    span[..take].Fill((byte)worker);
                    writer.Advance(take);
                    if (random.Next(3) == 0)
                    {
                        await writer.FlushAsync(ct);
                    }
                }
            }
            catch
            {
            }
        }, ct)).ToArray();

        var all = Task.WhenAll(workers);
        bool finished = await Task.WhenAny(all, Task.Delay(TimeSpan.FromSeconds(20), Ct)) == all;
        Assert.True(finished, "concurrent misuse hung the writer");

        try
        {
            writer.Complete();
        }
        catch
        {
        }

        pipe.Reader.Complete();
        await drain;
        try
        {
            writer.Dispose();
        }
        catch
        {
        }
    }

    private sealed class RecordingCipher : PacketCipher
    {
        private readonly PacketCipher _inner;

        public RecordingCipher(PacketCipher inner) => _inner = inner;

        public long TransformedBytes { get; private set; }

        public bool SawEmptyCall { get; private set; }

        public bool Disposed { get; private set; }

        public override void Transform(Span<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                SawEmptyCall = true;
            }

            _inner.Transform(buffer);
            TransformedBytes += buffer.Length;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            _inner.Dispose();
        }
    }

    private sealed class ThrowAfterCipher : PacketCipher
    {
        private readonly PacketCipher _inner;
        private int _remaining;

        public ThrowAfterCipher(PacketCipher inner, int failAfter)
        {
            _inner = inner;
            _remaining = failAfter;
        }

        public override void Transform(Span<byte> buffer)
        {
            if (buffer.Length <= _remaining)
            {
                _inner.Transform(buffer);
                _remaining -= buffer.Length;
                return;
            }

            _inner.Transform(buffer[.._remaining]);
            _remaining = 0;
            throw new BoomException();
        }

        protected override void Dispose(bool disposing) => _inner.Dispose();

        public sealed class BoomException : Exception
        {
            public BoomException() : base("cipher failed")
            {
            }
        }
    }

    private sealed class ScriptedFlushWriter : PipeWriter
    {
        private readonly PipeWriter _inner;

        public ScriptedFlushWriter(PipeWriter inner) => _inner = inner;

        public Exception? ThrowOnNextFlush { get; set; }

        public Exception? ThrowOnNextAdvance { get; set; }

        public FlushResult? NextResult { get; set; }

        public int FlushCalls { get; private set; }

        public override Span<byte> GetSpan(int sizeHint = 0) => _inner.GetSpan(sizeHint);

        public override Memory<byte> GetMemory(int sizeHint = 0) => _inner.GetMemory(sizeHint);

        public override void Advance(int bytes)
        {
            if (ThrowOnNextAdvance is { } advanceError)
            {
                ThrowOnNextAdvance = null;
                throw advanceError;
            }

            _inner.Advance(bytes);
        }

        public override async ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
        {
            FlushCalls++;
            if (ThrowOnNextFlush is { } error)
            {
                ThrowOnNextFlush = null;
                throw error;
            }

            var result = await _inner.FlushAsync(cancellationToken);
            if (NextResult is { } scripted)
            {
                NextResult = null;
                return scripted;
            }

            return result;
        }

        public override void Complete(Exception? exception = null) => _inner.Complete(exception);

        public override void CancelPendingFlush() => _inner.CancelPendingFlush();

        public override bool CanGetUnflushedBytes => _inner.CanGetUnflushedBytes;

        public override long UnflushedBytes => _inner.UnflushedBytes;
    }

    private sealed class NoUnflushedBytesWriter : PipeWriter
    {
        private readonly PipeWriter _inner;

        public NoUnflushedBytesWriter(PipeWriter inner) => _inner = inner;

        public override Span<byte> GetSpan(int sizeHint = 0) => _inner.GetSpan(sizeHint);

        public override Memory<byte> GetMemory(int sizeHint = 0) => _inner.GetMemory(sizeHint);

        public override void Advance(int bytes) => _inner.Advance(bytes);

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
            => _inner.FlushAsync(cancellationToken);

        public override void Complete(Exception? exception = null) => _inner.Complete(exception);

        public override void CancelPendingFlush() => _inner.CancelPendingFlush();
    }
}
