using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.chunk_batch_finished", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("BatchSize", "int")]
public sealed partial record ChunkBatchFinishedPacket(int BatchSize) : IPacket<ChunkBatchFinishedPacket>, IPacket
{
    public static ChunkBatchFinishedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBatchFinishedPacket>(protocolVersion);
        var batchSize = reader.ReadVarInt();
        return new ChunkBatchFinishedPacket(batchSize);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBatchFinishedPacket>(protocolVersion);
        writer.WriteVarInt(BatchSize);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("BatchSize");
        writer.WriteNumberValue(BatchSize);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.chunk_batch_finished", "ChunkBatchFinished", PacketPhase.Play, PacketDirection.Clientbound, 14);

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
