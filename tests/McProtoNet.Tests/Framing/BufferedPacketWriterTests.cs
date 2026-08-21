using System.Buffers;
using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Framing;

public class BufferedPacketWriterTests
{
    public static TheoryData<int, bool> Modes => new()
    {
        { -1, false }, { -1, true },
        { 0, false }, { 0, true },
        { 256, false }, { 256, true }
    };

    [Theory]
    [MemberData(nameof(Modes))]
    public async Task RoundTripsThroughStreamReader(int threshold, bool encrypted)
    {
        var token = TestContext.Current.CancellationToken;
        var packets = Frames.Sample(seed: 17, repeats: 1);

        var sink = new MemoryStream();
        using (var encryptor = encrypted ? Crypto.CreateEncryptor() : null)
        using (var writer = new BufferedPacketWriter(sink, threshold, encryptor))
        {
            foreach (var packet in packets)
            {
                writer.WritePacket(packet.Id, packet.Body);
                if (packet.Id % 3 == 0) await writer.FlushAsync(token);
            }

            await writer.CompleteAsync();
            Assert.Equal(0, writer.UnflushedBytes);
        }

        sink.Position = 0;
        using var cipher = Frames.Decryptor(encrypted);
        using var reader = new PacketStreamReader(sink, leaveOpen: true)
        {
            CompressionThreshold = threshold,
            Cipher = cipher
        };

        foreach (var expected in packets)
        {
            var packet = await reader.ReadPacketAsync(token);
            Assert.Equal(expected.Id, packet.Id);
            Assert.Equal(expected.Body, packet.Body.ToArray());
        }
    }

    [Theory]
    [MemberData(nameof(Modes))]
    public async Task SequenceOverloadMatchesSpanOverload(int threshold, bool encrypted)
    {
        var token = TestContext.Current.CancellationToken;
        var body = new byte[5000];
        Random.Shared.NextBytes(body);

        var contiguous = await WriteOne(threshold, encrypted, w => w.WritePacket(42, body), token);
        var segmented = await WriteOne(threshold, encrypted,
            w => w.WritePacket(42, Segments(body, 700)), token);

        Assert.Equal(contiguous, segmented);
    }

    [Fact]
    public async Task UnflushedBytesGrowsUntilFlushed()
    {
        var token = TestContext.Current.CancellationToken;
        var sink = new MemoryStream();
        using var writer = new BufferedPacketWriter(sink);

        Assert.Equal(0, writer.UnflushedBytes);
        writer.WritePacket(1, new byte[10]);
        var afterOne = writer.UnflushedBytes;
        Assert.True(afterOne > 10);

        writer.WritePacket(2, new byte[10]);
        Assert.Equal(afterOne * 2, writer.UnflushedBytes);
        Assert.Equal(0, sink.Length);

        await writer.FlushAsync(token);

        Assert.Equal(0, writer.UnflushedBytes);
        Assert.Equal(afterOne * 2, sink.Length);
    }

    [Fact]
    public async Task FailedFlushKillsTheWriterForGood()
    {
        var token = TestContext.Current.CancellationToken;
        using var writer = new BufferedPacketWriter(new FailingWriteStream());
        writer.WritePacket(1, new byte[10]);

        await Assert.ThrowsAsync<IOException>(async () => await writer.FlushAsync(token));

        Assert.Throws<IOException>(() => writer.WritePacket(2, new byte[10]));
        Assert.Throws<IOException>(() => _ = writer.UnflushedBytes);
        await Assert.ThrowsAsync<IOException>(async () => await writer.FlushAsync(token));
    }

    [Fact]
    public async Task CancelledFlushKillsTheWriterForGood()
    {
        var gate = new GateStream();
        using var writer = new BufferedPacketWriter(gate);
        writer.WritePacket(1, new byte[10]);

        using var cts = new CancellationTokenSource();
        var flush = writer.FlushAsync(cts.Token).AsTask();
        await gate.WriteStarted;
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => flush);
        Assert.ThrowsAny<OperationCanceledException>(() => writer.WritePacket(2, new byte[10]));
    }

    private static async Task<byte[]> WriteOne(int threshold, bool encrypted, Action<BufferedPacketWriter> write,
        CancellationToken token)
    {
        var sink = new MemoryStream();
        using (var encryptor = encrypted ? Crypto.CreateEncryptor() : null)
        using (var writer = new BufferedPacketWriter(sink, threshold, encryptor))
        {
            write(writer);
            await writer.FlushAsync(token);
        }

        return sink.ToArray();
    }

    private static ReadOnlySequence<byte> Segments(byte[] data, int chunk)
    {
        Segment? first = null;
        Segment? last = null;
        for (var offset = 0; offset < data.Length; offset += chunk)
        {
            var length = Math.Min(chunk, data.Length - offset);
            var next = new Segment(data.AsMemory(offset, length), last);
            first ??= next;
            last = next;
        }

        return new ReadOnlySequence<byte>(first!, 0, last!, last!.Memory.Length);
    }

    private sealed class Segment : ReadOnlySequenceSegment<byte>
    {
        public Segment(ReadOnlyMemory<byte> memory, Segment? previous)
        {
            Memory = memory;
            if (previous is null) return;

            RunningIndex = previous.RunningIndex + previous.Memory.Length;
            previous.Next = this;
        }
    }
}
