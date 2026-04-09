using System.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Tests.Serialization;

/// <summary>
/// Roundtrip tests for all MinecraftPrimitiveWriter / MinecraftPrimitiveReader primitive methods.
/// </summary>
public class PrimitiveRoundtripTests
{
    // Helper: write via action, return a reader over the written bytes (zero-copy, no pool)
    private static MinecraftPrimitiveReader Rdr(Action<MinecraftPrimitiveWriter> write)
    {
        var w = new MinecraftPrimitiveWriter();
        write(w);
        return new MinecraftPrimitiveReader(w.WrittenMemory);
    }

    // ── Boolean ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Boolean_Roundtrip(bool value)
        => Assert.Equal(value, Rdr(w => w.WriteBoolean(value)).ReadBoolean());

    // ── Unsigned Byte ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)1)]
    [InlineData((byte)127)]
    [InlineData((byte)255)]
    public void UnsignedByte_Roundtrip(byte value)
        => Assert.Equal(value, Rdr(w => w.WriteUnsignedByte(value)).ReadUnsignedByte());

    // ── Signed Byte ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData((sbyte)-128)]
    [InlineData((sbyte)0)]
    [InlineData((sbyte)127)]
    public void SignedByte_Roundtrip(sbyte value)
        => Assert.Equal(value, Rdr(w => w.WriteSignedByte(value)).ReadSignedByte());

    // ── Unsigned Short ────────────────────────────────────────────────────────

    [Theory]
    [InlineData((ushort)0)]
    [InlineData((ushort)1)]
    [InlineData((ushort)32767)]
    [InlineData(ushort.MaxValue)]
    public void UnsignedShort_Roundtrip(ushort value)
        => Assert.Equal(value, Rdr(w => w.WriteUnsignedShort(value)).ReadUnsignedShort());

    // ── Signed Short ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(short.MinValue)]
    [InlineData((short)-1)]
    [InlineData((short)0)]
    [InlineData((short)1)]
    [InlineData(short.MaxValue)]
    public void SignedShort_Roundtrip(short value)
        => Assert.Equal(value, Rdr(w => w.WriteSignedShort(value)).ReadSignedShort());

    // ── Signed Int ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(int.MinValue)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void SignedInt_Roundtrip(int value)
        => Assert.Equal(value, Rdr(w => w.WriteSignedInt(value)).ReadSignedInt());

    // ── Unsigned Int ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0u)]
    [InlineData(1u)]
    [InlineData(uint.MaxValue)]
    public void UnsignedInt_Roundtrip(uint value)
        => Assert.Equal(value, Rdr(w => w.WriteUnsignedInt(value)).ReadUnsignedInt());

    // ── Signed Long ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(-1L)]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(long.MaxValue)]
    public void SignedLong_Roundtrip(long value)
        => Assert.Equal(value, Rdr(w => w.WriteSignedLong(value)).ReadSignedLong());

    // ── Unsigned Long ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0ul)]
    [InlineData(1ul)]
    [InlineData(ulong.MaxValue)]
    public void UnsignedLong_Roundtrip(ulong value)
        => Assert.Equal(value, Rdr(w => w.WriteUnsignedLong(value)).ReadUnsignedLong());

    // ── Float ─────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0f)]
    [InlineData(1.5f)]
    [InlineData(-1.5f)]
    [InlineData(float.MaxValue)]
    [InlineData(float.MinValue)]
    [InlineData(float.Epsilon)]
    public void Float_Roundtrip(float value)
        => Assert.Equal(value, Rdr(w => w.WriteFloat(value)).ReadFloat());

    // ── Double ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.5)]
    [InlineData(-1.5)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(double.Epsilon)]
    public void Double_Roundtrip(double value)
        => Assert.Equal(value, Rdr(w => w.WriteDouble(value)).ReadDouble());

    // ── VarInt ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(128)]       // first 2-byte varint
    [InlineData(255)]
    [InlineData(256)]
    [InlineData(16383)]     // last 2-byte varint
    [InlineData(16384)]     // first 3-byte varint
    [InlineData(2097151)]   // last 3-byte varint
    [InlineData(2097152)]   // first 4-byte varint
    [InlineData(268435455)] // last 4-byte varint
    [InlineData(int.MaxValue)]
    public void VarInt_Roundtrip(int value)
        => Assert.Equal(value, Rdr(w => w.WriteVarInt(value)).ReadVarInt());

    [Fact]
    public void VarInt_AllValues_0_to_1000()
    {
        for (int v = 0; v <= 1000; v++)
            Assert.Equal(v, Rdr(w => w.WriteVarInt(v)).ReadVarInt());
    }

    // ── VarLong ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(127L)]
    [InlineData(128L)]
    [InlineData(long.MaxValue)]
    public void VarLong_Roundtrip(long value)
        => Assert.Equal(value, Rdr(w => w.WriteVarLong(value)).ReadVarLong());

    // ── String ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("hello")]
    [InlineData("Minecraft")]
    [InlineData("Привет мир")]
    [InlineData("日本語テスト")]
    [InlineData("A string with special chars: \t\n\r")]
    public void String_Roundtrip(string value)
        => Assert.Equal(value, Rdr(w => w.WriteString(value)).ReadString());

