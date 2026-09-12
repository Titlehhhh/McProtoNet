using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.unload_chunk", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ChunkX", "int")]
[PacketField("ChunkZ", "int")]
public sealed partial record UnloadChunkPacket(int ChunkX, int ChunkZ) : IPacket<UnloadChunkPacket>, IPacket
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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("ChunkX");
        writer.WriteNumberValue(ChunkX);
        writer.WritePropertyName("ChunkZ");
        writer.WriteNumberValue(ChunkZ);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.unload_chunk", "UnloadChunk", PacketPhase.Play, PacketDirection.Clientbound, 121);

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
