using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteType(Location, protocolVersion);
                writer.WriteUnsignedByte(Byte1);
                writer.WriteUnsignedByte(Byte2);
                writer.WriteVarInt(BlockId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockActionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                Location = reader.ReadType<Position>(protocolVersion);
                Byte1 = reader.ReadUnsignedByte();
                Byte2 = reader.ReadUnsignedByte();
                BlockId = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockActionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}