using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
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
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x20;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x1F;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x21;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x21;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x21;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x1E;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x20;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x1F;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x23;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x24;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x24;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x26;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x26;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x27;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x26;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x26;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
