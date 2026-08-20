using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_velocity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("VelocityX", "int", Group = "VUntil772", To = 772)]
[PacketField("VelocityY", "int", Group = "VUntil772", To = 772)]
[PacketField("VelocityZ", "int", Group = "VUntil772", To = 772)]
[PacketField("Velocity", "LpVec3", Group = "V773_Last", From = 773)]
public sealed partial record EntityVelocityPacket(int EntityId, EntityVelocityPacket.VUntil772Layer? VUntil772 = null, EntityVelocityPacket.V773_LastLayer? V773_Last = null) : IPacket<EntityVelocityPacket>, IPacket
{
    public readonly record struct VUntil772Layer(int VelocityX, int VelocityY, int VelocityZ);
    public readonly record struct V773_LastLayer(LpVec3 Velocity);
    public static EntityVelocityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityVelocityPacket>(protocolVersion);
        if (protocolVersion <= 772)
        {
            var entityId = reader.ReadVarInt();
            var velocityX = reader.ReadSignedShort();
            var velocityY = reader.ReadSignedShort();
            var velocityZ = reader.ReadSignedShort();
            return new EntityVelocityPacket(entityId, VUntil772: new VUntil772Layer(velocityX, velocityY, velocityZ));
        }

        if (protocolVersion >= 773)
        {
            var entityId = reader.ReadVarInt();
            var velocity = reader.ReadType<LpVec3>(protocolVersion);
            return new EntityVelocityPacket(entityId, V773_Last: new V773_LastLayer(velocity));
        }

        throw new System.NotSupportedException($"EntityVelocityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityVelocityPacket>(protocolVersion);
        if (protocolVersion <= 772)
        {
            var layer = VUntil772 ?? throw new WrongLayerException("EntityVelocityPacket", protocolVersion, "VUntil772");
            int VelocityX = layer.VelocityX;
            int VelocityY = layer.VelocityY;
            int VelocityZ = layer.VelocityZ;
            writer.WriteVarInt(EntityId);
            writer.WriteSignedShort((short)VelocityX);
            writer.WriteSignedShort((short)VelocityY);
            writer.WriteSignedShort((short)VelocityZ);
            return;
        }

        if (protocolVersion >= 773)
        {
            var layer = V773_Last ?? throw new WrongLayerException("EntityVelocityPacket", protocolVersion, "V773_Last");
            LpVec3 Velocity = layer.Velocity;
            writer.WriteVarInt(EntityId);
            writer.WriteType<LpVec3>(Velocity, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"EntityVelocityPacket has no wire layout for protocol version {protocolVersion}.");
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

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x65;
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
