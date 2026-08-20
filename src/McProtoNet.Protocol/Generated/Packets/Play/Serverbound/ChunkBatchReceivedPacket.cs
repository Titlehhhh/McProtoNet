using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toServer.chunk_batch_received", "ChunkBatchReceived", PacketPhase.Play, PacketDirection.Serverbound, 10);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
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
