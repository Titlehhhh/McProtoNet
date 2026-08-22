using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace McProtoNet.Tests.Protocol;

/// <summary>
/// The "registry id or inline definition" wire shape protodef calls a holder: a varint where 0
/// means an inline payload follows and n &gt; 0 means registry entry n - 1. The off-by-one is the
/// whole point of the type, so the tests pin the id offset, the n = 0 boundary, and that a payload
/// read still works when the bytes arrive in arbitrary chunks.
/// </summary>
public class RegistryOrInlineTests
{
    private sealed record SoundLike(string Name, float? Range) : IProtocolType<SoundLike>
    {
        public static SoundLike Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            var name = reader.ReadString();
            var hasRange = reader.ReadBoolean();
            return new SoundLike(name, hasRange ? reader.ReadFloat() : null);
        }

        public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            writer.WriteString(Name);
            writer.WriteBoolean(Range.HasValue);
            if (Range.HasValue) writer.WriteFloat(Range.Value);
        }
    }

    private static byte[] Write(RegistryOrInline<SoundLike> value, int pv = 772)
    {
        var writer = new MinecraftPrimitiveWriter();
        value.Write(writer, pv);
        using var mem = writer.GetWrittenMemory();
        return mem.Memory.ToArray();
    }

    private static RegistryOrInline<SoundLike> Read(byte[] bytes, int pv = 772)
    {
        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(bytes));
        return RegistryOrInline<SoundLike>.Read(ref reader, pv);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(int.MaxValue - 1)]
    public void Registry_RoundTrips(int id)
    {
        var back = Read(Write(RegistryOrInline<SoundLike>.FromRegistry(id)));

        Assert.True(back.IsRegistry);
        Assert.False(back.IsInline);
        Assert.Equal(id, back.Id);
        Assert.True(back.TryGetId(out var got));
        Assert.Equal(id, got);
        Assert.False(back.TryGetValue(out _));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(16f)]
    public void Inline_RoundTrips(object? range)
    {
        var value = new SoundLike("minecraft:entity.pig.ambient", (float?)range);
        var back = Read(Write(RegistryOrInline<SoundLike>.Inline(value)));

        Assert.True(back.IsInline);
        Assert.False(back.IsRegistry);
        Assert.Equal(value, back.Value);
        Assert.True(back.TryGetValue(out var got));
        Assert.Equal(value, got);
        Assert.False(back.TryGetId(out _));
    }

    [Fact]
    public void Registry_IsWrittenWithIdOffsetByOne()
    {
        Assert.Equal(new byte[] { 1 }, Write(RegistryOrInline<SoundLike>.FromRegistry(0)));
        Assert.Equal(new byte[] { 2 }, Write(RegistryOrInline<SoundLike>.FromRegistry(1)));
        Assert.Equal(new byte[] { 0x80, 0x01 }, Write(RegistryOrInline<SoundLike>.FromRegistry(127)));
    }

    [Fact]
    public void Inline_IsWrittenAsZeroThenPayload()
    {
        var bytes = Write(RegistryOrInline<SoundLike>.Inline(new SoundLike("a", null)));

        Assert.Equal(new byte[] { 0, 1, (byte)'a', 0 }, bytes);
    }

    [Fact]
    public void ZeroDiscriminator_ReadsInline_NotRegistryZero()
    {
        var back = Read(new byte[] { 0, 1, (byte)'a', 0 });

        Assert.True(back.IsInline);
        Assert.Equal(new SoundLike("a", null), back.Value);
    }

    [Fact]
    public void OneDiscriminator_ReadsRegistryZero_AndConsumesNoPayload()
    {
        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(new byte[] { 1, 42 }));
        var back = RegistryOrInline<SoundLike>.Read(ref reader, 772);

        Assert.Equal(0, back.Id);
        Assert.Equal(42, reader.ReadUnsignedByte());
    }

    [Fact]
    public void Accessors_ThrowOnTheWrongArm()
    {
        var registry = RegistryOrInline<SoundLike>.FromRegistry(3);
        var inline = RegistryOrInline<SoundLike>.Inline(new SoundLike("a", null));

        Assert.Throws<InvalidOperationException>(() => registry.Value);
        Assert.Throws<InvalidOperationException>(() => inline.Id);
    }

    [Fact]
    public void FromRegistry_RejectsNegativeId()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RegistryOrInline<SoundLike>.FromRegistry(-1));
    }

    [Fact]
    public void Inline_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => RegistryOrInline<SoundLike>.Inline(null!));
    }

    [Fact]
    public void Default_IsNeitherArm_AndRefusesToWrite()
    {
        var writer = new MinecraftPrimitiveWriter();

        Assert.Throws<InvalidOperationException>(
            () => default(RegistryOrInline<SoundLike>).Write(writer, 772));
    }

    [Fact]
    public void Equality_SeparatesTheArms()
    {
        Assert.Equal(RegistryOrInline<SoundLike>.FromRegistry(5), RegistryOrInline<SoundLike>.FromRegistry(5));
        Assert.NotEqual(RegistryOrInline<SoundLike>.FromRegistry(5), RegistryOrInline<SoundLike>.FromRegistry(6));
        Assert.NotEqual(
            RegistryOrInline<SoundLike>.FromRegistry(0),
            RegistryOrInline<SoundLike>.Inline(new SoundLike("a", null)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(11)]
    public void Read_SurvivesRandomLengthSegments(int seed)
    {
        var value = RegistryOrInline<SoundLike>.Inline(
            new SoundLike("minecraft:block.amethyst_block.chime", 12.5f));

        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(7);
        value.Write(writer, 772);
        writer.WriteVarInt(9);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(RandomChunks(mem.Memory.ToArray(), seed));
        Assert.Equal(7, reader.ReadVarInt());
        var back = RegistryOrInline<SoundLike>.Read(ref reader, 772);
        Assert.Equal(9, reader.ReadVarInt());
        Assert.Equal(value, back);
    }

    // The first delivered user of the holder: protodef's registryEntryHolder over ItemSoundEvent
    // (ExplosionPacket.sound, 765+). Proves the generated payload type travels through the
    // hand-written holder without a wrapper object in between.
    [Theory]
    [InlineData(761)]
    [InlineData(772)]
    public void GeneratedItemSoundEvent_TravelsThroughBothArms(int pv)
    {
        var sound = new ItemSoundEvent("minecraft:entity.pig.ambient", 16f);

        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(3);
        writer.WriteType(RegistryOrInline<ItemSoundEvent>.Inline(sound), pv);
        writer.WriteType(RegistryOrInline<ItemSoundEvent>.FromRegistry(11), pv);
        using var mem = writer.GetWrittenMemory();

        var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(mem.Memory));
        Assert.Equal(3, reader.ReadVarInt());

        var inline = reader.ReadType<RegistryOrInline<ItemSoundEvent>>(pv);
        Assert.True(inline.IsInline);
        Assert.Equal("minecraft:entity.pig.ambient", inline.Value.SoundName);
        Assert.Equal(16f, inline.Value.FixedRange);

        var registry = reader.ReadType<RegistryOrInline<ItemSoundEvent>>(pv);
        Assert.True(registry.IsRegistry);
        Assert.Equal(11, registry.Id);
    }

    [Fact]
    public void GeneratedItemSoundEvent_RefusesVersionsBeforeItExists()
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(0);
        using var mem = writer.GetWrittenMemory();
        var bytes = mem.Memory.ToArray();

        Assert.Throws<ProtocolNotSupportException>(() =>
        {
            var reader = new MinecraftPrimitiveReader(new ReadOnlySequence<byte>(bytes));
            reader.ReadType<RegistryOrInline<ItemSoundEvent>>(760);
        });
    }

    private static ReadOnlySequence<byte> RandomChunks(byte[] data, int seed)
    {
        var rng = new Random(seed);
        Seg? first = null, prev = null;
        long pos = 0;
        var offset = 0;
        while (offset < data.Length)
        {
            var take = Math.Min(rng.Next(1, 5), data.Length - offset);
            var seg = new Seg(data.AsMemory(offset, take), pos);
            first ??= seg;
            prev?.SetNext(seg);
            prev = seg;
            pos += take;
            offset += take;
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
