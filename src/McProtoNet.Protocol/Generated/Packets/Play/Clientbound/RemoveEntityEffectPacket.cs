using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.remove_entity_effect", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EffectId", "int")]
public sealed partial record RemoveEntityEffectPacket(int EntityId, int EffectId) : IPacket<RemoveEntityEffectPacket>, IPacket
{
    public static RemoveEntityEffectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RemoveEntityEffectPacket>(protocolVersion);
        if (protocolVersion <= 757)
        {
            var entityId = reader.ReadVarInt();
            var effectId = reader.ReadSignedByte();
            return new RemoveEntityEffectPacket(entityId, effectId);
        }

        if (protocolVersion >= 758)
        {
            var entityId = reader.ReadVarInt();
            var effectId = reader.ReadVarInt();
            return new RemoveEntityEffectPacket(entityId, effectId);
        }

        throw new System.NotSupportedException($"RemoveEntityEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RemoveEntityEffectPacket>(protocolVersion);
        if (protocolVersion <= 757)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteSignedByte((sbyte)EffectId);
            return;
        }

        if (protocolVersion >= 758)
        {
            writer.WriteVarInt(EntityId);
            writer.WriteVarInt(EffectId);
            return;
        }

        throw new System.NotSupportedException($"RemoveEntityEffectPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.remove_entity_effect", "RemoveEntityEffect", PacketPhase.Play, PacketDirection.Clientbound, 72);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x41;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x48;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x47;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x4E;
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
