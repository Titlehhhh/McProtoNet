using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("BlockDig", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1B)]
[PacketId(751, 754, 0x1B)]
[PacketId(755, 758, 0x1A)]
[PacketId(759, 759, 0x1C)]
[PacketId(760, 760, 0x1D)]
[PacketId(761, 761, 0x1C)]
[PacketId(762, 763, 0x1D)]
[PacketId(764, 764, 0x20)]
[PacketId(765, 765, 0x21)]
[PacketId(766, 767, 0x24)]
[PacketId(768, 768, 0x26)]
[PacketId(769, 770, 0x27)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x28)]
public sealed partial class BlockDigPacket : IClientPacket
{
    public int Status { get; set; }
    public Position Location { get; set; }
    public sbyte Face { get; set; }
    public int? Sequence { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                writer.WriteVarInt(Status);
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteSignedByte(Face);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Status);
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteSignedByte(Face);
                writer.WriteVarInt(Sequence ?? throw new InvalidOperationException("BlockDigPacket 759-last fields missing."));
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockDigPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                Status = reader.ReadVarInt();
                Location = reader.ReadType<Position>(protocolVersion);
                Face = reader.ReadSignedByte();
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                Status = reader.ReadVarInt();
                Location = reader.ReadType<Position>(protocolVersion);
                Face = reader.ReadSignedByte();
                Sequence = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockDigPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}