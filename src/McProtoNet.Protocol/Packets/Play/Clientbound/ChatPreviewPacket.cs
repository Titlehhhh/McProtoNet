using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ChatPreview", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(759, 760)]
[PacketId(759, 760, 0x0C)]
public sealed partial class ChatPreviewPacket : IServerPacket
{
    public int QueryId { get; set; }
    public string? Message { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedInt(QueryId);
        writer.WriteBoolean(Message != null);
        if (Message != null)
        {
            writer.WriteString(Message);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        QueryId = reader.ReadSignedInt();
        bool hasMessage = reader.ReadBoolean();
        if (hasMessage)
        {
            Message = reader.ReadString();
        }
        else
        {
            Message = null;
        }
    }
}