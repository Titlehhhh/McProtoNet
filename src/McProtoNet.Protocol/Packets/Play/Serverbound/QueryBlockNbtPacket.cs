using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("QueryBlockNbt", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x01)]
[PacketId(751, MinecraftVersion.LatestProtocol, 0x01)]
public sealed partial class QueryBlockNbtPacket : IClientPacket
{
    public int TransactionId { get; set; }
    public Position Location { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(TransactionId);
        writer.WriteType(Location, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        TransactionId = reader.ReadVarInt();
        Location = reader.ReadType<Position>(protocolVersion);
    }
}