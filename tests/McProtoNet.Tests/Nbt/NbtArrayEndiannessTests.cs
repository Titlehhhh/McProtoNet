using System.Buffers;
using McProtoNet.NBT;
using McProtoNet.Primitives;

namespace McProtoNet.Tests.Nbt;

/// <summary>
/// Big-endian element arrays (TAG_Int_Array, TAG_Long_Array, TAG_Byte_Array) across every
/// reader path: contiguous span, multi-segment sequence, and stream. Payload alignment is
/// varied through the root name so arrays land on odd byte offsets.
/// </summary>
public class NbtArrayEndiannessTests
{
    private sealed class Segment : ReadOnlySequenceSegment<byte>
    {
        public Segment(ReadOnlyMemory<byte> memory) => Memory = memory;

        public Segment Append(ReadOnlyMemory<byte> memory)
        {
            var next = new Segment(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = next;
            return next;
        }
    }

    private static ReadOnlySequence<byte> ToSegmented(byte[] data, int segmentSize)
    {
        var first = new Segment(data.AsMemory(0, Math.Min(segmentSize, data.Length)));
        var last = first;
        for (var offset = segmentSize; offset < data.Length; offset += segmentSize)
            last = last.Append(data.AsMemory(offset, Math.Min(segmentSize, data.Length - offset)));
        return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
    }

    private static readonly int[] Ints = [int.MinValue, -1, 0, 1, 0x01020304, int.MaxValue];
    private static readonly long[] Longs = [long.MinValue, -1L, 0L, 1L, 0x0102030405060708L, long.MaxValue];
    private static readonly byte[] Bytes = [0, 1, 0x7F, 0x80, 0xFF];

    private static NbtCompound BuildArrays(string? name, string padding)
    {
        return new NbtCompound(name)
        {
            new NbtString("pad", padding),
            new NbtByteArray("bytes", Bytes),
            new NbtIntArray("ints", Ints),
            new NbtLongArray("longs", Longs)
        };
    }

    private static void AssertArrays(NbtCompound compound)
    {
        Assert.Equal(Bytes, compound.Get<NbtByteArray>("bytes")!.Value);
        Assert.Equal(Ints, compound.Get<NbtIntArray>("ints")!.Value);
        Assert.Equal(Longs, compound.Get<NbtLongArray>("longs")!.Value);
    }

    [Fact]
    public void IntArray_ExactWireBytes()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(new NbtCompound { new NbtIntArray("a", [1, -1]) }, writeRootTag: false);

        Assert.Equal(new byte[]
        {
            (byte)NbtTagType.Compound,
            (byte)NbtTagType.IntArray, 0x00, 0x01, (byte)'a',
            0x00, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x01,
            0xFF, 0xFF, 0xFF, 0xFF,
            (byte)NbtTagType.End
        }, writer.WrittenSpan.ToArray());
    }

    [Fact]
    public void LongArray_ExactWireBytes()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(new NbtCompound { new NbtLongArray("a", [0x0102030405060708L]) }, writeRootTag: false);

        Assert.Equal(new byte[]
        {
            (byte)NbtTagType.Compound,
            (byte)NbtTagType.LongArray, 0x00, 0x01, (byte)'a',
            0x00, 0x00, 0x00, 0x01,
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            (byte)NbtTagType.End
        }, writer.WrittenSpan.ToArray());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void SpanReader_ReadsArrays_AtAnyPayloadOffset(int padLength)
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildArrays(null, new string('x', padLength)), writeRootTag: false);

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        AssertArrays(Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: false)));
        Assert.Equal(0, reader.RemainingCount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(13)]
    public void SequenceReader_ReadsArrays_AcrossSegmentSplits(int segmentSize)
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildArrays("root", "xyz"), writeRootTag: true);
        var data = writer.WrittenSpan.ToArray();

        var sequence = ToSegmented(data, segmentSize);
        Assert.False(sequence.IsSingleSegment);

        var reader = new MinecraftPrimitiveReader(sequence);
        var compound = Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: true));
        Assert.Equal("root", compound.Name);
        AssertArrays(compound);
        Assert.Equal(0, reader.RemainingCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void StreamReader_ReadsArrays_AtAnyPayloadOffset(int padLength)
    {
        var tag = BuildArrays("root", new string('x', padLength));

        var stream = new MemoryStream();
        NbtWriter.WriteTag(stream, tag, true);
        var encoded = stream.ToArray();

        AssertArrays(Assert.IsType<NbtCompound>(NbtReader.ReadTag(new MemoryStream(encoded))));

        var buffer = new ArrayBufferWriter<byte>();
        NbtBufferWriter.WriteTag(buffer, tag, true);
        Assert.Equal(encoded, buffer.WrittenSpan.ToArray());
    }
}
