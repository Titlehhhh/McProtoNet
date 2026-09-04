using System.Buffers;
using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Framing;

/// <summary>
///     A frame the reader refuses must leave the pool exactly as it found it: every rented array back
///     once, never twice, and nothing kept that was already handed back. The counting pool refuses a
///     second return on the spot, so a leak surfaces here instead of as someone else's corrupted buffer.
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

    [Fact]
    public async Task BrokenIdInPlainFrame_ReturnsEveryBufferExactlyOnce()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();

        var wire = new MemoryStream(Frame(BrokenIdPacket, -1), writable: false);
        var reader = new PacketStreamReader(wire, pool, leaveOpen: true);

        var error = await Record.ExceptionAsync(async () => await reader.ReadPacketAsync(token));
        Assert.IsType<IndexOutOfRangeException>(error);
        Assert.Empty(pool.Violations);

        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);
    }

    [Fact]
    public async Task BrokenIdInCompressedFrame_ReturnsEveryBufferExactlyOnce()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();

        var wire = new MemoryStream(Frame(BrokenIdPacket, 1), writable: false);
        var reader = new PacketStreamReader(wire, pool, leaveOpen: true) { CompressionThreshold = 1 };

        var error = await Record.ExceptionAsync(async () => await reader.ReadPacketAsync(token));
        Assert.IsType<IndexOutOfRangeException>(error);
        Assert.Empty(pool.Violations);

        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);
    }

    [Fact]
    public async Task ReaderKeepsWorkingAfterARefusedFrame()
    {
        var token = TestContext.Current.CancellationToken;
        var pool = new CountingArrayPool();

        var wire = new MemoryStream();
        wire.Write(Frame(BrokenIdPacket, 1));
        wire.Write(Frame([0x07, 9, 8, 7], 1));
        wire.Position = 0;

        var reader = new PacketStreamReader(wire, pool, leaveOpen: true) { CompressionThreshold = 1 };

        var error = await Record.ExceptionAsync(async () => await reader.ReadPacketAsync(token));
        Assert.IsType<IndexOutOfRangeException>(error);

        var packet = await reader.ReadPacketAsync(token);
        Assert.Equal(0x07, packet.Id);
        Assert.Equal<byte[]>([9, 8, 7], packet.Body.ToArray());

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

        Assert.Null(Record.Exception(reader.Dispose));
        Assert.Empty(pool.Violations);
        Assert.Equal(0, pool.OnLoan);
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
