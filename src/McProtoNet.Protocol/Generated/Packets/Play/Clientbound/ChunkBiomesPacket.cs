using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.chunk_biomes", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Biomes", "ChunkBiomeData[]")]
public sealed partial record ChunkBiomesPacket(ChunkBiomeData[] Biomes) : IPacket<ChunkBiomesPacket>, IPacket
{
    public static ChunkBiomesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBiomesPacket>(protocolVersion);
        int biomesCount = reader.ReadVarInt();
        var biomes = new ChunkBiomeData[biomesCount];
        for (int i = 0; i < biomes.Length; i++)
            biomes[i] = reader.ReadType<ChunkBiomeData>(protocolVersion);
        return new ChunkBiomesPacket(biomes);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChunkBiomesPacket>(protocolVersion);
        writer.WriteVarInt(Biomes.Length);
        foreach (var biomesItem in Biomes)
            writer.WriteType<ChunkBiomeData>(biomesItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.chunk_biomes", "ChunkBiomes", PacketPhase.Play, PacketDirection.Clientbound, 15);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x0D;
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
