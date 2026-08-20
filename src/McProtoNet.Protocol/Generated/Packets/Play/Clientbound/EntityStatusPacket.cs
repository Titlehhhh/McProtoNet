using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_status", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EntityStatus", "int")]
public sealed partial record EntityStatusPacket(int EntityId, int EntityStatus) : IPacket<EntityStatusPacket>, IPacket
{
    public static EntityStatusPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityStatusPacket>(protocolVersion);
        var entityId = reader.ReadSignedInt();
        var entityStatus = reader.ReadSignedByte();
        return new EntityStatusPacket(entityId, entityStatus);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityStatusPacket>(protocolVersion);
        writer.WriteSignedInt(EntityId);
        writer.WriteSignedByte((sbyte)EntityStatus);
    }

    public static PacketIdentity Identity => new("play.toClient.entity_status", "EntityStatus", PacketPhase.Play, PacketDirection.Clientbound, 37);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x22;
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
