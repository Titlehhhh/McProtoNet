using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.cookie_request", PacketPhase.Play, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("play.toClient.cookie_request", "CookieRequest", PacketPhase.Play, PacketDirection.Clientbound, 22);

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
