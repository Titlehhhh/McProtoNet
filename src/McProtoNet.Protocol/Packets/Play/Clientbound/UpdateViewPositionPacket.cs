using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UpdateViewPosition", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x40)]
[PacketId(751, 754, 0x40)]
[PacketId(755, 758, 0x49)]
[PacketId(759, 759, 0x48)]
[PacketId(760, 760, 0x4B)]
[PacketId(761, 761, 0x4A)]
[PacketId(762, 763, 0x4E)]
[PacketId(764, 764, 0x50)]
[PacketId(765, 765, 0x52)]
[PacketId(766, 767, 0x54)]
[PacketId(768, 769, 0x58)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x57)]
public sealed partial class UpdateViewPositionPacket : IServerPacket
{
    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(ChunkX);
        writer.WriteVarInt(ChunkZ);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ChunkX = reader.ReadVarInt();
        ChunkZ = reader.ReadVarInt();
    }
}