using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
public sealed partial class ChunkBiomeData : IProtocolType<ChunkBiomeData>
{
    public PackedChunkPos Position { get; }
    public byte[] Data { get; }

    public ChunkBiomeData(PackedChunkPos position, byte[] data)
    {
        Position = position;
        Data = data;
    }

    public static ChunkBiomeData Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBiomeData>(protocolVersion);
        var position = reader.ReadType<PackedChunkPos>(protocolVersion);
        var data = reader.ReadByteArray();
        return new ChunkBiomeData(position, data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBiomeData>(protocolVersion);
        writer.WriteType<PackedChunkPos>(Position, protocolVersion);
        writer.WriteByteArray(Data);
    }
}
