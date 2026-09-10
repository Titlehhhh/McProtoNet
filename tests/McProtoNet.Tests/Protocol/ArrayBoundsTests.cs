using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace McProtoNet.Tests.Protocol;

/// <summary>
/// A length-prefixed array takes its element count from the wire, so the count is whatever the peer
/// says it is. The array must never be allocated before the frame is known to carry that many
/// elements: three bytes of VarInt can otherwise ask for a two-billion-element array.
/// </summary>
public class ArrayBoundsTests
{
    private const int Version = MinecraftVersion.LatestProtocol;

    private static void PutVarInt(List<byte> to, int value)
    {
        var bits = (uint)value;
        while ((bits & ~0x7Fu) != 0)
        {
            to.Add((byte)((bits & 0x7F) | 0x80));
            bits >>= 7;
        }

        to.Add((byte)bits);
    }

    private static byte[] CountOnly(int count)
    {
        var bytes = new List<byte>();
        PutVarInt(bytes, count);
        return [.. bytes];
    }

    [Theory]
    [InlineData(2_000_000_000)]
    [InlineData(int.MaxValue)]
    [InlineData(-1)]
    public void CountThatCannotFit_Throws(int count)
    {
        var data = CountOnly(count);

        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
            reader.ReadStringArray();
        });
    }

    [Fact]
    public void HugeCount_IsRefusedBeforeAnythingIsAllocated()
    {
        var data = CountOnly(2_000_000_000);

        var before = GC.GetAllocatedBytesForCurrentThread();
        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
            reader.ReadTypeArray<Tag>(Version);
        });
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(allocated < 64 * 1024, $"refusing the count allocated {allocated} bytes");
    }

    [Fact]
    public void CountLargerThanTheBytesLeft_Throws()
    {
        var bytes = new List<byte>();
        PutVarInt(bytes, 100);
        bytes.AddRange([1, 2, 3]);
        var data = bytes.ToArray();

        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
            reader.ReadVarIntArray();
        });
    }

    [Fact]
    public void ElementSizeTightensTheBound()
    {
        var bytes = new List<byte>();
        PutVarInt(bytes, 64);
        bytes.AddRange(new byte[64]);
        var data = bytes.ToArray();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
        Assert.Equal(64, reader.ReadCount());

        Assert.Throws<InvalidDataException>(() =>
        {
            var strict = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
            strict.ReadSignedLongArray();
        });
    }

    [Fact]
    public void MaxCountIsHonouredEvenWhenTheBytesAreThere()
    {
        var bytes = new List<byte>();
        PutVarInt(bytes, 3);
        PutVarInt(bytes, 1);
        PutVarInt(bytes, 2);
        PutVarInt(bytes, 3);
        var data = bytes.ToArray();

        Assert.Throws<InvalidDataException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
            reader.ReadVarIntArray(maxCount: 2);
        });
    }

    [Fact]
    public void EmptyArrayCostsNoAllocation()
    {
        var data = CountOnly(0);

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
        var first = reader.ReadStringArray();

        var again = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(data));
        var second = again.ReadStringArray();

        Assert.Empty(first);
        Assert.Same(first, second);
    }

    [Fact]
    public void StringArray_RoundTrips()
    {
        var value = new[] { "alpha", "", "омега" };
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteStringArray(value);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        Assert.Equal(value, reader.ReadStringArray());
    }

    [Fact]
    public void VarIntArray_RoundTrips()
    {
        var value = new[] { 0, 1, -1, int.MaxValue, int.MinValue };
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarIntArray(value);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        Assert.Equal(value, reader.ReadVarIntArray());
    }

    [Fact]
    public void SignedLongArray_RoundTrips()
    {
        var value = new[] { 0L, -1L, long.MaxValue, long.MinValue };
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteSignedLongArray(value);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        Assert.Equal(value, reader.ReadSignedLongArray());
    }

    [Fact]
    public void UuidArray_RoundTrips()
    {
        var value = new[] { Guid.Empty, Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8") };
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteUuidArray(value);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        Assert.Equal(value, reader.ReadUuidArray());
    }

    [Fact]
    public void TypeArray_RoundTrips()
    {
        var value = new[] { new Tag("minecraft:logs", [1, 2, 3]), new Tag("minecraft:wool", []) };
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteTypeArray(value, Version);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        var read = reader.ReadTypeArray<Tag>(Version);

        Assert.Equal(value.Length, read.Length);
        Assert.Equal(value[0].TagName, read[0].TagName);
        Assert.Equal(value[0].Entries, read[0].Entries);
        Assert.Equal(value[1].TagName, read[1].TagName);
        Assert.Empty(read[1].Entries);
    }
}
