using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DestroyEntity", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, 755)]
[PacketId(755, 755, 0x3A)]
public sealed partial class DestroyEntityPacket : IServerPacket
{
    public int EntityId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(EntityId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => EntityId = reader.ReadVarInt();
}