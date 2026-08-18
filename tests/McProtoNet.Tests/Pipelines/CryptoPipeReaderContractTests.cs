using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Net;
using McProtoNet.Tests.Infrastructure;

namespace McProtoNet.Tests.Pipelines;

public class CryptoPipeReaderContractTests
{
    private static byte[] Encrypt(byte[] plain)
    {
        byte[] encrypted = (byte[])plain.Clone();
        using var encryptor = Crypto.CreateEncryptor();
        encryptor.Transform(encrypted);
        return encrypted;
    }

    private static byte[] RandomBytes(int length, int seed)
    {
        byte[] bytes = new byte[length];
        new Random(seed).NextBytes(bytes);
        return bytes;
    }

    private static (Pipe pipe, CryptoPipeReader reader) CreateEncryptedReader()
    {
        var pipe = new Pipe();
        var reader = new CryptoPipeReader(pipe.Reader);
        reader.EnableEncryption(Crypto.CreateDecryptor());
        return (pipe, reader);
    }

    private static CancellationToken Timeout()
    {
        return new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token;
    }

    [Theory]
    [InlineData(1, 8, 11)]
    [InlineData(1, 3000, 12)]
    [InlineData(4096, 65536, 13)]
    public async Task ReadAsync_ShouldDeliverPlaintextInOrder_WhenConsumerAdvancesRandomly(
        int minChunk, int maxChunk, int seed)
    {
        byte[] plain = RandomBytes(400_000, seed);
        byte[] encrypted = Encrypt(plain);
        var (pipe, reader) = CreateEncryptedReader();

        var feeder = Task.Run(async () =>
        {
            var random = new Random(seed + 100);
            int position = 0;
            while (position < encrypted.Length)
            {
                int length = Math.Min(random.Next(minChunk, maxChunk + 1), encrypted.Length - position);
                await pipe.Writer.WriteAsync(encrypted.AsMemory(position, length));
                position += length;
            }

            await pipe.Writer.CompleteAsync();
        });

        var consumerRandom = new Random(seed + 200);
        using var collected = new MemoryStream();
        CancellationToken timeout = Timeout();
        while (true)
        {
            ReadResult result = await reader.ReadAsync(timeout);
            ReadOnlySequence<byte> buffer = result.Buffer;

            long take = consumerRandom.Next(4) == 0
                ? 0
                : (long)(buffer.Length * consumerRandom.NextDouble());
            if (result.IsCompleted)
            {
                take = buffer.Length;
            }

            foreach (ReadOnlyMemory<byte> segment in buffer.Slice(0, take))
            {
                collected.Write(segment.Span);
            }

            reader.AdvanceTo(buffer.GetPosition(take), buffer.End);
            if (result.IsCompleted)
            {
                break;
            }
        }

        await feeder;
        Assert.Equal(plain, collected.ToArray());
    }

    [Fact]
    public async Task ReadAsync_ShouldReturnImmediately_WhenExaminedIsBeforeEnd()
    {
        byte[] plain = RandomBytes(300, 21);
        var (pipe, reader) = CreateEncryptedReader();
        await pipe.Writer.WriteAsync(Encrypt(plain));

        ReadResult first = await reader.ReadAsync(Timeout());
        Assert.Equal(300, first.Buffer.Length);
        SequencePosition middle = first.Buffer.GetPosition(100);
        reader.AdvanceTo(middle, middle);

        ValueTask<ReadResult> second = reader.ReadAsync(Timeout());
        Assert.True(second.IsCompleted);
        ReadResult result = await second;
        Assert.Equal(plain.AsSpan(100).ToArray(), result.Buffer.ToArray());
        reader.AdvanceTo(result.Buffer.End);
    }

    [Fact]
    public async Task ReadAsync_ShouldWaitAndKeepLeftover_WhenEverythingIsExamined()
    {
        byte[] plain = RandomBytes(500, 22);
        byte[] encrypted = Encrypt(plain);
        var (pipe, reader) = CreateEncryptedReader();
        await pipe.Writer.WriteAsync(encrypted.AsMemory(0, 300));

        ReadResult first = await reader.ReadAsync(Timeout());
        Assert.Equal(300, first.Buffer.Length);
        reader.AdvanceTo(first.Buffer.GetPosition(120), first.Buffer.End);

        ValueTask<ReadResult> pending = reader.ReadAsync(Timeout());
        Assert.False(pending.IsCompleted);

        await pipe.Writer.WriteAsync(encrypted.AsMemory(300));
        ReadResult second = await pending;

        Assert.Equal(plain.AsSpan(120).ToArray(), second.Buffer.ToArray());
        reader.AdvanceTo(second.Buffer.End);
    }

