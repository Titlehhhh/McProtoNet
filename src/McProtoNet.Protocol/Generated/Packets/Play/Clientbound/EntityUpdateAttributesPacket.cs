using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_update_attributes", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Properties", "EntityAttribute[]")]
public sealed partial record EntityUpdateAttributesPacket(int EntityId, EntityAttribute[] Properties) : IPacket<EntityUpdateAttributesPacket>, IPacket
{
    public static EntityUpdateAttributesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityUpdateAttributesPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var entityId = reader.ReadVarInt();
            int propertiesCount = checked((int)reader.ReadSignedInt());
            var properties = new EntityAttribute[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<EntityAttribute>(protocolVersion);
            return new EntityUpdateAttributesPacket(entityId, properties);
        }

        if (protocolVersion >= 755)
        {
            var entityId = reader.ReadVarInt();
            int propertiesCount = reader.ReadVarInt();
            var properties = new EntityAttribute[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<EntityAttribute>(protocolVersion);
            return new EntityUpdateAttributesPacket(entityId, properties);
        }

        throw new System.NotSupportedException($"EntityUpdateAttributesPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityUpdateAttributesPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteSignedInt((int)Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<EntityAttribute>(propertiesItem, protocolVersion);
            return;
        }

        if (protocolVersion >= 755)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteVarInt(Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<EntityAttribute>(propertiesItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"EntityUpdateAttributesPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.entity_update_attributes", "EntityUpdateAttributes", PacketPhase.Play, PacketDirection.Clientbound, 39);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            id = 0x64;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x65;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x68;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x6A;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x6D;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x71;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x75;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x7C;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x81;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x83;
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
