using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Pipelines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

public class CryptoPipeBlackBoxFuzzTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static byte[] RandomKey(Random random)
    {
        var key = new byte[PacketCipher.SharedSecretLength];
        random.NextBytes(key);
        return key;
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

    private static int PickLength(Random random)
    {
        return random.Next(6) switch
        {
            0 => 0,
            1 => 1,
            2 => random.Next(2, 4096),
            3 => random.Next(4096, 70_001),
            4 => random.Next(70_001, 300_000),
            _ => random.Next(300_000, (1 << 20) + 1),
        };
    }

    private static int PickChunk(Random random, int remaining)
    {
        int size = random.Next(4) switch
        {
            0 => random.Next(1, 17),
            1 => random.Next(1, 4097),
            2 => random.Next(1, 20_000),
            _ => random.Next(1, 70_001),
        };
        return Math.Min(size, remaining);
    }

    private static async Task<byte[]> DrainAsync(PipeReader reader, CancellationToken ct)
    {
        var collected = new ArrayBufferWriter<byte>();
        while (true)
        {
            var result = await reader.ReadAsync(ct);
            foreach (var segment in result.Buffer)
            {
                collected.Write(segment.Span);
            }

            reader.AdvanceTo(result.Buffer.End);
            if (result.IsCompleted)
            {
                return collected.WrittenSpan.ToArray();
            }
        }
    }

    private static async Task WriteRandomAsync(
        CryptoPipeWriter writer,
        byte[] plain,
        int prefix,
        byte[] key,
        Random random,
        bool useMemory,
        CancellationToken ct)
    {
        int position = 0;
        bool enabled = false;
        while (true)
        {
            if (!enabled && position >= prefix)
            {
                if (writer.CanGetUnflushedBytes && writer.UnflushedBytes > 0)
                {
                    await writer.FlushAsync(ct);
                }

                writer.EnableEncryption(key);
                enabled = true;
            }

            if (position == plain.Length)
            {
                break;
            }

            int limit = enabled ? plain.Length - position : prefix - position;
            int chunk = PickChunk(random, limit);
            if (useMemory)
            {
                var memory = writer.GetMemory(random.Next(2) == 0 ? chunk : 0);
                int take = Math.Min(chunk, memory.Length);
                plain.AsSpan(position, take).CopyTo(memory.Span);
                writer.Advance(take);
                chunk = take;
            }
            else
            {
                var span = writer.GetSpan(chunk);
                Assert.True(span.Length >= chunk);
                plain.AsSpan(position, chunk).CopyTo(span);
                writer.Advance(chunk);
            }

            position += chunk;
            if (random.Next(3) == 0)
            {
                await writer.FlushAsync(ct);
            }
        }

        await writer.FlushAsync(ct);
        writer.Complete();
    }

    private static async Task<byte[]> ReadRandomAsync(
        CryptoPipeReader reader,
        int prefix,
        byte[] key,
        Random random,
        CancellationToken ct)
    {
        var collected = new ArrayBufferWriter<byte>();
        long consumedTotal = 0;
        bool enabled = false;
        while (true)
        {
            if (!enabled && consumedTotal >= prefix)
            {
                reader.EnableEncryption(key);
                enabled = true;
            }

            ReadResult result;
            if (random.Next(4) == 0 && reader.TryRead(out var tried))
            {
                result = tried;
            }
            else
            {
                result = await reader.ReadAsync(ct);
            }

            var buffer = result.Buffer;
            long available = buffer.Length;
            long allowed = enabled ? available : Math.Min(available, prefix - consumedTotal);
            long take = random.Next(5) switch
            {
                0 => 0,
                1 => allowed,
                _ => (long)(random.NextDouble() * allowed),
            };

            if (result.IsCompleted && take < allowed)
            {
                take = allowed;
            }

            var consumedSlice = buffer.Slice(0, take);
            foreach (var segment in consumedSlice)
            {
                collected.Write(segment.Span);
            }

            var consumed = consumedSlice.End;
            bool stoppedAtBoundary = !enabled && consumedTotal + take == prefix;
            var examined = stoppedAtBoundary && random.Next(2) == 0 ? consumed : buffer.End;
            reader.AdvanceTo(consumed, examined);
            consumedTotal += take;

            if (result.IsCompleted && take == available)
            {
                break;
            }
        }

        reader.Complete();
        return collected.WrittenSpan.ToArray();
    }

    public static IEnumerable<object[]> Seeds()
    {
        for (int seed = 1; seed <= 40; seed++)
        {
            yield return [seed];
        }
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task Writer_ProducesReferenceCiphertext_ForRandomChunksAndFlushes(int seed)
    {
        var random = new Random(seed);
        var key = RandomKey(random);
        var plain = new byte[PickLength(random)];
        random.NextBytes(plain);
        int prefix = random.Next(3) == 0 ? plain.Length : random.Next(plain.Length + 1);
        bool useMemory = random.Next(2) == 0;

        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        using var writer = new CryptoPipeWriter(pipe.Writer);

        var drain = Task.Run(() => DrainAsync(pipe.Reader, cts.Token), cts.Token);
        await WriteRandomAsync(writer, plain, prefix, key, random, useMemory, cts.Token);
        byte[] wire = await drain;

        Assert.Equal(Wire(key, plain, prefix), wire);
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task Reader_DecryptsReferenceCiphertext_ForRandomChunksAndAdvances(int seed)
    {
        var random = new Random(seed + 1000);
        var key = RandomKey(random);
        var plain = new byte[PickLength(random)];
        random.NextBytes(plain);
        int prefix = random.Next(3) == 0 ? plain.Length : random.Next(plain.Length + 1);
        byte[] wire = Wire(key, plain, prefix);

        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        using var reader = new CryptoPipeReader(pipe.Reader);

        var feed = Task.Run(async () =>
        {
            var feeder = new Random(seed + 2000);
            int position = 0;
            while (position < wire.Length)
            {
                int chunk = PickChunk(feeder, wire.Length - position);
                await pipe.Writer.WriteAsync(wire.AsMemory(position, chunk), cts.Token);
                position += chunk;
            }

            pipe.Writer.Complete();
        }, cts.Token);

        byte[] output = await ReadRandomAsync(reader, prefix, key, random, cts.Token);
        await feed;

        Assert.Equal(plain, output);
    }

    [Theory]
    [MemberData(nameof(Seeds))]
    public async Task WriterToReader_RoundTrips_WithRandomChunksOnBothSides(int seed)
    {
        var random = new Random(seed + 3000);
        var key = RandomKey(random);
        var plain = new byte[PickLength(random)];
        random.NextBytes(plain);
        int prefix = random.Next(3) == 0 ? plain.Length : random.Next(plain.Length + 1);
        bool useMemory = random.Next(2) == 0;

        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        using var writer = new CryptoPipeWriter(pipe.Writer);
        using var reader = new CryptoPipeReader(pipe.Reader);

        var readerRandom = new Random(seed + 4000);
        var read = Task.Run(() => ReadRandomAsync(reader, prefix, key, readerRandom, cts.Token), cts.Token);
        await WriteRandomAsync(writer, plain, prefix, key, random, useMemory, cts.Token);
        byte[] output = await read;

        Assert.Equal(plain, output);
    }

    [Fact]
    public async Task Reader_ConsumedLessThanExamined_WaitsForNewInput_ThenDeliversEverything()
    {
        var random = new Random(7);
        var key = RandomKey(random);
        var plain = new byte[300];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);
        var encryptor = Reference(key, true);

        int written = 0;
        for (int round = 0; round < 5; round++)
        {
            int chunk = 60;
            var cipherChunk = new byte[chunk];
            Assert.Equal(chunk, encryptor.ProcessBytes(plain, written, chunk, cipherChunk, 0));
            await pipe.Writer.WriteAsync(cipherChunk, Ct);
            written += chunk;

            var result = await reader.ReadAsync(Ct);
            Assert.Equal(written, result.Buffer.Length);
            Assert.Equal(plain.AsSpan(0, written).ToArray(), result.Buffer.ToArray());
            reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);

            var pending = reader.ReadAsync(Ct);
            Assert.False(pending.IsCompleted);
            reader.CancelPendingRead();
            var canceled = await pending;
            Assert.True(canceled.IsCanceled);
            Assert.Equal(written, canceled.Buffer.Length);
            reader.AdvanceTo(canceled.Buffer.Start, canceled.Buffer.End);
        }

        pipe.Writer.Complete();
        var final = await reader.ReadAsync(Ct);
        Assert.True(final.IsCompleted);
        Assert.Equal(plain, final.Buffer.ToArray());
        reader.AdvanceTo(final.Buffer.End);
        Assert.Equal(300, written);
    }

    [Theory]
    [InlineData(false, 1234)]
    [InlineData(true, 1234)]
    [InlineData(false, 1)]
    [InlineData(true, 1)]
    [InlineData(true, 4096)]
    [InlineData(true, 4097)]
    public async Task Reader_PartialConsume_AfterInnerCompleted_DeliversEverything(bool encrypted, int step)
    {
        var random = new Random(8 + step);
        var key = RandomKey(random);
        var plain = new byte[step == 1 ? 3000 : 70_001];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        if (encrypted)
        {
            reader.EnableEncryption(key);
        }

        await pipe.Writer.WriteAsync(encrypted ? ReferenceTransform(key, true, plain) : plain, Ct);
        pipe.Writer.Complete();

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        var collected = new ArrayBufferWriter<byte>();
        while (true)
        {
            var result = await reader.ReadAsync(cts.Token);
            long take = Math.Min(step, result.Buffer.Length);
            var slice = result.Buffer.Slice(0, take);
            foreach (var segment in slice)
            {
                collected.Write(segment.Span);
            }

            reader.AdvanceTo(slice.End, result.Buffer.End);
            if (result.IsCompleted && take == result.Buffer.Length)
            {
                break;
            }
        }

        Assert.Equal(plain, collected.WrittenSpan.ToArray());
    }

    [Fact]
    public async Task Reader_PartialConsume_GrowingSteps_KeepsRemainderDecryptedOnce()
    {
        var random = new Random(8);
        var key = RandomKey(random);
        var plain = new byte[10_000];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);
        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, plain), Ct);
        pipe.Writer.Complete();

        var collected = new ArrayBufferWriter<byte>();
        int step = 1;
        while (true)
        {
            var result = await reader.ReadAsync(Ct);
            long take = Math.Min(step, result.Buffer.Length);
            var slice = result.Buffer.Slice(0, take);
            foreach (var segment in slice)
            {
                collected.Write(segment.Span);
            }

            reader.AdvanceTo(slice.End, result.Buffer.End);
            step = step * 2 + 1;
            if (result.IsCompleted && take == result.Buffer.Length)
            {
                break;
            }
        }

        Assert.Equal(plain, collected.WrittenSpan.ToArray());
    }

    [Fact]
    public async Task Reader_TryRead_ReturnsFalseWithoutNewData_TrueAfterWrite()
    {
        var random = new Random(9);
        var key = RandomKey(random);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        Assert.False(reader.TryRead(out _));

        var encryptor = Reference(key, true);
        var first = new byte[5];
        Assert.Equal(5, encryptor.ProcessBytes("hello"u8.ToArray(), 0, 5, first, 0));
        await pipe.Writer.WriteAsync(first, Ct);

        Assert.True(reader.TryRead(out var result));
        Assert.Equal("hello"u8.ToArray(), result.Buffer.ToArray());
        reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);

        Assert.False(reader.TryRead(out _));

        var second = new byte[6];
        Assert.Equal(6, encryptor.ProcessBytes(" world"u8.ToArray(), 0, 6, second, 0));
        await pipe.Writer.WriteAsync(second, Ct);

        Assert.True(reader.TryRead(out result));
        Assert.Equal("hello world"u8.ToArray(), result.Buffer.ToArray());
        reader.AdvanceTo(result.Buffer.End);
    }

    [Fact]
    public async Task Reader_CancelPendingRead_BeforeRead_CancelsNextRead()
    {
        var random = new Random(10);
        var key = RandomKey(random);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        reader.CancelPendingRead();
        var result = await reader.ReadAsync(Ct);
        Assert.True(result.IsCanceled);
        Assert.Equal(0, result.Buffer.Length);
        reader.AdvanceTo(result.Buffer.End);

        var pending = reader.ReadAsync(Ct);
        Assert.False(pending.IsCompleted);
        reader.CancelPendingRead();
        result = await pending;
        Assert.True(result.IsCanceled);
        reader.AdvanceTo(result.Buffer.End);

        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, "abc"u8), Ct);
        result = await reader.ReadAsync(Ct);
        Assert.False(result.IsCanceled);
        Assert.Equal("abc"u8.ToArray(), result.Buffer.ToArray());
        reader.AdvanceTo(result.Buffer.End);
    }

    [Fact]
    public async Task Reader_InnerCompletion_DeliversAllBytes_ThenIsCompleted()
    {
        var random = new Random(11);
        var key = RandomKey(random);
        var plain = new byte[70_000];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);

        var read = Task.Run(() => DrainAsync(reader, Ct), Ct);
        byte[] wire = ReferenceTransform(key, true, plain);
        for (int i = 0; i < wire.Length; i += 1234)
        {
            await pipe.Writer.WriteAsync(wire.AsMemory(i, Math.Min(1234, wire.Length - i)), Ct);
        }

        pipe.Writer.Complete();
        Assert.Equal(plain, await read);

        var after = await reader.ReadAsync(Ct);
        Assert.True(after.IsCompleted);
        Assert.Equal(0, after.Buffer.Length);
        reader.AdvanceTo(after.Buffer.End);
    }

    [Fact]
    public async Task Reader_InnerCompletionWithException_Propagates()
    {
        var random = new Random(12);
        var key = RandomKey(random);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(key);
        await pipe.Writer.WriteAsync(ReferenceTransform(key, true, "xyz"u8), Ct);
        pipe.Writer.Complete(new IOException("boom"));

        var thrown = await Assert.ThrowsAsync<IOException>(async () => await reader.ReadAsync(Ct));
        Assert.Equal("boom", thrown.Message);
    }

    [Fact]
    public async Task Reader_EnableEncryption_AfterPlaintextPrefixConsumed_DecryptsBufferedTail()
    {
        var random = new Random(13);
        var key = RandomKey(random);
        var plain = new byte[5000];
        random.NextBytes(plain);
        int prefix = 1234;
        byte[] wire = Wire(key, plain, prefix);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var reader = new CryptoPipeReader(pipe.Reader);
        await pipe.Writer.WriteAsync(wire, Ct);
        pipe.Writer.Complete();

        var result = await reader.ReadAsync(Ct);
        Assert.Equal(wire.Length, result.Buffer.Length);
        Assert.Equal(plain.AsSpan(0, prefix).ToArray(), result.Buffer.Slice(0, prefix).ToArray());
        reader.AdvanceTo(result.Buffer.GetPosition(prefix), result.Buffer.End);

        reader.EnableEncryption(key);
        Assert.True(reader.EncryptionEnabled);

        Assert.Equal(plain.AsSpan(prefix).ToArray(), await DrainAsync(reader, Ct));
    }

    [Fact]
    public async Task Reader_AfterDispose_ReadAsync_Throws()
    {
        var pipe = new Pipe();
        var reader = new CryptoPipeReader(pipe.Reader);
        reader.Dispose();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(TimeSpan.FromSeconds(10));
        await Assert.ThrowsAnyAsync<InvalidOperationException>(async () => await reader.ReadAsync(cts.Token));
    }

    [Fact]
    public async Task Writer_FlushAsync_HonoursBackpressure()
    {
        var random = new Random(14);
        var key = RandomKey(random);
        var plain = new byte[4096];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 64, resumeWriterThreshold: 32, useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write(plain);
        var flush = writer.FlushAsync(Ct);
        Assert.False(flush.IsCompleted);

        var read = Task.Run(() => DrainAsync(pipe.Reader, Ct), Ct);
        var result = await flush;
        Assert.False(result.IsCanceled);
        Assert.False(result.IsCompleted);
        writer.Complete();

        Assert.Equal(ReferenceTransform(key, true, plain), await read);
    }

    [Fact]
    public async Task Writer_CancelPendingFlush_ReportsCanceled_DataStaysIntact()
    {
        var random = new Random(15);
        var key = RandomKey(random);
        var plain = new byte[2048];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 64, resumeWriterThreshold: 32, useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write(plain.AsSpan(0, 1024));
        var flush = writer.FlushAsync(Ct);
        Assert.False(flush.IsCompleted);
        writer.CancelPendingFlush();
        var canceled = await flush;
        Assert.True(canceled.IsCanceled);

        writer.Write(plain.AsSpan(1024));
        var second = writer.FlushAsync(Ct);
        var read = Task.Run(() => DrainAsync(pipe.Reader, Ct), Ct);
        var result = await second;
        Assert.False(result.IsCanceled);
        writer.Complete();

        Assert.Equal(ReferenceTransform(key, true, plain), await read);
    }

    [Fact]
    public async Task Writer_CompleteWithException_FaultsReader()
    {
        var random = new Random(16);
        var key = RandomKey(random);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        writer.Write("never flushed"u8);
        writer.Complete(new InvalidDataException("dropped"));

        var thrown = await Assert.ThrowsAsync<InvalidDataException>(async () => await pipe.Reader.ReadAsync(Ct));
        Assert.Equal("dropped", thrown.Message);
    }

    [Fact]
    public async Task Writer_Complete_DeliversUnflushedBytes_AndCompletesReader()
    {
        var random = new Random(17);
        var key = RandomKey(random);
        var plain = new byte[9000];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.Write(plain.AsSpan(0, 1000));
        await writer.FlushAsync(Ct);
        writer.EnableEncryption(key);
        writer.Write(plain.AsSpan(1000));
        writer.Complete();

        Assert.Equal(Wire(key, plain, 1000), await DrainAsync(pipe.Reader, Ct));
    }

    [Fact]
    public async Task Writer_CompleteAsync_DeliversUnflushedBytes()
    {
        var random = new Random(18);
        var key = RandomKey(random);
        var plain = new byte[3000];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);
        writer.Write(plain);
        await writer.CompleteAsync();

        Assert.Equal(ReferenceTransform(key, true, plain), await DrainAsync(pipe.Reader, Ct));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Writer_UnflushedBytes_TracksAdvancesAndFlushes(bool encrypted)
    {
        var random = new Random(19);
        var key = RandomKey(random);
        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        if (encrypted)
        {
            writer.EnableEncryption(key);
        }

        if (!writer.CanGetUnflushedBytes)
        {
            return;
        }

        Assert.Equal(0, writer.UnflushedBytes);
        writer.GetSpan(10);
        writer.Advance(10);
        Assert.Equal(10, writer.UnflushedBytes);
        writer.Write(new byte[5000]);
        Assert.Equal(5010, writer.UnflushedBytes);
        writer.GetMemory(1);
        writer.Advance(1);
        Assert.Equal(5011, writer.UnflushedBytes);
        await writer.FlushAsync(Ct);
        Assert.Equal(0, writer.UnflushedBytes);

        var result = await pipe.Reader.ReadAsync(Ct);
        Assert.Equal(5011, result.Buffer.Length);
        pipe.Reader.AdvanceTo(result.Buffer.End);
    }

    [Fact]
    public async Task Writer_ManySmallWritesAcrossFlushes_KeepCipherStreamContinuous()
    {
        var random = new Random(20);
        var key = RandomKey(random);
        var plain = new byte[20_000];
        random.NextBytes(plain);
        var pipe = new Pipe(new PipeOptions(pauseWriterThreshold: 0, useSynchronizationContext: false));
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(key);

        for (int i = 0; i < plain.Length; i++)
        {
            writer.GetSpan(1)[0] = plain[i];
            writer.Advance(1);
            if (i % 997 == 0)
            {
                await writer.FlushAsync(Ct);
            }
        }

        writer.Complete();
        Assert.Equal(ReferenceTransform(key, true, plain), await DrainAsync(pipe.Reader, Ct));
    }

    [Fact]
    public void Writer_EnableEncryption_Twice_Throws()
    {
        var random = new Random(21);
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(RandomKey(random));
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(RandomKey(random)));
    }

    [Fact]
    public void Writer_EnableEncryption_AfterDispose_Throws()
    {
        var random = new Random(22);
        var pipe = new Pipe();
        var writer = new CryptoPipeWriter(pipe.Writer);
        writer.Dispose();
        Assert.ThrowsAny<InvalidOperationException>(() => writer.EnableEncryption(RandomKey(random)));
    }

    [Fact]
    public void Writer_EnableEncryption_WithUnflushedPlaintext_Throws()
    {
        var random = new Random(23);
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.Write("plain"u8);
        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(RandomKey(random)));
    }

    [Fact]
    public void Writer_EnableEncryption_WrongKeyLength_Throws()
    {
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        Assert.ThrowsAny<ArgumentException>(() => writer.EnableEncryption(new byte[15]));
        Assert.False(writer.EncryptionEnabled);
    }

    [Fact]
    public void Writer_Advance_Misuse_Throws()
    {
        var random = new Random(24);
        var pipe = new Pipe();
        using var writer = new CryptoPipeWriter(pipe.Writer);
        writer.EnableEncryption(RandomKey(random));

        int room = writer.GetSpan(16).Length;
        Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(room + 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(-1));
        writer.Advance(16);
    }

    [Fact]
    public void Reader_EnableEncryption_Twice_Throws()
    {
        var random = new Random(25);
        var pipe = new Pipe();
        using var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(RandomKey(random));
        Assert.Throws<InvalidOperationException>(() => reader.EnableEncryption(RandomKey(random)));
    }

    [Fact]
    public void Reader_EnableEncryption_AfterDispose_Throws()
    {
        var random = new Random(26);
        var pipe = new Pipe();
        var reader = new CryptoPipeReader(pipe.Reader);
        reader.Dispose();
        Assert.ThrowsAny<InvalidOperationException>(() => reader.EnableEncryption(RandomKey(random)));
    }

    [Fact]
    public async Task Reader_EnableEncryption_WithOutstandingRead_Throws()
    {
        var random = new Random(27);
        var pipe = new Pipe();
        using var reader = new CryptoPipeReader(pipe.Reader);
        await pipe.Writer.WriteAsync("plain"u8.ToArray(), Ct);
        var result = await reader.ReadAsync(Ct);
        Assert.Throws<InvalidOperationException>(() => reader.EnableEncryption(RandomKey(random)));
        reader.AdvanceTo(result.Buffer.End);
        reader.EnableEncryption(RandomKey(random));
        Assert.True(reader.EncryptionEnabled);
    }

    [Fact]
    public void Reader_EnableEncryption_WrongKeyLength_Throws()
    {
        var pipe = new Pipe();
        using var reader = new CryptoPipeReader(pipe.Reader);
        Assert.ThrowsAny<ArgumentException>(() => reader.EnableEncryption(new byte[3]));
        Assert.False(reader.EncryptionEnabled);
    }

    public static IEnumerable<object[]> PacketScenarios()
    {
        foreach (int threshold in new[] { -1, 0, 64 })
        {
            foreach (int switchAt in new[] { -1, 0, 3 })
            {
                yield return [threshold, switchAt, 1];
                yield return [threshold, switchAt, 2];
            }
        }
    }

    [Theory]
    [MemberData(nameof(PacketScenarios))]
    public async Task PacketPipeline_RoundTrips_WithFragmentedWire(int compressionThreshold, int switchAt, int seed)
    {
        var random = new Random(seed * 100 + compressionThreshold + switchAt);
        var key = RandomKey(random);
        const int count = 40;
        var ids = new int[count];
        var bodies = new byte[count][];
        for (int i = 0; i < count; i++)
        {
            ids[i] = random.Next(0, 128);
            bodies[i] = new byte[random.Next(5) switch
            {
                0 => 0,
                1 => random.Next(1, 32),
                2 => random.Next(32, 400),
                3 => random.Next(400, 5000),
                _ => random.Next(5000, 40_000),
            }];
            if (random.Next(2) == 0)
            {
                random.NextBytes(bodies[i]);
            }
            else
            {
                Array.Fill(bodies[i], (byte)random.Next(256));
            }
        }

        var wirePipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        var readerPipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        using var writer = new MinecraftPacketPipeWriter(wirePipe.Writer) { CompressionThreshold = compressionThreshold };
        using var reader = new MinecraftPacketPipeReader(readerPipe.Reader) { CompressionThreshold = compressionThreshold };

        var pump = Task.Run(async () =>
        {
            var fragments = new Random(seed + 500);
            while (true)
            {
                var result = await wirePipe.Reader.ReadAsync(cts.Token);
                var buffer = result.Buffer;
                while (buffer.Length > 0)
                {
                    long take = Math.Min(buffer.Length, fragments.Next(1, 700));
                    foreach (var segment in buffer.Slice(0, take))
                    {
                        await readerPipe.Writer.WriteAsync(segment, cts.Token);
                    }

                    buffer = buffer.Slice(take);
                }

                wirePipe.Reader.AdvanceTo(buffer.End);
                if (result.IsCompleted)
                {
                    break;
                }
            }

            readerPipe.Writer.Complete();
        }, cts.Token);

        var producer = Task.Run(async () =>
        {
            for (int i = 0; i < count; i++)
            {
                if (i == switchAt)
                {
                    await writer.FlushAsync(cts.Token);
                    writer.EnableEncryption(key);
                }

                writer.WritePacket(ids[i], bodies[i]);
                if (random.Next(3) == 0)
                {
                    await writer.FlushAsync(cts.Token);
                }
            }

            await writer.FlushAsync(cts.Token);
            writer.Complete();
        }, cts.Token);

        for (int i = 0; i < count; i++)
        {
            if (i == switchAt)
            {
                reader.EnableEncryption(key);
            }

            var packet = await reader.ReadPacketAsync(cts.Token);
            Assert.Equal(ids[i], packet.Id);
            Assert.Equal(bodies[i], packet.Data.ToArray());
        }

        await producer;
        await pump;
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await reader.ReadPacketAsync(cts.Token));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(128)]
    public async Task PacketPipeline_Enumeration_SwitchesEncryptionMidStream(int compressionThreshold)
    {
        var random = new Random(31 + compressionThreshold);
        var key = RandomKey(random);
        var pipe = new Pipe(new PipeOptions(useSynchronizationContext: false));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        cts.CancelAfter(Timeout);
        using var writer = new MinecraftPacketPipeWriter(pipe.Writer) { CompressionThreshold = compressionThreshold };
        using var reader = new MinecraftPacketPipeReader(pipe.Reader) { CompressionThreshold = compressionThreshold };

        var bodies = new byte[30][];
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i] = new byte[random.Next(0, 3000)];
            random.NextBytes(bodies[i]);
        }

        for (int i = 0; i < 5; i++)
        {
            writer.WritePacket(i, bodies[i]);
        }

        await writer.FlushAsync(cts.Token);

        int received = 0;
        await foreach (var packet in reader.ReadPacketsAsync(cts.Token))
        {
            Assert.Equal(received, packet.Id);
            Assert.Equal(bodies[received], packet.Data.ToArray());
            received++;

            if (received == 5)
            {
                reader.EnableEncryption(key);
                writer.EnableEncryption(key);
                for (int i = 5; i < bodies.Length; i++)
                {
                    writer.WritePacket(i, bodies[i]);
                    if (i % 4 == 0)
                    {
                        await writer.FlushAsync(cts.Token);
                    }
                }

                await writer.FlushAsync(cts.Token);
                writer.Complete();
            }
        }

        Assert.Equal(bodies.Length, received);
    }
}
