using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChatPreview", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(759, 760)]
[PacketId(759, 759, 0x05)]
[PacketId(760, 760, 0x06)]
public sealed partial class ChatPreviewPacket : IClientPacket
{
    public int Name { get; set; }
    public string Message { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedInt(Name);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadSignedInt();
}