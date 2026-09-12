using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_view_distance", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ViewDistance", "int")]
public sealed partial record UpdateViewDistancePacket(int ViewDistance) : IPacket<UpdateViewDistancePacket>, IPacket
{
    public static UpdateViewDistancePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateViewDistancePacket>(protocolVersion);
        var viewDistance = reader.ReadVarInt();
        return new UpdateViewDistancePacket(viewDistance);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateViewDistancePacket>(protocolVersion);
        writer.WriteVarInt(ViewDistance);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("ViewDistance");
        writer.WriteNumberValue(ViewDistance);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.update_view_distance", "UpdateViewDistance", PacketPhase.Play, PacketDirection.Clientbound, 126);

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
