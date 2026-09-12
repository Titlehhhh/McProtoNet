using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.chunk_batch_received", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("ChunksPerTick", "float")]
public sealed partial record ChunkBatchReceivedPacket(float ChunksPerTick) : IPacket<ChunkBatchReceivedPacket>, IPacket
{
    public static ChunkBatchReceivedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBatchReceivedPacket>(protocolVersion);
        var chunksPerTick = reader.ReadFloat();
        return new ChunkBatchReceivedPacket(chunksPerTick);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBatchReceivedPacket>(protocolVersion);
        writer.WriteFloat(ChunksPerTick);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("ChunksPerTick");
        if (double.IsFinite(ChunksPerTick))
            writer.WriteNumberValue(ChunksPerTick);
        else
            writer.WriteStringValue(double.IsNaN(ChunksPerTick) ? "NaN" : ChunksPerTick > 0 ? "Infinity" : "-Infinity");
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.chunk_batch_received", "ChunkBatchReceived", PacketPhase.Play, PacketDirection.Serverbound, 13);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
