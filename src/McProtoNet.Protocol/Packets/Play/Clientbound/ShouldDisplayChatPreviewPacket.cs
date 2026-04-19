using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ShouldDisplayChatPreview", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(759, 760)]
[PacketId(759, 759, 0x4B)]
[PacketId(760, 760, 0x4E)]
public sealed partial class ShouldDisplayChatPreviewPacket : IServerPacket
{
    public bool ShouldDisplayChatPreview { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteBoolean(ShouldDisplayChatPreview);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => ShouldDisplayChatPreview = reader.ReadBoolean();
}