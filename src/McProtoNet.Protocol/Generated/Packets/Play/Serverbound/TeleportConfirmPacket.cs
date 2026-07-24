using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct TeleportConfirmPacket(int TeleportId) : IProtocolType<TeleportConfirmPacket>
{
    public static TeleportConfirmPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        var teleportId = reader.ReadVarInt();
        return new TeleportConfirmPacket(teleportId);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        writer.WriteVarInt(TeleportId);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x00;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x00;
        if (protocolVersion >= 755 && protocolVersion <= 758)
            return 0x00;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x00;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x00;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x00;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x00;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x00;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x00;
        if (protocolVersion >= 766 && protocolVersion <= 767)
            return 0x00;
        if (protocolVersion >= 768 && protocolVersion <= 768)
            return 0x00;
        if (protocolVersion >= 769 && protocolVersion <= 769)
            return 0x00;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x00;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x00;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