    [Fact]
    public async Task ReadAsync_ShouldGrowBuffer_WhenLeftoverExceedsBlockSize()
    {
        const int bigLength = 300_000;
        byte[] plain = RandomBytes(bigLength + 10, 23);
        byte[] encrypted = Encrypt(plain);
        var (pipe, reader) = CreateEncryptedReader();

        var feeder = Task.Run(async () =>
        {
            for (int position = 0; position < bigLength; position += 1000)
            {
                await pipe.Writer.WriteAsync(encrypted.AsMemory(position, 1000));
            }
        });

        CancellationToken timeout = Timeout();
        while (true)
        {
            ReadResult result = await reader.ReadAsync(timeout);
            if (result.Buffer.Length >= bigLength)
            {
                Assert.Equal(plain.AsSpan(0, bigLength).ToArray(), result.Buffer.ToArray());
                reader.AdvanceTo(result.Buffer.End);
                break;
            }

            reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
        }

        await feeder;

        await pipe.Writer.WriteAsync(encrypted.AsMemory(bigLength));
        ReadResult after = await reader.ReadAsync(timeout);
        Assert.Equal(plain.AsSpan(bigLength).ToArray(), after.Buffer.ToArray());
        reader.AdvanceTo(after.Buffer.End);
    }

    [Fact]
    public async Task CancelPendingRead_ShouldCancelPendingEncryptedRead_AndKeepReaderUsable()
    {
        var (pipe, reader) = CreateEncryptedReader();

        ValueTask<ReadResult> pending = reader.ReadAsync(Timeout());
        Assert.False(pending.IsCompleted);

        reader.CancelPendingRead();
        ReadResult canceled = await pending;
        Assert.True(canceled.IsCanceled);
        Assert.False(canceled.IsCompleted);
        Assert.True(canceled.Buffer.IsEmpty);
        reader.AdvanceTo(canceled.Buffer.End);

        byte[] plain = RandomBytes(64, 31);
        await pipe.Writer.WriteAsync(Encrypt(plain));
        ReadResult next = await reader.ReadAsync(Timeout());
        Assert.False(next.IsCanceled);
        Assert.Equal(plain, next.Buffer.ToArray());
        reader.AdvanceTo(next.Buffer.End);
    }

    [Fact]
    public async Task CancelPendingRead_ShouldNeverLoseCancellation_WhenRacingWithReads()
    {
        var (pipe, reader) = CreateEncryptedReader();
        byte[] plain = RandomBytes(2000, 33);
        byte[] encrypted = Encrypt(plain);
        CancellationToken timeout = Timeout();

        int delivered = 0;
        for (int round = 0; round < 500; round++)
        {
            ValueTask<ReadResult> pending = reader.ReadAsync(timeout);
            Task cancel = Task.Run(reader.CancelPendingRead);
            Task feed = round % 2 == 0 && delivered < plain.Length
                ? Task.Run(() => pipe.Writer.WriteAsync(encrypted.AsMemory(delivered, 4)).AsTask())
                : Task.CompletedTask;

            ReadResult result = await pending;
            await cancel;
            await feed;

            reader.AdvanceTo(result.Buffer.Start, result.Buffer.Start);
            if (!result.IsCanceled)
            {
                ReadResult canceled = await reader.ReadAsync(timeout);
                Assert.True(canceled.IsCanceled);
                reader.AdvanceTo(canceled.Buffer.Start, canceled.Buffer.Start);
            }

            if (feed != Task.CompletedTask)
            {
                delivered += 4;
            }
        }

        while (true)
        {
            ReadResult result = await reader.ReadAsync(timeout);
            if (result.IsCanceled)
            {
                reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
                continue;
            }

            if (result.Buffer.Length >= delivered)
            {
                Assert.Equal(plain.AsSpan(0, delivered).ToArray(), result.Buffer.ToArray());
                reader.AdvanceTo(result.Buffer.End);
                break;
            }

            reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
        }
    }

    [Fact]
    public async Task CancelPendingRead_ShouldCancelNextRead_WhenIdle()
    {
        var (pipe, reader) = CreateEncryptedReader();
        byte[] plain = RandomBytes(32, 32);
        await pipe.Writer.WriteAsync(Encrypt(plain));

        reader.CancelPendingRead();

        ReadResult canceled = await reader.ReadAsync(Timeout());
        Assert.True(canceled.IsCanceled);
        reader.AdvanceTo(canceled.Buffer.Start, canceled.Buffer.Start);

        ReadResult next = await reader.ReadAsync(Timeout());
        Assert.False(next.IsCanceled);
        Assert.Equal(plain, next.Buffer.ToArray());
        reader.AdvanceTo(next.Buffer.End);
    }

