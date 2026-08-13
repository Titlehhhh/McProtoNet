using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_velocity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("VelocityX", "int")]
[PacketField("VelocityY", "int")]
[PacketField("VelocityZ", "int")]
public sealed partial record EntityVelocityPacket(int EntityId, int VelocityX, int VelocityY, int VelocityZ) : IPacket<EntityVelocityPacket>, IPacket
{
    public static EntityVelocityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityVelocityPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var velocityX = reader.ReadSignedShort();
        var velocityY = reader.ReadSignedShort();
        var velocityZ = reader.ReadSignedShort();
        return new EntityVelocityPacket(entityId, velocityX, velocityY, velocityZ);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityVelocityPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort((short)VelocityX);
        writer.WriteSignedShort((short)VelocityY);
        writer.WriteSignedShort((short)VelocityZ);
    }

    public static PacketIdentity Identity => new("play.toClient.entity_velocity", "EntityVelocity", PacketPhase.Play, PacketDirection.Clientbound, 40);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x46;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x46;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 759)
        {
            id = 0x4F;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x52;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x50;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x54;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x56;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x5A;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x5E;
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
