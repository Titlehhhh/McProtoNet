using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Status.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("status.toClient.server_info", PacketPhase.Status, PacketDirection.Clientbound)]
[PacketField("Response", "string")]
public sealed partial record ServerInfoPacket(string Response) : IPacket<ServerInfoPacket>, IPacket
{
    public static ServerInfoPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerInfoPacket>(protocolVersion);
        var response = reader.ReadString();
        return new ServerInfoPacket(response);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerInfoPacket>(protocolVersion);
        writer.WriteString(Response);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Response");
        writer.WriteStringValue(Response);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("status.toClient.server_info", "ServerInfo", PacketPhase.Status, PacketDirection.Clientbound, 1);

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
