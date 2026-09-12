using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_view_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ChunkX", "int")]
[PacketField("ChunkZ", "int")]
public sealed partial record UpdateViewPositionPacket(int ChunkX, int ChunkZ) : IPacket<UpdateViewPositionPacket>, IPacket
{
    public static UpdateViewPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateViewPositionPacket>(protocolVersion);
        var chunkX = reader.ReadVarInt();
        var chunkZ = reader.ReadVarInt();
        return new UpdateViewPositionPacket(chunkX, chunkZ);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateViewPositionPacket>(protocolVersion);
        writer.WriteVarInt(ChunkX);
        writer.WriteVarInt(ChunkZ);
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

    public static PacketIdentity Identity => new("play.toClient.update_view_position", "UpdateViewPosition", PacketPhase.Play, PacketDirection.Clientbound, 126);

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
