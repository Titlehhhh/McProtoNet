using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("ChunkBatchReceived", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x07)]
[PacketId(766, 767, 0x08)]
[PacketId(768, 770, 0x09)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x0A)]
public sealed partial class ChunkBatchReceivedPacket : IClientPacket
{
    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("ChunkBatchReceivedPacket 764-last fields missing.");
                writer.WriteFloat(fields.ChunksPerTick);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ChunkBatchReceivedPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                V764_Last = new V764_LastFields { ChunksPerTick = reader.ReadFloat() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ChunkBatchReceivedPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public V764_LastFields? V764_Last { get; set; }

    public struct V764_LastFields
    {
        public float ChunksPerTick { get; set; }
    }
}