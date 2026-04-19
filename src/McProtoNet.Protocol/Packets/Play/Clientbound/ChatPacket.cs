using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Chat", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0E)]
[PacketId(751, 754, 0x0E)]
[PacketId(755, 758, 0x0F)]
public sealed partial class ChatPacket : IServerPacket
{
    public string Message { get; set; }
    public sbyte Position { get; set; }
    public Guid Sender { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Message);
        writer.WriteSignedByte(Position);
        writer.WriteUUID(Sender);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Message = reader.ReadString();
        Position = reader.ReadSignedByte();
        Sender = reader.ReadUUID();
    }
}