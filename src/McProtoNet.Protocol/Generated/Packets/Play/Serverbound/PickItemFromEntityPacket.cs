using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.pick_item_from_entity", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("EntityId", "int")]
[PacketField("IncludeData", "bool")]
public sealed partial record PickItemFromEntityPacket(int EntityId, bool IncludeData) : IPacket<PickItemFromEntityPacket>, IPacket
{
    public static PickItemFromEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemFromEntityPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var includeData = reader.ReadBoolean();
        return new PickItemFromEntityPacket(entityId, includeData);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PickItemFromEntityPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteBoolean(IncludeData);
    }

    public static PacketIdentity Identity => new("play.toServer.pick_item_from_entity", "PickItemFromEntity", PacketPhase.Play, PacketDirection.Serverbound, 33);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x25;
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
