using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("NameItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1F)]
[PacketId(751, 758, 0x20)]
[PacketId(759, 759, 0x22)]
[PacketId(760, 763, 0x23)]
[PacketId(764, 764, 0x26)]
[PacketId(765, 765, 0x27)]
[PacketId(766, 767, 0x2A)]
[PacketId(768, 768, 0x2C)]
[PacketId(769, 770, 0x2E)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2F)]
public sealed partial class NameItemPacket : IClientPacket
{
    public string Name { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteString(Name);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(NameItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Name = reader.ReadString();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(NameItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}