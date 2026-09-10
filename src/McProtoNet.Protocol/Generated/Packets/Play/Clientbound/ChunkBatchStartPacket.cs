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

    public static PacketIdentity Identity => new("play.toClient.chunk_batch_start", "ChunkBatchStart", PacketPhase.Play, PacketDirection.Clientbound, 15);

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
