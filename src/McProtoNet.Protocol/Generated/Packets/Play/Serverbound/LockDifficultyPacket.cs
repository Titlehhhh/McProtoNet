using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct LockDifficultyPacket(bool Locked) : IProtocolType<LockDifficultyPacket>
{
    public static LockDifficultyPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LockDifficultyPacket>(protocolVersion);
        var locked = reader.ReadBoolean();
        return new LockDifficultyPacket(locked);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LockDifficultyPacket>(protocolVersion);
        writer.WriteBoolean(Locked);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x11;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x11;
        if (protocolVersion >= 755 && protocolVersion <= 758)
            return 0x10;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x12;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x13;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x12;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x13;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x15;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x16;
        if (protocolVersion >= 766 && protocolVersion <= 767)
            return 0x19;
        if (protocolVersion >= 768 && protocolVersion <= 768)
            return 0x1B;
        if (protocolVersion >= 769 && protocolVersion <= 769)
            return 0x1B;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x1B;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x1C;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
