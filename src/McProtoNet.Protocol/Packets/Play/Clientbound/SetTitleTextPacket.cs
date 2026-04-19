using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetTitleText", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 756, 0x59)]
[PacketId(757, 759, 0x5A)]
[PacketId(760, 760, 0x5D)]
[PacketId(761, 761, 0x5B)]
[PacketId(762, 763, 0x5F)]
[PacketId(764, 764, 0x61)]
[PacketId(765, 765, 0x63)]
[PacketId(766, 767, 0x65)]
[PacketId(768, 769, 0x6C)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x6B)]
public sealed partial class SetTitleTextPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                var fields = V755_764 ?? throw new InvalidOperationException("SetTitleTextPacket 755-764 fields missing.");
                writer.WriteString(fields.Text);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("SetTitleTextPacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Text, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetTitleTextPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                V755_764 = new V755_764Fields { Text = reader.ReadString() };
                V765_Last = null;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields { Text = reader.ReadAnonymousNbtTag(protocolVersion) };
                V755_764 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetTitleTextPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public V755_764Fields? V755_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    public struct V755_764Fields
    {
        public string Text { get; set; }
    }

    public struct V765_LastFields
    {
        public NbtTag Text { get; set; }
    }
}