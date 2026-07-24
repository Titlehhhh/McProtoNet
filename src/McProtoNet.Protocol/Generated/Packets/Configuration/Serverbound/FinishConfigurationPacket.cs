using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
public readonly partial record struct FinishConfigurationPacket() : IProtocolType<FinishConfigurationPacket>
{
    public static FinishConfigurationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FinishConfigurationPacket>(protocolVersion);
        return new FinishConfigurationPacket();
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<FinishConfigurationPacket>(protocolVersion);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
            return 0x02;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x03;
        if (protocolVersion >= 767 && protocolVersion <= 770)
            return 0x03;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x03;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
