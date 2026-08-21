using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
namespace McProtoNet.Tests.Protocol;

public class PacketIoTests
{
    // Until the generated packets become classes (form A), the core is exercised by a local
    // hand-written IPacket implementation; PacketIoTests switch to a real packet in Task 8.
    private sealed record FakePacket(long Value) : IPacket<FakePacket>
    {
        public static PacketIdentity Identity =>
            new("test.toClient.fake", "Fake", PacketPhase.Play, PacketDirection.Clientbound, 0);

        public static bool TryGetPacketId(int protocolVersion, out int id)
        {
            id = 0x42;
            return true;
        }

        public static FakePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
            => new(reader.ReadSignedLong());

        public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
            => writer.WriteSignedLong(Value);
    }

    private static IncomingPacket Raw(params byte[][] parts)
    {
        var all = parts.SelectMany(p => p).ToArray();
        return new IncomingPacket(0x42, new ReadOnlySequence<byte>(all));
    }

    private static byte[] Body(long value)
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteSignedLong(value);
        using var owner = writer.GetWrittenMemory();
        return owner.Memory.ToArray();
    }

    [Fact]
    public void TryDecode_ValidBody_ReturnsPacket()
    {
        var raw = Raw(Body(123456789L));

        var ok = PacketIo.TryDecode<FakePacket>(in raw, 772, out var packet, out var error);

        Assert.True(ok);
        Assert.Equal(DecodeError.None, error);
        Assert.Equal(123456789L, packet!.Value);
    }

    [Fact]
    public void TryDecode_TrailingBytes_Fails()
    {
        var raw = Raw(Body(1L), new byte[] { 0xAA });

        var ok = PacketIo.TryDecode<FakePacket>(in raw, 772, out var packet, out var error);

        Assert.False(ok);
        Assert.Null(packet);
        Assert.Equal(DecodeError.TrailingBytes, error);
    }

    [Fact]
    public void TryDecode_Underflow_IsMalformed()
    {
        var raw = Raw(new byte[] { 1, 2, 3, 4 });

        var ok = PacketIo.TryDecode<FakePacket>(in raw, 772, out _, out var error);

        Assert.False(ok);
        Assert.Equal(DecodeError.Malformed, error);
    }

    [Fact]
    public void Decode_Underflow_ThrowsWithError()
    {
        var raw = Raw(new byte[] { 1, 2, 3 });

        var e = Assert.Throws<PacketDecodeException>(() => PacketIo.Decode<FakePacket>(in raw, 772));

        Assert.Equal(DecodeError.Malformed, e.Error);
        Assert.Equal(typeof(FakePacket), e.PacketType);
    }

    [Fact]
    public void Decode_Valid_RoundTrips()
    {
        var raw = Raw(Body(-42L));

        var packet = PacketIo.Decode<FakePacket>(in raw, 772);

        Assert.Equal(-42L, packet.Value);
    }
}
