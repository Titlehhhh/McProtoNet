using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.chunk_batch_start", PacketPhase.Play, PacketDirection.Clientbound)]
public sealed partial record ChunkBatchStartPacket() : IPacket<ChunkBatchStartPacket>, IPacket
{
    public static ChunkBatchStartPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBatchStartPacket>(protocolVersion);
        return new ChunkBatchStartPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBatchStartPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.chunk_batch_start", "ChunkBatchStart", PacketPhase.Play, PacketDirection.Clientbound, 14);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x0C;
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
