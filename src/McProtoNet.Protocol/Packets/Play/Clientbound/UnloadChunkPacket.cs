using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UnloadChunk", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class UnloadChunkPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 763),
        new(764, MinecraftVersion.LatestProtocol),
    };

    public int ChunkX { get; set; }
    public int ChunkZ { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                writer.WriteSignedInt(ChunkX);
                writer.WriteSignedInt(ChunkZ);
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                writer.WriteSignedInt(ChunkZ);
                writer.WriteSignedInt(ChunkX);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UnloadChunk), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                ChunkX = reader.ReadSignedInt();
                ChunkZ = reader.ReadSignedInt();
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                ChunkZ = reader.ReadSignedInt();
                ChunkX = reader.ReadSignedInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UnloadChunk), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
