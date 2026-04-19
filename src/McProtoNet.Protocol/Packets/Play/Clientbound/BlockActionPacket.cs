using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("BlockAction", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0A)]
[PacketId(751, 754, 0x0A)]
[PacketId(755, 758, 0x0B)]
[PacketId(759, 761, 0x08)]
[PacketId(762, 763, 0x09)]
[PacketId(764, 769, 0x08)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x07)]
public sealed partial class BlockActionPacket : IServerPacket
{
    public Position Location { get; set; }
    public byte Byte1 { get; set; }
    public byte Byte2 { get; set; }
    public int BlockId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteUnsignedByte(Byte1);
        writer.WriteUnsignedByte(Byte2);
        writer.WriteVarInt(BlockId);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Location = reader.ReadType<Position>(protocolVersion);
        Byte1 = reader.ReadUnsignedByte();
        Byte2 = reader.ReadUnsignedByte();
        BlockId = reader.ReadVarInt();
    }
}