    [Fact]
    public async Task ReadAsync_ShouldThrowOperationCanceled_AndKeepReaderUsable()
    {
        var (pipe, reader) = CreateEncryptedReader();
        using var cts = new CancellationTokenSource();

        ValueTask<ReadResult> pending = reader.ReadAsync(cts.Token);
        cts.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await pending);

        byte[] plain = RandomBytes(16, 41);
        await pipe.Writer.WriteAsync(Encrypt(plain));
        ReadResult next = await reader.ReadAsync(Timeout());
        Assert.Equal(plain, next.Buffer.ToArray());
        reader.AdvanceTo(next.Buffer.End);
    }

    [Fact]
    public async Task ReadAsync_ShouldThrow_WhenPreviousReadWasNotAdvanced()
    {
        var (pipe, reader) = CreateEncryptedReader();
        await pipe.Writer.WriteAsync(Encrypt(RandomBytes(8, 51)));

        ReadResult result = await reader.ReadAsync(Timeout());

        Assert.Throws<InvalidOperationException>(() => reader.ReadAsync(Timeout()));
        Assert.Throws<InvalidOperationException>(() => reader.TryRead(out _));

        reader.AdvanceTo(result.Buffer.End);
    }

    [Fact]
    public void AdvanceTo_ShouldThrow_WithoutOutstandingRead()
    {
        var (_, reader) = CreateEncryptedReader();
        Assert.Throws<InvalidOperationException>(() => reader.AdvanceTo(default));
    }

    [Fact]
    public async Task AdvanceTo_ShouldThrow_WhenExaminedIsBeforeConsumed()
    {
        var (pipe, reader) = CreateEncryptedReader();
        await pipe.Writer.WriteAsync(Encrypt(RandomBytes(50, 61)));
        ReadResult result = await reader.ReadAsync(Timeout());

        Assert.Throws<InvalidOperationException>(
            () => reader.AdvanceTo(result.Buffer.GetPosition(20), result.Buffer.GetPosition(10)));

        reader.AdvanceTo(result.Buffer.End);
    }

    [Fact]
    public async Task TryRead_ShouldThrow_WhileReadIsPending()
    {
        var (_, reader) = CreateEncryptedReader();
        ValueTask<ReadResult> pending = reader.ReadAsync(Timeout());

        Assert.Throws<InvalidOperationException>(() => reader.TryRead(out _));
        Assert.Throws<InvalidOperationException>(() => reader.Complete());

        reader.CancelPendingRead();
        ReadResult canceled = await pending;
        Assert.True(canceled.IsCanceled);
        reader.AdvanceTo(canceled.Buffer.End);
    }

    [Fact]
    public async Task TryRead_ShouldDecryptBufferedData()
    {
        var (pipe, reader) = CreateEncryptedReader();
        byte[] plain = RandomBytes(40, 71);
        await pipe.Writer.WriteAsync(Encrypt(plain));

        Assert.True(reader.TryRead(out ReadResult result));
        Assert.Equal(plain, result.Buffer.ToArray());
        reader.AdvanceTo(result.Buffer.End);

        Assert.False(reader.TryRead(out _));
    }

    [Fact]
    public async Task ReadAsync_ShouldReportCompletion_UntilLeftoverIsConsumed()
    {
        var (pipe, reader) = CreateEncryptedReader();
        byte[] plain = RandomBytes(30, 81);
        await pipe.Writer.WriteAsync(Encrypt(plain));
        await pipe.Writer.CompleteAsync();

        ReadResult first = await reader.ReadAsync(Timeout());
        Assert.True(first.IsCompleted);
        reader.AdvanceTo(first.Buffer.GetPosition(10), first.Buffer.End);

        ReadResult second = await reader.ReadAsync(Timeout());
        Assert.True(second.IsCompleted);
        Assert.Equal(plain.AsSpan(10).ToArray(), second.Buffer.ToArray());
        reader.AdvanceTo(second.Buffer.End);

        ReadResult third = await reader.ReadAsync(Timeout());
        Assert.True(third.IsCompleted);
        Assert.True(third.Buffer.IsEmpty);
        reader.AdvanceTo(third.Buffer.End);
        reader.Complete();
    }

    [Fact]
    public async Task ReadAsync_ShouldThrowObjectDisposed_AfterDispose()
    {
        var (_, reader) = CreateEncryptedReader();
        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.ReadAsync());
        await Task.CompletedTask;
    }
}
