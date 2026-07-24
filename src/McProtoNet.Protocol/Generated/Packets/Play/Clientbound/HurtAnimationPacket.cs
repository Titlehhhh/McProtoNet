using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
public readonly partial record struct HurtAnimationPacket(int EntityId, float Yaw) : IProtocolType<HurtAnimationPacket>
{
    public static HurtAnimationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HurtAnimationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var yaw = reader.ReadFloat();
        return new HurtAnimationPacket(entityId, yaw);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HurtAnimationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteFloat(Yaw);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x21;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x22;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x22;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x24;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x24;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x25;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x24;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x24;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
