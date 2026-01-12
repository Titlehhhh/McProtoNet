using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ChunkBiomes", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ChunkBiomesPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(762, 765),
        new(766, MinecraftVersion.LatestProtocol),
    };

    public BiomeEntry[] Biomes { get; set; } = Array.Empty<BiomeEntry>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 762 and <= 765:
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Biomes.Length);
                for (int i = 0; i < Biomes.Length; i++)
                {
                    writer.WritePackedChunkPos(Biomes[i].Position, protocolVersion);
                    writer.WriteBuffer<VarInt>(Biomes[i].Data);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ChunkBiomes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 762 and <= 765:
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                int length = reader.ReadVarInt();
                if (length == 0)
                {
                    Biomes = Array.Empty<BiomeEntry>();
                    return;
                }

                var biomes = new BiomeEntry[length];
                for (int i = 0; i < biomes.Length; i++)
                {
                    biomes[i] = new BiomeEntry
                    {
                        Position = reader.ReadPackedChunkPos(protocolVersion),
                        Data = reader.ReadBuffer(LengthFormat.VarInt)
                    };
                }

                Biomes = biomes;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ChunkBiomes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct BiomeEntry
    {
        public PackedChunkPos Position { get; set; }
        public byte[] Data { get; set; }
    }
}
