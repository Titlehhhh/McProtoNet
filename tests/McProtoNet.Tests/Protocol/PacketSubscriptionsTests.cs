using McProtoNet.Net;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Tests.Protocol;

public class PacketSubscriptionsTests
{
    // Local hand-written IPacket implementations, same shape as PacketIoTests' FakePacket;
    // Ordinal is deliberately non-adjacent (0 and 5) to exercise slot-array growth.
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

    private sealed record FakeOtherPacket(int Value) : IPacket<FakeOtherPacket>
    {
        public static PacketIdentity Identity =>
            new("test.toClient.fakeOther", "FakeOther", PacketPhase.Play, PacketDirection.Clientbound, 5);

        public static bool TryGetPacketId(int protocolVersion, out int id)
        {
            id = 0x43;
            return true;
        }

        public static FakeOtherPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
            => new(reader.ReadVarInt());

        public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
            => writer.WriteVarInt(Value);
    }

    [Fact]
    public void On_Then_Visit_InvokesHandlerWithSameInstance()
    {
        var subscriptions = new PacketSubscriptions();
        FakePacket? received = null;
        subscriptions.On<FakePacket>(p => received = p);

        var packet = new FakePacket(123456789L);
        subscriptions.Visit(packet);

        Assert.Same(packet, received);
    }

    [Fact]
    public void Visit_UnsubscribedType_DoesNotThrow()
    {
        var subscriptions = new PacketSubscriptions();

        var exception = Record.Exception(() => subscriptions.Visit(new FakePacket(1L)));

        Assert.Null(exception);
    }

    [Fact]
    public void Visit_OrdinalBeyondSlotArray_DoesNotThrow()
    {
        var subscriptions = new PacketSubscriptions();
        // Subscribing only to the low-ordinal type leaves the slot array shorter than
        // FakeOtherPacket's Ordinal (5), exercising the out-of-range guard in Visit.
        subscriptions.On<FakePacket>(_ => { });

        var exception = Record.Exception(() => subscriptions.Visit(new FakeOtherPacket(7)));

        Assert.Null(exception);
    }

    [Fact]
    public void On_SameTypeTwice_ReplacesHandler()
    {
        var subscriptions = new PacketSubscriptions();
        var firstCalled = false;
        var secondCalled = false;
        subscriptions.On<FakePacket>(_ => firstCalled = true);
        subscriptions.On<FakePacket>(_ => secondCalled = true);

        subscriptions.Visit(new FakePacket(1L));

        Assert.False(firstCalled);
        Assert.True(secondCalled);
    }

    [Fact]
    public void Unknown_DoesNotThrow()
    {
        var subscriptions = new PacketSubscriptions();
        var raw = new InputPacket(0x99, new System.Buffers.ReadOnlySequence<byte>(Array.Empty<byte>()));

        var exception = Record.Exception(() => subscriptions.Unknown(in raw));

        Assert.Null(exception);
    }
}
