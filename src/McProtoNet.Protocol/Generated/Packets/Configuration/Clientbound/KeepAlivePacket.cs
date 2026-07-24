using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
public readonly partial record struct KeepAlivePacket(long KeepAliveId) : IProtocolType<KeepAlivePacket>
{
    public static KeepAlivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KeepAlivePacket>(protocolVersion);
        var keepAliveId = reader.ReadSignedLong();
        return new KeepAlivePacket(keepAliveId);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KeepAlivePacket>(protocolVersion);
        writer.WriteSignedLong(KeepAliveId);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x03;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x03;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x04;
        if (protocolVersion >= 767 && protocolVersion <= 770)
            return 0x04;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x04;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
