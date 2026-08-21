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

    public static PacketIdentity Identity => new("play.toClient.entity_destroy", "EntityDestroy", PacketPhase.Play, PacketDirection.Clientbound, 32);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 756 && protocolVersion <= 758)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x3E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x47;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x46;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x4B;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x4D;
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
