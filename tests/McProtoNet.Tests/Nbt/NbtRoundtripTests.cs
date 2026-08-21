using McProtoNet.NBT;
using McProtoNet.Primitives;
namespace McProtoNet.Tests.Nbt;

/// <summary>
/// Roundtrip tests for NBT serialization:
///   - NbtWriter (write) + NbtSpanReader (read)
///   - MinecraftPrimitiveWriter.WriteNbt + MinecraftPrimitiveReader.ReadNbtTag
/// </summary>
public class NbtRoundtripTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Write an NBT compound using NbtWriter, return the raw bytes.</summary>
    private static byte[] WriteCompound(string rootName, Action<NbtWriter> fill)
    {
        var ms = new MemoryStream();
        var w = new NbtWriter(ms, rootName);
        fill(w);
        w.Finish();
        return ms.ToArray();
    }

    /// <summary>Parse bytes with NbtSpanReader, read the root compound.</summary>
    private static NbtCompound ReadCompound(byte[] bytes)
    {
        var sr = new NbtSpanReader(bytes);
        var tag = sr.ReadAsTag<NbtCompound>(readRootName: true);
        Assert.NotNull(tag);
        return tag;
    }

    // ── Single-field tests ────────────────────────────────────────────────────

    [Fact]
    public void NbtInt_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteInt("val", 42));
        var c = ReadCompound(bytes);
        Assert.Equal(42, c.Get<NbtInt>("val")!.Value);
    }

    [Fact]
    public void NbtString_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteString("name", "Minecraft"));
        var c = ReadCompound(bytes);
        Assert.Equal("Minecraft", c.Get<NbtString>("name")!.Value);
    }

    [Fact]
    public void NbtByte_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteByte("b", 0xAB));
        var c = ReadCompound(bytes);
        Assert.Equal(0xAB, c.Get<NbtByte>("b")!.Value);
    }

    [Fact]
    public void NbtShort_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteShort("s", -1234));
        var c = ReadCompound(bytes);
        Assert.Equal(-1234, c.Get<NbtShort>("s")!.Value);
    }

    [Fact]
    public void NbtLong_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteLong("l", long.MaxValue));
        var c = ReadCompound(bytes);
        Assert.Equal(long.MaxValue, c.Get<NbtLong>("l")!.Value);
    }

    [Fact]
    public void NbtFloat_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteFloat("f", 3.14f));
        var c = ReadCompound(bytes);
        Assert.Equal(3.14f, c.Get<NbtFloat>("f")!.Value);
    }

    [Fact]
    public void NbtDouble_WriteRead_Roundtrip()
    {
        var bytes = WriteCompound("root", w => w.WriteDouble("d", 2.71828));
        var c = ReadCompound(bytes);
        Assert.Equal(2.71828, c.Get<NbtDouble>("d")!.Value);
    }

    // ── Multi-field compound ──────────────────────────────────────────────────

    [Fact]
    public void NbtCompound_MultipleFields_Roundtrip()
    {
        var bytes = WriteCompound("data", w =>
        {
            w.WriteInt("x", 100);
            w.WriteInt("y", -50);
            w.WriteDouble("health", 20.0);
            w.WriteString("name", "Player");
            w.WriteByte("flags", 0x07);
        });

        var c = ReadCompound(bytes);
        Assert.Equal(100,    c.Get<NbtInt>("x")!.Value);
        Assert.Equal(-50,    c.Get<NbtInt>("y")!.Value);
        Assert.Equal(20.0,   c.Get<NbtDouble>("health")!.Value);
        Assert.Equal("Player", c.Get<NbtString>("name")!.Value);
        Assert.Equal(0x07,   c.Get<NbtByte>("flags")!.Value);
    }

    // ── List ──────────────────────────────────────────────────────────────────

    [Fact]
    public void NbtList_Int_Roundtrip()
    {
        var bytes = WriteCompound("root", w =>
        {
            w.BeginList("items", NbtTagType.Int, 3);
            w.WriteInt(10);
            w.WriteInt(20);
            w.WriteInt(30);
            w.EndList();
        });

        var c = ReadCompound(bytes);
        var list = c.Get<NbtList>("items");
        Assert.NotNull(list);
        Assert.Equal(3, list.Count);
        Assert.Equal(10, list.Get<NbtInt>(0).Value);
        Assert.Equal(20, list.Get<NbtInt>(1).Value);
        Assert.Equal(30, list.Get<NbtInt>(2).Value);
    }

    [Fact]
    public void NbtList_String_Roundtrip()
    {
        var bytes = WriteCompound("root", w =>
        {
            w.BeginList("names", NbtTagType.String, 2);
            w.WriteString("Alice");
            w.WriteString("Bob");
            w.EndList();
        });

        var c = ReadCompound(bytes);
        var list = c.Get<NbtList>("names");
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
        Assert.Equal("Alice", list.Get<NbtString>(0).Value);
        Assert.Equal("Bob",   list.Get<NbtString>(1).Value);
    }

    // ── Nested compound ───────────────────────────────────────────────────────

    [Fact]
    public void NbtCompound_Nested_Roundtrip()
    {
        var bytes = WriteCompound("root", w =>
        {
            w.BeginCompound("inner");
            w.WriteInt("depth", 2);
            w.WriteString("label", "nested");
            w.EndCompound();
            w.WriteInt("outer", 99);
        });

        var c = ReadCompound(bytes);
        var inner = c.Get<NbtCompound>("inner");
        Assert.NotNull(inner);
        Assert.Equal(2,        inner.Get<NbtInt>("depth")!.Value);
        Assert.Equal("nested", inner.Get<NbtString>("label")!.Value);
        Assert.Equal(99,       c.Get<NbtInt>("outer")!.Value);
    }

    // ── MinecraftPrimitiveWriter.WriteNbt / Reader.ReadNbtTag ────────────────

    [Fact]
    public void PrimitiveWriter_WriteNbt_ReadNbt_Roundtrip()
    {
        // Build a compound using NbtWriter first to get valid bytes
        var raw = WriteCompound("", w =>
        {
            w.WriteInt("score", 9999);
            w.WriteString("player", "Steve");
        });

        // Parse it into an NbtTag object
        var sr = new NbtSpanReader(raw);
        var compound = sr.ReadAsTag<NbtCompound>(readRootName: true)!;

        // Roundtrip through MinecraftPrimitiveWriter/Reader
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteNbt(compound);

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var result = reader.ReadNbtTag(readRootTag: false) as NbtCompound;

        Assert.NotNull(result);
        Assert.Equal(9999,    result.Get<NbtInt>("score")!.Value);
        Assert.Equal("Steve", result.Get<NbtString>("player")!.Value);
    }

    [Fact]
    public void PrimitiveWriter_WriteOptionalNbt_Null_Roundtrip()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteOptionalNbt(null);

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var result = reader.ReadOptionalNbtTag(readRootTag: false);
        Assert.Null(result);
    }

    [Fact]
    public void PrimitiveWriter_WriteOptionalNbt_Value_Roundtrip()
    {
        var compound = new NbtCompound("c") { new NbtInt("v", 7) };

        var writer = new MinecraftPrimitiveWriter();
        writer.WriteOptionalNbt(compound);

        var reader = new MinecraftPrimitiveReader(writer.WrittenMemory);
        var result = reader.ReadOptionalNbtTag(readRootTag: false) as NbtCompound;

        Assert.NotNull(result);
        Assert.Equal(7, result.Get<NbtInt>("v")!.Value);
    }
}
