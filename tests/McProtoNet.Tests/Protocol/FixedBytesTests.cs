using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace McProtoNet.Tests.Protocol;

/// <summary>
/// The exactly-n-bytes payload the generated code uses for protodef's fixed buffers
/// (ArgumentSignature.signature, ChatCommandSigned.acknowledged): no length prefix on the wire,
/// so the length is a contract the writer must enforce and the reader must consume exactly.
/// </summary>
public class FixedBytesTests
{
    private static byte[] Payload(int length)
        => [.. Enumerable.Range(0, length).Select(i => (byte)(i * 7 + 1))];

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(256)]
    public void WriteThenRead_RoundTrips_WithoutLengthPrefix(int length)
    {
        var value = Payload(length);
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteFixedBytes(value, length);
        using var mem = writer.GetWrittenMemory();

        Assert.Equal(length, mem.Memory.Length);

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        Assert.Equal(value, reader.ReadFixedBytes(length));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(64)]
    public void Read_SpansSegmentBoundaries(int segmentSize)
    {
        var value = Payload(256);
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(7);
        writer.WriteFixedBytes(value, 256);
        writer.WriteVarInt(9);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(Chunked(mem.Memory.ToArray(), segmentSize));
        Assert.Equal(7, reader.ReadVarInt());
        Assert.Equal(value, reader.ReadFixedBytes(256));
        Assert.Equal(9, reader.ReadVarInt());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(0)]
    public void Write_WrongLength_Throws(int actual)
    {
        var writer = new MinecraftPrimitiveWriter();

        Assert.Throws<ArgumentException>(() => writer.WriteFixedBytes(Payload(actual), 3));
    }

    private static ReadOnlySequence<byte> Chunked(byte[] data, int segmentSize)
    {
        Seg? first = null, prev = null;
        long pos = 0;
        foreach (var chunk in data.Chunk(segmentSize))
        {
            var seg = new Seg(chunk, pos);
            first ??= seg;
            prev?.SetNext(seg);
            prev = seg;
            pos += chunk.Length;
        }

        return new ReadOnlySequence<byte>(first!, 0, prev!, prev!.Memory.Length);
    }

    private sealed class Seg : ReadOnlySequenceSegment<byte>
    {
        public Seg(ReadOnlyMemory<byte> memory, long runningIndex)
        {
            Memory = memory;
            RunningIndex = runningIndex;
        }

        public void SetNext(Seg next) => Next = next;
    }
}
