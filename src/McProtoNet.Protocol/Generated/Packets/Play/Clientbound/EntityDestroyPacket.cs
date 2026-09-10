using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_destroy", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityIds", "int[]")]
public sealed partial record EntityDestroyPacket(int[] EntityIds) : IPacket<EntityDestroyPacket>, IPacket
{
    public static EntityDestroyPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityDestroyPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            int entityIdsCount = reader.ReadVarInt();
            var entityIds = new int[entityIdsCount];
            for (int i = 0; i < entityIds.Length; i++)
                entityIds[i] = reader.ReadVarInt();
            return new EntityDestroyPacket(entityIds);
        }

        if (protocolVersion >= 756)
        {
            int entityIdsCount = reader.ReadVarInt();
            var entityIds = new int[entityIdsCount];
            for (int i = 0; i < entityIds.Length; i++)
                entityIds[i] = reader.ReadVarInt();
            return new EntityDestroyPacket(entityIds);
        }

        throw new System.NotSupportedException($"EntityDestroyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityDestroyPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            writer.WriteVarInt(EntityIds.Length);
            foreach (var entityIdsItem in EntityIds)
                writer.WriteVarInt(entityIdsItem);
            return;
        }

        if (protocolVersion >= 756)
        {
            writer.WriteVarInt(EntityIds.Length);
            foreach (var entityIdsItem in EntityIds)
                writer.WriteVarInt(entityIdsItem);
            return;
        }

        throw new System.NotSupportedException($"EntityDestroyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.entity_destroy", "EntityDestroy", PacketPhase.Play, PacketDirection.Clientbound, 34);

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
