using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct SpectatePacket(Guid Target) : IProtocolType<SpectatePacket>
{
    public static SpectatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectatePacket>(protocolVersion);
        var target = reader.ReadUUID();
        return new SpectatePacket(target);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpectatePacket>(protocolVersion);
        writer.WriteUUID(Target);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x2C;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x2D;
        if (protocolVersion >= 755 && protocolVersion <= 758)
            return 0x2D;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x2F;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x30;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x30;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x30;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x33;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x34;
        if (protocolVersion >= 766 && protocolVersion <= 767)
            return 0x37;
        if (protocolVersion >= 768 && protocolVersion <= 768)
            return 0x39;
        if (protocolVersion >= 769 && protocolVersion <= 769)
            return 0x3B;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x3D;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x3D;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
