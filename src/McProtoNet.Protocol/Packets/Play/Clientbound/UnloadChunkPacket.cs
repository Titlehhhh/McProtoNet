using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("UnloadChunk", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 763)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
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
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
            {
                var fields = VFirst_763 ?? throw new InvalidOperationException("UnloadChunkPacket 1-763 fields missing.");
                writer.WriteSignedInt(fields.ChunkX);
                writer.WriteSignedInt(fields.ChunkZ);
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("UnloadChunkPacket 764-last fields missing.");
                writer.WriteSignedInt(fields.ChunkZ);
                writer.WriteSignedInt(fields.ChunkX);
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
                VFirst_763 = new VFirst_763Fields
                {
                    ChunkX = reader.ReadSignedInt(),
                    ChunkZ = reader.ReadSignedInt()
                };
                V764_Last = null;
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                V764_Last = new V764_LastFields
                {
                    ChunkZ = reader.ReadSignedInt(),
                    ChunkX = reader.ReadSignedInt()
                };
                VFirst_763 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UnloadChunkPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public VFirst_763Fields? VFirst_763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    public struct VFirst_763Fields
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
    }

    public struct V764_LastFields
    {
        public int ChunkZ { get; set; }
        public int ChunkX { get; set; }
    }
}