using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("Chat", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x03)]
[PacketId(751, 758, 0x03)]
public sealed partial class ChatPacket : IClientPacket
{
    public string Name { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteString(Name);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadString();
}