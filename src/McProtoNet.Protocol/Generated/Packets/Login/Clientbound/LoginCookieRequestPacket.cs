using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.cookie_request", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("Cookie", "string")]
public sealed partial record LoginCookieRequestPacket(string Cookie) : IPacket<LoginCookieRequestPacket>, IPacket
{
    public static LoginCookieRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCookieRequestPacket>(protocolVersion);
        var cookie = reader.ReadString();
        return new LoginCookieRequestPacket(cookie);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCookieRequestPacket>(protocolVersion);
        writer.WriteString(Cookie);
    }

    public static PacketIdentity Identity => new("login.toClient.cookie_request", "LoginCookieRequest", PacketPhase.Login, PacketDirection.Clientbound, 1);

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
