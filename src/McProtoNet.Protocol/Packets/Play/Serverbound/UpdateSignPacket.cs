using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UpdateSign", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2A)]
[PacketId(751, 758, 0x2B)]
[PacketId(759, 759, 0x2D)]
[PacketId(760, 763, 0x2E)]
[PacketId(764, 764, 0x31)]
[PacketId(765, 765, 0x32)]
[PacketId(766, 767, 0x35)]
[PacketId(768, 768, 0x37)]
[PacketId(769, 769, 0x39)]
[PacketId(770, 770, 0x3A)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x3B)]
public sealed partial class UpdateSignPacket : IClientPacket
{
    public Position Location { get; set; }
    public string Text1 { get; set; }
    public string Text2 { get; set; }
    public string Text3 { get; set; }
    public string Text4 { get; set; }
    public bool? IsFrontText { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteString(Text1);
                writer.WriteString(Text2);
                writer.WriteString(Text3);
                writer.WriteString(Text4);
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteString(Text1);
                writer.WriteString(Text2);
                writer.WriteString(Text3);
                writer.WriteString(Text4);
                writer.WriteBoolean(IsFrontText ?? false);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UpdateSignPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                Location = reader.ReadType<Position>(protocolVersion);
                Text1 = reader.ReadString();
                Text2 = reader.ReadString();
                Text3 = reader.ReadString();
                Text4 = reader.ReadString();
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                Location = reader.ReadType<Position>(protocolVersion);
                Text1 = reader.ReadString();
                Text2 = reader.ReadString();
                Text3 = reader.ReadString();
                Text4 = reader.ReadString();
                IsFrontText = reader.ReadBoolean();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UpdateSignPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}