using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using McProtoNet.Cryptography;
using McProtoNet.Net;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

public class CryptoPipeWriterTests
{
    private static readonly byte[] TestKey = "0123456789ABCDEF"u8.ToArray();

    private static (Pipe pipe, CryptoPipeWriter writer) CreatePlain()
    {
        var pipe = new Pipe();
        return (pipe, new CryptoPipeWriter(pipe.Writer));
    }

    private static (Pipe pipe, CryptoPipeWriter writer) CreateEncrypted()
    {
        var (pipe, writer) = CreatePlain();
        writer.EnableEncryption(PacketCipher.CreateEncryptor(TestKey));
        return (pipe, writer);
    }

    private static IBufferedCipher CreateReferenceCipher(bool forEncryption)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(TestKey), TestKey));
        return cipher;
    }

    private static byte[] ReferenceDecrypt(byte[] data)
    {
        var cipher = CreateReferenceCipher(false);
        byte[] output = new byte[cipher.GetOutputSize(data.Length)];
        int written = cipher.ProcessBytes(data, 0, data.Length, output, 0);
        return output.AsSpan(0, written).ToArray();
    }

    private static void WriteBytes(CryptoPipeWriter writer, ReadOnlySpan<byte> data)
    {
        data.CopyTo(writer.GetSpan(data.Length));
        writer.Advance(data.Length);
    }

    private static byte[] RandomBytes(int count)
    {
        byte[] data = new byte[count];
        new Random(40).NextBytes(data);
        return data;
    }

    private static async Task<byte[]> ReadAllAsync(PipeReader reader)
    {
        while (true)
        {
            ReadResult result = await reader.ReadAsync();
            if (result.IsCompleted)
            {
                byte[] data = result.Buffer.ToArray();
                reader.AdvanceTo(result.Buffer.End);
                return data;
            }

            reader.AdvanceTo(result.Buffer.Start, result.Buffer.End);
        }
    }

    [Fact]
    public async Task Flush_ShouldEncryptWrittenBytes()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = Encoding.UTF8.GetBytes("Hello Minecraft World!");

        WriteBytes(writer, plain);
        await writer.FlushAsync();
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.NotEqual(plain, transferred);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public async Task Flush_ShouldPassBytesThrough_WhenNotEncrypted()
    {
        var (pipe, writer) = CreatePlain();
        byte[] plain = Encoding.UTF8.GetBytes("no encryption test");

        WriteBytes(writer, plain);
        await writer.FlushAsync();
        writer.Complete();

        Assert.Equal(plain, await ReadAllAsync(pipe.Reader));
    }

    [Fact]
    public async Task Flush_ShouldKeepCipherContinuous_AcrossFlushes()
    {
        var (pipe, writer) = CreateEncrypted();

        foreach (string part in new[] { "segment1-", "segment2-", "segment3" })
        {
            WriteBytes(writer, Encoding.UTF8.GetBytes(part));
            await writer.FlushAsync();
        }

        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal("segment1-segment2-segment3", Encoding.UTF8.GetString(ReferenceDecrypt(transferred)));
    }

    [Fact]
    public async Task Flush_ShouldKeepCipherContinuous_AcrossSegmentBoundaries()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = RandomBytes(9000);

        WriteBytes(writer, plain.AsSpan(0, 3000));
        WriteBytes(writer, plain.AsSpan(3000, 3000));
        WriteBytes(writer, plain.AsSpan(6000, 3000));
        await writer.FlushAsync();
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public async Task Write_ShouldHandleWriteLargerThanSegment()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = RandomBytes(20_000);

        WriteBytes(writer, plain);
        await writer.FlushAsync();
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public void Write_ShouldNotReachInnerPipe_BeforeFlush_WhenEncrypted()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = Encoding.UTF8.GetBytes("buffered until flush");

        WriteBytes(writer, plain);

        Assert.Equal(0, pipe.Writer.UnflushedBytes);
        Assert.Equal(plain.Length, writer.UnflushedBytes);
        Assert.False(pipe.Reader.TryRead(out _));
    }

    [Fact]
    public async Task Write_ShouldKeepCipherContinuous_AcrossRandomChunksAndFlushes()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = RandomBytes(50_000);
        var random = new Random(7);

        int offset = 0;
        while (offset < plain.Length)
        {
            int length = Math.Min(random.Next(1, 3000), plain.Length - offset);
            WriteBytes(writer, plain.AsSpan(offset, length));
            offset += length;
            if (random.Next(4) == 0)
            {
                await writer.FlushAsync();
            }
        }

        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public async Task Write_ShouldEncryptEveryChunk_WhenBufferWriterExtensionSplitsTheWrite()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = RandomBytes(50_000);

        writer.WriteVarInt(plain.Length);
        writer.Write(plain);
        await writer.FlushAsync();
        writer.Complete();

        byte[] frame = ReferenceDecrypt(await ReadAllAsync(pipe.Reader));
        Assert.Equal(plain.Length, frame.AsSpan().ReadVarInt(out int headerLength));
        Assert.Equal(plain, frame.AsSpan(headerLength).ToArray());
    }

    [Fact]
    public async Task Advance_ShouldCommitOnlyAdvancedBytes_WhenSplitAcrossCalls()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = RandomBytes(100);

        plain.CopyTo(writer.GetSpan(plain.Length));
        writer.Advance(40);
        writer.Advance(60);
        await writer.FlushAsync();
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public async Task GetSpan_ShouldDropUnadvancedBytes_WhenRequestedAgain()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] dropped = RandomBytes(10);
        byte[] kept = RandomBytes(5000);

        dropped.CopyTo(writer.GetSpan(dropped.Length));
        kept.CopyTo(writer.GetSpan(kept.Length));
        writer.Advance(kept.Length);
        await writer.FlushAsync();
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(kept, ReferenceDecrypt(transferred));
    }

    [Fact]
    public void EnableEncryption_ShouldInvalidatePlaintextBuffer_IssuedBeforeEnabling()
    {
        var (pipe, writer) = CreatePlain();
        byte[] plain = Encoding.UTF8.GetBytes("issued before, advanced after");

        plain.CopyTo(writer.GetSpan(plain.Length));
        writer.EnableEncryption(TestKey);

        Assert.Throws<InvalidOperationException>(() => writer.Advance(plain.Length));
        Assert.Equal(0, pipe.Writer.UnflushedBytes);
    }

    [Fact]
    public void UnflushedBytes_ShouldCountEveryAdvance_WhenEncrypted()
    {
        var (_, writer) = CreateEncrypted();

        WriteBytes(writer, RandomBytes(3));
        WriteBytes(writer, RandomBytes(5000));
        WriteBytes(writer, RandomBytes(7));

        Assert.True(writer.CanGetUnflushedBytes);
        Assert.Equal(5010, writer.UnflushedBytes);
    }

    [Fact]
    public void Write_ShouldReachInnerPipe_Immediately_WhenNotEncrypted()
    {
        var (pipe, writer) = CreatePlain();
        byte[] plain = Encoding.UTF8.GetBytes("transparent");

        WriteBytes(writer, plain);

        Assert.Equal(plain.Length, pipe.Writer.UnflushedBytes);
        Assert.Equal(plain.Length, writer.UnflushedBytes);
    }

    [Fact]
    public async Task Flush_ShouldDropUnflushedBytes_ToZero()
    {
        var (_, writer) = CreateEncrypted();
        WriteBytes(writer, Encoding.UTF8.GetBytes("pending"));

        await writer.FlushAsync();

        Assert.Equal(0, writer.UnflushedBytes);
    }

    [Fact]
    public async Task Complete_ShouldDrainPendingBytes()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = Encoding.UTF8.GetBytes("drained on complete");

        WriteBytes(writer, plain);
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public async Task CompleteAsync_ShouldDrainPendingBytes()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = Encoding.UTF8.GetBytes("drained on complete");

        WriteBytes(writer, plain);
        await writer.CompleteAsync();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public async Task Complete_ShouldDiscardPendingBytes_OnError()
    {
        var (pipe, writer) = CreateEncrypted();
        WriteBytes(writer, Encoding.UTF8.GetBytes("must not leak"));

        writer.Complete(new InvalidOperationException("boom"));

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await pipe.Reader.ReadAsync());
        Assert.Equal("boom", thrown.Message);
    }

    [Fact]
    public async Task CompleteAsync_ShouldDiscardPendingBytes_OnError()
    {
        var (pipe, writer) = CreateEncrypted();
        WriteBytes(writer, Encoding.UTF8.GetBytes("must not leak"));

        await writer.CompleteAsync(new InvalidOperationException("boom"));

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await pipe.Reader.ReadAsync());
        Assert.Equal("boom", thrown.Message);
    }

    [Fact]
    public async Task CancelPendingFlush_ShouldCancelFlush_ButKeepData()
    {
        var (pipe, writer) = CreateEncrypted();
        byte[] plain = Encoding.UTF8.GetBytes("survives cancellation");
        WriteBytes(writer, plain);

        writer.CancelPendingFlush();
        FlushResult cancelled = await writer.FlushAsync();
        FlushResult completed = await writer.FlushAsync();
        writer.Complete();

        Assert.True(cancelled.IsCanceled);
        Assert.False(completed.IsCanceled);
        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(plain, ReferenceDecrypt(transferred));
    }

    [Fact]
    public void EnableEncryption_ShouldThrow_WhenAlreadyEnabled()
    {
        var (_, writer) = CreateEncrypted();

        Assert.Throws<InvalidOperationException>(
            () => writer.EnableEncryption(PacketCipher.CreateEncryptor(TestKey)));
    }

    [Fact]
    public async Task EnableEncryption_ShouldThrow_WhenPlaintextPending()
    {
        var (_, writer) = CreatePlain();
        WriteBytes(writer, Encoding.UTF8.GetBytes("unflushed plaintext"));

        Assert.Throws<InvalidOperationException>(() => writer.EnableEncryption(TestKey));

        await writer.FlushAsync();
        writer.EnableEncryption(TestKey);
        Assert.True(writer.EncryptionEnabled);
    }

    [Fact]
    public void EnableEncryption_ShouldThrow_OnWrongSecretLength()
    {
        var (_, writer) = CreatePlain();

        Assert.Throws<ArgumentException>(() => writer.EnableEncryption("12345678"u8));
    }

    [Fact]
    public async Task EnableEncryption_ShouldStartCipher_AtFlushBoundary()
    {
        var (pipe, writer) = CreatePlain();
        byte[] handshake = Encoding.UTF8.GetBytes("handshake");
        byte[] secret = Encoding.UTF8.GetBytes("secret payload");

        WriteBytes(writer, handshake);
        await writer.FlushAsync();
        writer.EnableEncryption(TestKey);
        WriteBytes(writer, secret);
        await writer.FlushAsync();
        writer.Complete();

        byte[] transferred = await ReadAllAsync(pipe.Reader);
        Assert.Equal(handshake, transferred.AsSpan(0, handshake.Length).ToArray());
        Assert.Equal(secret, ReferenceDecrypt(transferred.AsSpan(handshake.Length).ToArray()));
    }

    [Fact]
    public void Advance_ShouldThrow_OnNegative()
    {
        var (_, writer) = CreateEncrypted();
        writer.GetSpan(16);

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(-1));
    }

    [Fact]
    public void Advance_ShouldThrow_WhenPastSegmentEnd()
    {
        var (_, writer) = CreateEncrypted();
        writer.GetSpan(16);

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(1_000_000));
    }

    [Fact]
    public async Task Advance_ShouldThrow_WithoutRequestedBuffer_AfterFlush()
    {
        var (_, writer) = CreateEncrypted();
        WriteBytes(writer, Encoding.UTF8.GetBytes("first"));
        await writer.FlushAsync();

        Assert.Throws<InvalidOperationException>(() => writer.Advance(10));
    }

    [Fact]
    public void GetSpan_ShouldThrow_AfterDispose()
    {
        var (_, writer) = CreateEncrypted();
        writer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => writer.GetSpan(1));
        Assert.Throws<ObjectDisposedException>(() => writer.GetMemory(1));
        Assert.Throws<ObjectDisposedException>(() => writer.Advance(0));
    }

    [Fact]
    public void WritePacket_ShouldThrow_AfterDispose()
    {
        var pipe = new Pipe();
        var writer = new MinecraftPacketPipeWriter(pipe.Writer);
        writer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => writer.WritePacket("x"u8));
    }

    [Fact]
    public void Dispose_ShouldBeIdempotent()
    {
        var (_, writer) = CreateEncrypted();
        WriteBytes(writer, Encoding.UTF8.GetBytes("pending"));

        writer.Dispose();
        writer.Dispose();
    }

    [Fact]
    public void GetMemory_ShouldReturnAtLeastSizeHint()
    {
        var (_, writer) = CreateEncrypted();

        Assert.True(writer.GetMemory(100_000).Length >= 100_000);
        Assert.True(writer.GetSpan(100_000).Length >= 100_000);
    }

    [Fact]
    public async Task Complete_ShouldDropEvenFlushedBytes_OnError()
    {
        var (pipe, writer) = CreateEncrypted();
        WriteBytes(writer, RandomBytes(20));
        await writer.FlushAsync();
        WriteBytes(writer, RandomBytes(30));

        writer.Complete(new InvalidOperationException("boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await pipe.Reader.ReadAsync());
    }

    [Fact]
    public async Task WritePacket_ShouldEncryptWholeFrame()
    {
        var pipe = new Pipe();
        using var writer = new MinecraftPacketPipeWriter(pipe.Writer);
        writer.EnableEncryption(TestKey);
        byte[] body = Encoding.UTF8.GetBytes("hello");

        writer.WritePacket(0x2A, body);
        await writer.FlushAsync();
        writer.Complete();

        byte[] frame = ReferenceDecrypt(await ReadAllAsync(pipe.Reader));
        Assert.Equal(6, frame[0]);
        Assert.Equal(0x2A, frame[1]);
        Assert.Equal(body, frame.AsSpan(2).ToArray());
    }

    [Fact]
    public async Task WritePacket_ShouldThrow_OnConcurrentWrite()
    {
        var pipe = new Pipe();
        var blocking = new BlockingPipeWriter(pipe.Writer);
        using var writer = new MinecraftPacketPipeWriter(blocking);
        byte[] packet = Encoding.UTF8.GetBytes("hello");

        var first = Task.Run(() => writer.WritePacket(packet));
        Assert.True(blocking.Entered.Wait(TimeSpan.FromSeconds(10)));

        Assert.Throws<InvalidOperationException>(() => writer.WritePacket(packet));

        blocking.Release.Set();
        await first;
    }

    private sealed class BlockingPipeWriter : PipeWriter
    {
        private readonly PipeWriter _inner;

        public BlockingPipeWriter(PipeWriter inner) => _inner = inner;

        public ManualResetEventSlim Entered { get; } = new(false);
        public ManualResetEventSlim Release { get; } = new(false);

        public override Span<byte> GetSpan(int sizeHint = 0)
        {
            Entered.Set();
            Release.Wait();
            return _inner.GetSpan(sizeHint);
        }

        public override Memory<byte> GetMemory(int sizeHint = 0)
        {
            Entered.Set();
            Release.Wait();
            return _inner.GetMemory(sizeHint);
        }

        public override void Advance(int bytes) => _inner.Advance(bytes);

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
            => _inner.FlushAsync(cancellationToken);

        public override void Complete(Exception? exception = null) => _inner.Complete(exception);

        public override void CancelPendingFlush() => _inner.CancelPendingFlush();
    }
}
