using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_move_look", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Dx", "int")]
[PacketField("Dy", "int")]
[PacketField("Dz", "int")]
[PacketField("Yaw", "int")]
[PacketField("Pitch", "int")]
[PacketField("OnGround", "bool")]
public sealed partial record EntityMoveLookPacket(int EntityId, int Dx, int Dy, int Dz, int Yaw, int Pitch, bool OnGround) : IPacket<EntityMoveLookPacket>, IPacket
{
    public static EntityMoveLookPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMoveLookPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var dx = reader.ReadSignedShort();
        var dy = reader.ReadSignedShort();
        var dz = reader.ReadSignedShort();
        var yaw = reader.ReadSignedByte();
        var pitch = reader.ReadSignedByte();
        var onGround = reader.ReadBoolean();
        return new EntityMoveLookPacket(entityId, dx, dy, dz, yaw, pitch, onGround);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMoveLookPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort((short)Dx);
        writer.WriteSignedShort((short)Dy);
        writer.WriteSignedShort((short)Dz);
        writer.WriteSignedByte((sbyte)Yaw);
        writer.WriteSignedByte((sbyte)Pitch);
        writer.WriteBoolean(OnGround);
    }

    public static PacketIdentity Identity => new("play.toClient.entity_move_look", "EntityMoveLook", PacketPhase.Play, PacketDirection.Clientbound, 36);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x36;
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
