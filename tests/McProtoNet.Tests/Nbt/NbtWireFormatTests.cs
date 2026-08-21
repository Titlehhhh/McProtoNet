using System.Buffers;
using McProtoNet.NBT;
using McProtoNet.Primitives;
namespace McProtoNet.Tests.Nbt;

/// <summary>
/// Wire-format tests for the packet-path NBT serialization:
///   - MinecraftPrimitiveWriter.WriteNbt root-format switch (named vs nameless network root)
///   - MinecraftPrimitiveReader.ReadNbtTag over single- and multi-segment sequences
/// </summary>
public class NbtWireFormatTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

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

    /// <summary>Splits bytes into a multi-segment sequence with segments of the given size.</summary>
    private static ReadOnlySequence<byte> ToSegmented(byte[] data, int segmentSize)
    {
        Assert.True(data.Length > segmentSize, "need at least two segments");
        var first = new Segment(data.AsMemory(0, segmentSize));
        var last = first;
        for (var offset = segmentSize; offset < data.Length; offset += segmentSize)
        {
            var count = Math.Min(segmentSize, data.Length - offset);
            last = last.Append(data.AsMemory(offset, count));
        }

        var sequence = new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
        Assert.False(sequence.IsSingleSegment);
        return sequence;
    }

    /// <summary>A compound exercising every wire tag type, including nesting.</summary>
    private static NbtCompound BuildKitchenSink(string? name)
    {
        return new NbtCompound(name)
        {
            new NbtByte("byte", 0xAB),
            new NbtShort("short", -1234),
            new NbtInt("int", int.MinValue),
            new NbtLong("long", long.MaxValue),
            new NbtFloat("float", 3.14f),
            new NbtDouble("double", 2.71828),
            new NbtString("string", "Minecraft — привет"),
            new NbtByteArray("bytes", [1, 2, 3, 255]),
            new NbtIntArray("ints", [int.MinValue, -1, 0, 1, int.MaxValue]),
            new NbtLongArray("longs", [long.MinValue, -1L, 0L, 1L, long.MaxValue]),
            new NbtList("intList", [new NbtInt(10), new NbtInt(20), new NbtInt(30)]),
            new NbtList("compoundList",
            [
                new NbtCompound { new NbtString("k", "v1") },
                new NbtCompound { new NbtString("k", "v2") }
            ]),
            new NbtCompound("nested")
            {
                new NbtCompound("deeper") { new NbtInt("depth", 2) }
            },
            new NbtList("emptyList", NbtTagType.End),
            new NbtCompound("emptyCompound")
        };
    }

    private static void AssertKitchenSink(NbtCompound c)
    {
        Assert.Equal(0xAB, c.Get<NbtByte>("byte")!.Value);
        Assert.Equal(-1234, c.Get<NbtShort>("short")!.Value);
        Assert.Equal(int.MinValue, c.Get<NbtInt>("int")!.Value);
        Assert.Equal(long.MaxValue, c.Get<NbtLong>("long")!.Value);
        Assert.Equal(3.14f, c.Get<NbtFloat>("float")!.Value);
        Assert.Equal(2.71828, c.Get<NbtDouble>("double")!.Value);
        Assert.Equal("Minecraft — привет", c.Get<NbtString>("string")!.Value);
        Assert.Equal(new byte[] { 1, 2, 3, 255 }, c.Get<NbtByteArray>("bytes")!.Value);
        Assert.Equal(new[] { int.MinValue, -1, 0, 1, int.MaxValue }, c.Get<NbtIntArray>("ints")!.Value);
        Assert.Equal(new[] { long.MinValue, -1L, 0L, 1L, long.MaxValue }, c.Get<NbtLongArray>("longs")!.Value);

        var intList = c.Get<NbtList>("intList")!;
        Assert.Equal(3, intList.Count);
        Assert.Equal(20, intList.Get<NbtInt>(1).Value);

        var compoundList = c.Get<NbtList>("compoundList")!;
        Assert.Equal(2, compoundList.Count);
        Assert.Equal("v2", compoundList.Get<NbtCompound>(1).Get<NbtString>("k")!.Value);

        Assert.Equal(2, c.Get<NbtCompound>("nested")!.Get<NbtCompound>("deeper")!.Get<NbtInt>("depth")!.Value);
        Assert.Empty(c.Get<NbtList>("emptyList")!);
        Assert.Empty(c.Get<NbtCompound>("emptyCompound")!);
    }

    // ── Root-format switch ────────────────────────────────────────────────────

    [Fact]
    public void WriteNbt_NamelessRoot_ExactWireBytes()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(new NbtCompound { new NbtInt("v", 7) }, writeRootTag: false);

        // Network root: type byte, then immediately the payload — no root name.
        Assert.Equal(new byte[]
        {
            (byte)NbtTagType.Compound,
            (byte)NbtTagType.Int, 0x00, 0x01, (byte)'v', 0x00, 0x00, 0x00, 0x07,
            (byte)NbtTagType.End
        }, writer.WrittenSpan.ToArray());
    }

    [Fact]
    public void WriteNbt_NamelessRoot_ReadNbt_Roundtrip()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink("ignored"), writeRootTag: false);

        Assert.Equal((byte)NbtTagType.Compound, writer.WrittenSpan[0]);

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var c = Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: false));
        Assert.Null(c.Name);
        AssertKitchenSink(c);
        Assert.Equal(0, reader.RemainingCount);
    }

    [Fact]
    public void WriteNbt_NamedRoot_ReadNbt_Roundtrip()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink("root"), writeRootTag: true);

        // Named root: type byte, then big-endian ushort name length and UTF-8 name.
        Assert.Equal((byte)NbtTagType.Compound, writer.WrittenSpan[0]);
        Assert.Equal(new byte[] { 0x00, 0x04, (byte)'r', (byte)'o', (byte)'o', (byte)'t' },
            writer.WrittenSpan.Slice(1, 6).ToArray());

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var c = Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: true));
        Assert.Equal("root", c.Name);
        AssertKitchenSink(c);
        Assert.Equal(0, reader.RemainingCount);
    }

    [Fact]
    public void WriteNbt_NamedRoot_NullName_WritesEmptyName()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(new NbtCompound { new NbtInt("v", 7) }, writeRootTag: true);

        Assert.Equal((byte)NbtTagType.Compound, writer.WrittenSpan[0]);
        Assert.Equal(new byte[] { 0x00, 0x00 }, writer.WrittenSpan.Slice(1, 2).ToArray());

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var c = Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: true));
        Assert.Equal(7, c.Get<NbtInt>("v")!.Value);
    }

    [Fact]
    public void WriteOptionalNbt_NamedRoot_Roundtrip()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteOptionalNbt(new NbtCompound("c") { new NbtInt("v", 7) }, writeRootTag: true);

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var c = Assert.IsType<NbtCompound>(reader.ReadOptionalNbtTag(readRootTag: true));
        Assert.Equal("c", c.Name);
        Assert.Equal(7, c.Get<NbtInt>("v")!.Value);
    }

    // ── Reading from a multi-segment sequence ─────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void ReadNbtTag_MultiSegmentSequence_Roundtrip(int segmentSize)
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink(null), writeRootTag: false);
        var bytes = writer.WrittenSpan.ToArray();

        var reader = new MinecraftPrimitiveReader(ToSegmented(bytes, segmentSize));
        var c = Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: false));

        AssertKitchenSink(c);
        Assert.Equal(0, reader.RemainingCount);
    }

    [Fact]
    public void ReadNbtTag_MultiSegment_NamedRoot_SplitInsideName()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink("segmented-root-name"), writeRootTag: true);
        var bytes = writer.WrittenSpan.ToArray();

        // Segment size 2 splits both the name length prefix and the name itself.
        var reader = new MinecraftPrimitiveReader(ToSegmented(bytes, 2));
        var c = Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: true));

        Assert.Equal("segmented-root-name", c.Name);
        AssertKitchenSink(c);
    }

    [Fact]
    public void ReadNbtTag_MultiSegment_ConsumesExactly()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink(null));
        writer.WriteVarInt(12345);
        writer.WriteString("trailing");
        var bytes = writer.WrittenSpan.ToArray();

        var reader = new MinecraftPrimitiveReader(ToSegmented(bytes, 3));
        Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: false));

        // The tag must consume exactly its own bytes, leaving the rest of the packet intact.
        Assert.Equal(12345, reader.ReadVarInt());
        Assert.Equal("trailing", reader.ReadString());
        Assert.Equal(0, reader.RemainingCount);
    }

    [Fact]
    public void ReadNbtTag_SingleSegment_ConsumesExactly()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink(null));
        writer.WriteVarInt(12345);
        var bytes = writer.WrittenSpan.ToArray();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(bytes));
        Assert.IsType<NbtCompound>(reader.ReadNbtTag(readRootTag: false));

        Assert.Equal(12345, reader.ReadVarInt());
        Assert.Equal(0, reader.RemainingCount);
    }

    [Fact]
    public void ReadNbtTag_MultiSegment_TagEnd_ReturnsNull()
    {
        byte[] bytes = [(byte)NbtTagType.End, 0xEE];
        var reader = new MinecraftPrimitiveReader(ToSegmented(bytes, 1));

        Assert.Null(reader.ReadNbtTag(readRootTag: false));
        Assert.Equal(0xEE, reader.ReadUnsignedByte());
    }

    [Fact]
    public void ReadNbtTag_MultiSegment_Truncated_Throws()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(BuildKitchenSink(null));
        var truncated = writer.WrittenSpan.Slice(0, writer.WrittenSpan.Length / 2).ToArray();

        Assert.Throws<EndOfStreamException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(ToSegmented(truncated, 3));
            reader.ReadNbtTag(readRootTag: false);
        });
    }
}
