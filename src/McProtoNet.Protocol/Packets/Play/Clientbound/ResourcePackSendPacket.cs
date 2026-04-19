using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ResourcePackSend", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 764)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x39)]
[PacketId(751, 754, 0x38)]
[PacketId(755, 758, 0x3C)]
[PacketId(759, 759, 0x3A)]
[PacketId(760, 760, 0x3D)]
[PacketId(761, 761, 0x3C)]
[PacketId(762, 763, 0x40)]
[PacketId(764, 764, 0x42)]
public sealed partial class ResourcePackSendPacket : IServerPacket
{
    public string Url { get; set; }
    public string Hash { get; set; }
    public bool? Forced { get; set; }
    public string? PromptMessage { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                writer.WriteString(Url);
                writer.WriteString(Hash);
                return;
            }
            case >= 755 and <= 764:
            {
                writer.WriteString(Url);
                writer.WriteString(Hash);
                writer.WriteBoolean(Forced.Value);
                writer.WriteString(PromptMessage!);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ResourcePackSendPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
                return;
            }
            case >= 755 and <= 764:
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
                Forced = reader.ReadBoolean();
                PromptMessage = reader.ReadString();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ResourcePackSendPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}