    [Fact]
    public void String_Long_Roundtrip()
    {
        var s = new string('x', 2048);
        Assert.Equal(s, Rdr(w => w.WriteString(s)).ReadString());
    }

    // ── UUID ──────────────────────────────────────────────────────────────────

    [Fact]
    public void UUID_Roundtrip()
    {
        var guid = Guid.NewGuid();
        Assert.Equal(guid, Rdr(w => w.WriteUUID(guid)).ReadUUID());
    }

    [Fact]
    public void UUID_Empty_Roundtrip()
        => Assert.Equal(Guid.Empty, Rdr(w => w.WriteUUID(Guid.Empty)).ReadUUID());

    [Fact]
    public void UUID_AllOnes_Roundtrip()
    {
        var all = new Guid(new byte[16].Select(_ => (byte)0xFF).ToArray());
        Assert.Equal(all, Rdr(w => w.WriteUUID(all)).ReadUUID());
    }

    // ── Buffer ────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1024)]
    [InlineData(65535)]
    public void Buffer_Roundtrip(int length)
    {
        var data = new byte[length];
        new Random(42).NextBytes(data);
        Assert.Equal(data, Rdr(w => w.WriteBuffer(data)).ReadBuffer(length));
    }

    [Fact]
    public void ReadRestBuffer_Roundtrip()
    {
        byte[] data = [1, 2, 3, 4, 5];
        var r = Rdr(w => w.WriteBuffer(data));
        Assert.Equal(data, r.ReadRestBuffer());
    }

    // ── Written span / memory properties ─────────────────────────────────────

    [Fact]
    public void WrittenSpan_Length_IsCorrect()
    {
        var w = new MinecraftPrimitiveWriter();
        w.WriteVarInt(300);   // 2 bytes
        w.WriteBoolean(true); // 1 byte
        Assert.Equal(3, w.WrittenSpan.Length);
        Assert.Equal(3, w.WrittenMemory.Length);
    }

    [Fact]
    public void GetWrittenMemory_ReturnsPooledCopy_And_IsDisposable()
    {
        var w = new MinecraftPrimitiveWriter();
        w.WriteSignedInt(0x12345678);
        using var mem = w.GetWrittenMemory();
        Assert.Equal(4, mem.Length);
        var r = new MinecraftPrimitiveReader(mem.Memory);
        Assert.Equal(0x12345678, r.ReadSignedInt());
    }

    // ── Multi-field sequence (simulate packet) ────────────────────────────────

    [Fact]
    public void MultiField_Sequence_Roundtrip()
    {
        var w = new MinecraftPrimitiveWriter();
        w.WriteVarInt(42);
        w.WriteBoolean(true);
        w.WriteString("hello world");
        w.WriteSignedLong(-9_999_999_999L);
        w.WriteFloat(3.14f);
        w.WriteDouble(2.71828);
        w.WriteUUID(new Guid("12345678-1234-1234-1234-123456789abc"));
        w.WriteSignedShort(-300);
        w.WriteUnsignedByte(200);

        var r = new MinecraftPrimitiveReader(w.WrittenMemory);
        Assert.Equal(42, r.ReadVarInt());
        Assert.True(r.ReadBoolean());
        Assert.Equal("hello world", r.ReadString());
        Assert.Equal(-9_999_999_999L, r.ReadSignedLong());
        Assert.Equal(3.14f, r.ReadFloat());
        Assert.Equal(2.71828, r.ReadDouble());
        Assert.Equal(new Guid("12345678-1234-1234-1234-123456789abc"), r.ReadUUID());
        Assert.Equal(-300, r.ReadSignedShort());
        Assert.Equal(200, r.ReadUnsignedByte());
    }

    // ── ReadOnlySequence (multi-segment) constructor ───────────────────────────

    [Fact]
    public void Reader_MultiSegmentSequence_Works()
    {
        var w = new MinecraftPrimitiveWriter();
        w.WriteVarInt(127);
        w.WriteVarInt(128);
        var bytes = w.WrittenSpan.ToArray();

        // Split into 1-byte segments to force multi-segment path
        var seq = BuildMultiSegmentSequence(bytes, 1);
        var r = new MinecraftPrimitiveReader(seq);
        Assert.Equal(127, r.ReadVarInt());
        Assert.Equal(128, r.ReadVarInt());
    }

    private static ReadOnlySequence<byte> BuildMultiSegmentSequence(byte[] data, int segmentSize)
    {
        var segments = data.Chunk(segmentSize)
                           .Select(c => new ReadOnlyMemory<byte>(c))
                           .ToList();
        if (segments.Count == 1) return new ReadOnlySequence<byte>(segments[0]);

        Seg? first = null, prev = null;
        long pos = 0;
        foreach (var mem in segments)
        {
            var seg = new Seg(mem, pos);
            if (first == null) first = seg;
            prev?.SetNext(seg);
            prev = seg;
            pos += mem.Length;
        }
        return new ReadOnlySequence<byte>(first!, 0, prev!, prev!.Memory.Length);
    }

    private sealed class Seg : ReadOnlySequenceSegment<byte>
    {
        public Seg(ReadOnlyMemory<byte> mem, long runningIndex)
        {
            Memory = mem;
            RunningIndex = runningIndex;
        }
        public void SetNext(Seg next) => Next = next;
    }
}
