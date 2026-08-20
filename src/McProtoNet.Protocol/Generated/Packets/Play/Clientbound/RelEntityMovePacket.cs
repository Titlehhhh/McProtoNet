using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.rel_entity_move", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Dx", "int")]
[PacketField("Dy", "int")]
[PacketField("Dz", "int")]
[PacketField("OnGround", "bool")]
public sealed partial record RelEntityMovePacket(int EntityId, int Dx, int Dy, int Dz, bool OnGround) : IPacket<RelEntityMovePacket>, IPacket
{
    public static RelEntityMovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RelEntityMovePacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var dx = reader.ReadSignedShort();
        var dy = reader.ReadSignedShort();
        var dz = reader.ReadSignedShort();
        var onGround = reader.ReadBoolean();
        return new RelEntityMovePacket(entityId, dx, dy, dz, onGround);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RelEntityMovePacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort((short)Dx);
        writer.WriteSignedShort((short)Dy);
        writer.WriteSignedShort((short)Dz);
        writer.WriteBoolean(OnGround);
    }

    public static PacketIdentity Identity => new("play.toClient.rel_entity_move", "RelEntityMove", PacketPhase.Play, PacketDirection.Clientbound, 71);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x35;
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
