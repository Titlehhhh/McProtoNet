using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ActionBar", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x41)]
[PacketId(759, 759, 0x40)]
[PacketId(760, 760, 0x43)]
[PacketId(761, 761, 0x42)]
[PacketId(762, 763, 0x46)]
[PacketId(764, 764, 0x48)]
[PacketId(765, 765, 0x4A)]
[PacketId(766, 767, 0x4C)]
[PacketId(768, 769, 0x51)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x50)]
public sealed partial class ActionBarPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 764:
            {
                var fields = V755_764 ?? throw new InvalidOperationException("ActionBarPacket 755-764 fields missing.");
                writer.WriteString(fields.Text);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("ActionBarPacket 765-last fields missing.");
                writer.WriteAnonymousNbtTag(fields.Text, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ActionBarPacket), protocolVersion, SupportedVersions);
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
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                V765_Last = new V765_LastFields { Text = reader.ReadAnonymousNbtTag(protocolVersion) };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ActionBarPacket), protocolVersion, SupportedVersions);
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