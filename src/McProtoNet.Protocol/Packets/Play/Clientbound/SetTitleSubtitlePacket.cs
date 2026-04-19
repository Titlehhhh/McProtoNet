using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetTitleSubtitle", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 756, 0x57)]
[PacketId(757, 759, 0x58)]
[PacketId(760, 760, 0x5B)]
[PacketId(761, 761, 0x59)]
[PacketId(762, 763, 0x5D)]
[PacketId(764, 764, 0x5F)]
[PacketId(765, 765, 0x61)]
[PacketId(766, 767, 0x63)]
[PacketId(768, 769, 0x6A)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x69)]
public sealed partial class SetTitleSubtitlePacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = V755_764 ?? throw new InvalidOperationException("SetTitleSubtitlePacket 755-764 fields missing.");
                writer.WriteString(fields.Text);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("SetTitleSubtitlePacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Text, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetTitleSubtitlePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                V755_764 = new V755_764Fields { Text = reader.ReadString() };
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields { Text = reader.ReadAnonymousNbtTag(protocolVersion) };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetTitleSubtitlePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public V755_764Fields? V755_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    public struct V755_764Fields { public string Text { get; set; } }
    public struct V765_LastFields { public NbtTag Text { get; set; } }
}