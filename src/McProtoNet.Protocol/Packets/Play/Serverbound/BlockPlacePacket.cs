using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("BlockPlace", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2D)]
[PacketId(751, 758, 0x2E)]
[PacketId(759, 759, 0x30)]
[PacketId(760, 763, 0x31)]
[PacketId(764, 764, 0x34)]
[PacketId(765, 765, 0x35)]
[PacketId(766, 767, 0x38)]
[PacketId(768, 768, 0x3A)]
[PacketId(769, 769, 0x3C)]
[PacketId(770, 770, 0x3E)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x3F)]
public sealed partial class BlockPlacePacket : IClientPacket
{
    public int Hand { get; set; }
    public Position Location { get; set; }
    public int Direction { get; set; }
    public float CursorX { get; set; }
    public float CursorY { get; set; }
    public float CursorZ { get; set; }
    public bool InsideBlock { get; set; }
    public int? Sequence { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                writer.WriteVarInt(Hand);
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteVarInt(Direction);
                writer.WriteFloat(CursorX);
                writer.WriteFloat(CursorY);
                writer.WriteFloat(CursorZ);
                writer.WriteBoolean(InsideBlock);
                return;
            }
            case >= 759 and <= 767:
            {
                writer.WriteVarInt(Hand);
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteVarInt(Direction);
                writer.WriteFloat(CursorX);
                writer.WriteFloat(CursorY);
                writer.WriteFloat(CursorZ);
                writer.WriteBoolean(InsideBlock);
                writer.WriteVarInt(Sequence ?? throw new InvalidOperationException("BlockPlacePacket 759-767 Sequence missing."));
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Hand);
                writer.WriteType<Position>(Location, protocolVersion);
                writer.WriteVarInt(Direction);
                writer.WriteFloat(CursorX);
                writer.WriteFloat(CursorY);
                writer.WriteFloat(CursorZ);
                writer.WriteBoolean(InsideBlock);
                writer.WriteBoolean(V768_Last?.WorldBorderHit ?? throw new InvalidOperationException("BlockPlacePacket 768-last WorldBorderHit missing."));
                writer.WriteVarInt(Sequence ?? throw new InvalidOperationException("BlockPlacePacket 768-last Sequence missing."));
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockPlacePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                Hand = reader.ReadVarInt();
                Location = reader.ReadType<Position>(protocolVersion);
                Direction = reader.ReadVarInt();
                CursorX = reader.ReadFloat();
                CursorY = reader.ReadFloat();
                CursorZ = reader.ReadFloat();
                InsideBlock = reader.ReadBoolean();
                V768_Last = null;
                Sequence = null;
                return;
            }
            case >= 759 and <= 767:
            {
                Hand = reader.ReadVarInt();
                Location = reader.ReadType<Position>(protocolVersion);
                Direction = reader.ReadVarInt();
                CursorX = reader.ReadFloat();
                CursorY = reader.ReadFloat();
                CursorZ = reader.ReadFloat();
                InsideBlock = reader.ReadBoolean();
                Sequence = reader.ReadVarInt();
                V768_Last = null;
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                Hand = reader.ReadVarInt();
                Location = reader.ReadType<Position>(protocolVersion);
                Direction = reader.ReadVarInt();
                CursorX = reader.ReadFloat();
                CursorY = reader.ReadFloat();
                CursorZ = reader.ReadFloat();
                InsideBlock = reader.ReadBoolean();
                V768_Last = new V768_LastFields { WorldBorderHit = reader.ReadBoolean() };
                Sequence = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockPlacePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V768_LastFields { public bool WorldBorderHit { get; set; } }
}