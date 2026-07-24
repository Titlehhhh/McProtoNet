using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class NameItemPacket : IProtocolType<NameItemPacket>
{
    public string Name { get; }

    public NameItemPacket(string name)
    {
        Name = name;
    }

    public static NameItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NameItemPacket>(protocolVersion);
        var name = reader.ReadString();
        return new NameItemPacket(name);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<NameItemPacket>(protocolVersion);
        writer.WriteString(Name);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x1F;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x20;
        if (protocolVersion >= 755 && protocolVersion <= 758)
            return 0x20;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x22;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x23;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x23;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x23;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x26;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x27;
        if (protocolVersion >= 766 && protocolVersion <= 767)
            return 0x2A;
        if (protocolVersion >= 768 && protocolVersion <= 768)
            return 0x2C;
        if (protocolVersion >= 769 && protocolVersion <= 769)
            return 0x2E;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x2E;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x2F;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
