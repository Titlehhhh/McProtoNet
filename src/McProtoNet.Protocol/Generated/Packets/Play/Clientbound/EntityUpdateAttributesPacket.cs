using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.entity_update_attributes", "EntityUpdateAttributes", PacketPhase.Play, PacketDirection.Clientbound, 41);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
