using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("configuration.toClient.cookie_request", "CookieRequest", PacketPhase.Configuration, PacketDirection.Clientbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x00;
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
