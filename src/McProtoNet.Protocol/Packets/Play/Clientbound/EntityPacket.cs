using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Entity", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2B)]
[PacketId(751, 754, 0x2A)]
public sealed partial class EntityPacket : IServerPacket
{
    public int EntityId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(EntityId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => EntityId = reader.ReadVarInt();
}