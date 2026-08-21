using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.chunk_batch_finished", "ChunkBatchFinished", PacketPhase.Play, PacketDirection.Clientbound, 13);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x0B;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
