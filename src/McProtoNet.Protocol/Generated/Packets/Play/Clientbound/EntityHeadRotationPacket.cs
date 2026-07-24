using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct EntityHeadRotationPacket(int EntityId, int HeadYaw) : IProtocolType<EntityHeadRotationPacket>
{
    public static EntityHeadRotationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityHeadRotationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var headYaw = reader.ReadSignedByte();
        return new EntityHeadRotationPacket(entityId, headYaw);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityHeadRotationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedByte((sbyte)HeadYaw);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x3B;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x3A;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x3E;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x3E;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x3E;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x3C;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x3F;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x3E;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x42;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x44;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x46;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x48;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x48;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x4D;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x4C;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x4C;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
