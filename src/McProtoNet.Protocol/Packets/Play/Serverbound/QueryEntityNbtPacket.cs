using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("QueryEntityNbt", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0D)]
[PacketId(751, 754, 0x0D)]
[PacketId(755, 758, 0x0C)]
[PacketId(759, 759, 0x0E)]
[PacketId(760, 760, 0x0F)]
[PacketId(761, 761, 0x0E)]
[PacketId(762, 763, 0x0F)]
[PacketId(764, 764, 0x11)]
[PacketId(765, 765, 0x12)]
[PacketId(766, 767, 0x15)]
[PacketId(768, 770, 0x17)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x18)]
public sealed partial class QueryEntityNbtPacket : IClientPacket
{
    public int TransactionId { get; set; }
    public int EntityId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(TransactionId);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => TransactionId = reader.ReadVarInt();
           EntityId = reader.ReadVarInt();
}