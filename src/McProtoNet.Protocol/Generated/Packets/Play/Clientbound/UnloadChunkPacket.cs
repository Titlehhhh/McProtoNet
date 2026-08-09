using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.unload_chunk", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ChunkX", "int")]
[PacketField("ChunkZ", "int")]
public sealed partial record UnloadChunkPacket(int ChunkX, int ChunkZ) : IPacket<UnloadChunkPacket>
{
    public static UnloadChunkPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UnloadChunkPacket>(protocolVersion);
        if (protocolVersion <= 763)
        {
            var chunkX = reader.ReadSignedInt();
            var chunkZ = reader.ReadSignedInt();
            return new UnloadChunkPacket(chunkX, chunkZ);
        }

        if (protocolVersion >= 764)
        {
            var chunkZ = reader.ReadSignedInt();
            var chunkX = reader.ReadSignedInt();
            return new UnloadChunkPacket(chunkX, chunkZ);
        }

        throw new System.NotSupportedException($"UnloadChunkPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UnloadChunkPacket>(protocolVersion);
        if (protocolVersion <= 763)
        {
            writer.WriteSignedInt(ChunkX);
            writer.WriteSignedInt(ChunkZ);
            return;
        }

        if (protocolVersion >= 764)
        {
            writer.WriteSignedInt(ChunkZ);
            writer.WriteSignedInt(ChunkX);
            return;
        }

        throw new System.NotSupportedException($"UnloadChunkPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.unload_chunk", "UnloadChunk", PacketPhase.Play, PacketDirection.Clientbound, 15);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x21;
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
