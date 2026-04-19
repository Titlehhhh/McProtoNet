using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UnloadChunk", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1D)]
[PacketId(751, 754, 0x1C)]
[PacketId(755, 758, 0x1D)]
[PacketId(759, 759, 0x1A)]
[PacketId(760, 760, 0x1C)]
[PacketId(761, 761, 0x1B)]
[PacketId(762, 763, 0x1E)]
[PacketId(764, 765, 0x1F)]
[PacketId(766, 767, 0x21)]
[PacketId(768, 769, 0x22)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x21)]
public sealed partial class UnloadChunkPacket : IServerPacket
{
    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                writer.WriteSignedInt(ChunkX);
                writer.WriteSignedInt(ChunkZ);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteSignedInt(ChunkZ);
                writer.WriteSignedInt(ChunkX);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UnloadChunkPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                ChunkX = reader.ReadSignedInt();
                ChunkZ = reader.ReadSignedInt();
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                ChunkZ = reader.ReadSignedInt();
                ChunkX = reader.ReadSignedInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UnloadChunkPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}