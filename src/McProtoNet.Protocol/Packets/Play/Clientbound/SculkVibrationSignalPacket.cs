using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SculkVibrationSignal", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, 758)]
[PacketId(755, 758, 0x05)]
public sealed partial class SculkVibrationSignalPacket : IServerPacket
{
    public Position SourcePosition { get; set; }
    public string DestinationIdentifier { get; set; }
    public Position DestinationBlock { get; set; }
    public int DestinationEntityId { get; set; }
    public int ArrivalTicks { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteType<Position>(SourcePosition, protocolVersion);
        writer.WriteString(DestinationIdentifier);
        writer.WriteType<Position>(DestinationBlock, protocolVersion);
        writer.WriteVarInt(DestinationEntityId);
        writer.WriteVarInt(ArrivalTicks);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        SourcePosition = reader.ReadType<Position>(protocolVersion);
        DestinationIdentifier = reader.ReadString();
        DestinationBlock = reader.ReadType<Position>(protocolVersion);
        DestinationEntityId = reader.ReadVarInt();
        ArrivalTicks = reader.ReadVarInt();
    }
}