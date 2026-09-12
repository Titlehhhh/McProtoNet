using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.cookie_request", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Cookie", "string")]
public sealed partial record CookieRequestPacket(string Cookie) : IPacket<CookieRequestPacket>, IPacket
{
    public static CookieRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CookieRequestPacket>(protocolVersion);
        var cookie = reader.ReadString();
        return new CookieRequestPacket(cookie);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CookieRequestPacket>(protocolVersion);
        writer.WriteString(Cookie);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Cookie");
        writer.WriteStringValue(Cookie);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.cookie_request", "CookieRequest", PacketPhase.Configuration, PacketDirection.Clientbound, 3);

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
