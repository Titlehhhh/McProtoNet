using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct UnloadChunkPacket(int ChunkX, int ChunkZ) : IProtocolType<UnloadChunkPacket>
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

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
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

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x1D;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x1C;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x1D;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x1D;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x1D;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x1A;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x1C;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x1B;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x1E;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x1F;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x1F;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x21;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x21;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x22;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x21;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x21;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
