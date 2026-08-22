using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.test_instance_block_status", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Status", "NbtTag")]
[PacketField("Size", "Vec3i?")]
public sealed partial record TestInstanceBlockStatusPacket(NbtTag Status, Vec3i? Size) : IPacket<TestInstanceBlockStatusPacket>, IPacket
{
    public static TestInstanceBlockStatusPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockStatusPacket>(protocolVersion);
        var status = reader.ReadNbtTag(false)!;
        Vec3i? size = null;
        if (reader.ReadBoolean())
            size = reader.ReadType<Vec3i>(protocolVersion);
        return new TestInstanceBlockStatusPacket(status, size);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TestInstanceBlockStatusPacket>(protocolVersion);
        writer.WriteNbt(Status);
        writer.WriteBoolean(Size is not null);
        if (Size is { } sizeValue)
            writer.WriteType<Vec3i>(sizeValue, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.test_instance_block_status", "TestInstanceBlockStatus", PacketPhase.Play, PacketDirection.Clientbound, 105);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x77;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x7C;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x7E;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
