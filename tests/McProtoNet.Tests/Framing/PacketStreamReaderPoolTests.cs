using System.Buffers;
using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Framing;

/// <summary>
///     Every array the reader rents goes back exactly once, and the one behind a packet goes back when
///     the packet is disposed: not at the next read, not when the reader is disposed. A frame the
///     reader refuses leaves nothing on loan. The counting pool refuses a second return on the spot,
///     so a leak surfaces here instead of as someone else's corrupted buffer.
/// </summary>
/// <remarks>
///     The reader is disposed by hand rather than with <c>using</c>: a throw from <c>Dispose</c> would
///     replace the failure inside the block and hide which of the two moments actually leaked.
/// </remarks>
public class PacketStreamReaderPoolTests
{
    /// <summary>A packet whose id varint is cut short: one 0x80 byte, no continuation byte behind it.</summary>
    private static byte[] BrokenIdPacket => [0x80];

    private static byte[] Frame(ReadOnlySpan<byte> packet, int compressionThreshold)
    {
        var writer = new ArrayBufferWriter<byte>(64);
        writer.WritePacket(packet, compressionThreshold);
        return writer.WrittenSpan.ToArray();
    }

    private static MemoryStream Wire(int compressionThreshold, params byte[][] packets)
    {
        var wire = new MemoryStream();
        foreach (var packet in packets) wire.Write(Frame(packet, compressionThreshold));
        wire.Position = 0;
        return wire;
    }

    /// <summary>With threshold 1 every body here is above the threshold, so it goes through inflate.</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public async Task Packet_OwnsItsBuffer_PastTheNextRead_AndPastTheReader(int threshold)
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var reader = new PacketStreamReader(Wire(threshold, [0x07, 9, 8, 7], [0x08, 1, 2]), pool, leaveOpen: true)
            { CompressionThreshold = threshold };

        var first = await reader.ReadPacketAsync(token);
        Assert.Equal(1, pool.OnLoan);

        var second = await reader.ReadPacketAsync(token);
        Assert.Equal(2, pool.OnLoan);
        Assert.Equal<byte[]>([9, 8, 7], first.Body.ToArray());
        Assert.Equal<byte[]>([1, 2], second.Body.ToArray());

        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Equal(2, pool.OnLoan);
        Assert.Equal(0x07, first.Id);
        Assert.Equal<byte[]>([9, 8, 7], first.Body.ToArray());

        first.Dispose();
        second.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public async Task CompressedFrame_ReturnsTheWireBuffer_AsSoonAsItIsInflated()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var reader = new PacketStreamReader(Wire(1, [0x07, 9, 8, 7]), pool, leaveOpen: true)
            { CompressionThreshold = 1 };

        var packet = await reader.ReadPacketAsync(token);

        Assert.Equal(2, pool.Rents);
        Assert.Equal(1, pool.OnLoan);

        packet.Dispose();
        Assert.Equal(0, pool.OnLoan);
        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public async Task BrokenId_ReturnsEveryBufferExactlyOnce(int threshold)
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var reader = new PacketStreamReader(Wire(threshold, BrokenIdPacket), pool, leaveOpen: true)
            { CompressionThreshold = threshold };

        var error = await Record.ExceptionAsync(async () => await reader.ReadPacketAsync(token));
        Assert.IsType<IndexOutOfRangeException>(error);
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);

        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);
    }

    [Fact]
    public async Task ReaderKeepsWorkingAfterARefusedFrame()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();
        var reader = new PacketStreamReader(Wire(1, BrokenIdPacket, [0x07, 9, 8, 7]), pool, leaveOpen: true)
            { CompressionThreshold = 1 };

        var error = await Record.ExceptionAsync(async () => await reader.ReadPacketAsync(token));
        Assert.IsType<IndexOutOfRangeException>(error);

        var packet = await reader.ReadPacketAsync(token);
        Assert.Equal(0x07, packet.Id);
        Assert.Equal<byte[]>([9, 8, 7], packet.Body.ToArray());

        packet.Dispose();
        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);
    }

    [Fact]
    public async Task EmptyCompressionEnvelope_ThrowsInvalidData_AndLeavesNothingRented()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();

        // one byte of body: the "not compressed" size varint and nothing behind it
        var wire = new MemoryStream([0x01, 0x00], writable: false);
        var reader = new PacketStreamReader(wire, pool, leaveOpen: true) { CompressionThreshold = 0 };

        var error = await Record.ExceptionAsync(async () => await reader.ReadPacketAsync(token));
        Assert.IsType<InvalidDataException>(error);
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);

        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
    }

    [Fact]
    public async Task EmptyCompressionEnvelope_BothReadersSayTheSameThing()
    {
        var token = TestContext.Current.CancellationToken;
        byte[] wire = [0x01, 0x00];

        using var streamReader =
            new PacketStreamReader(new MemoryStream(wire, writable: false), leaveOpen: true)
                { CompressionThreshold = 0 };
        var fromStreamReader = await Record.ExceptionAsync(async () => await streamReader.ReadPacketAsync(token));

        using var bufferedReader = new BufferedPacketReader(new MemoryStream(wire, writable: false), 0);
        var fromBufferedReader = await Record.ExceptionAsync(async () => await bufferedReader.ReadBatchAsync(token));

        Assert.IsType<InvalidDataException>(fromBufferedReader);
        Assert.IsType<InvalidDataException>(fromStreamReader);
        Assert.Equal(fromBufferedReader.Message, fromStreamReader.Message);
    }
}
