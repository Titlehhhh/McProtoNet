using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.cookie_request", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("Cookie", "string")]
public sealed partial record LoginCookieRequestPacket(string Cookie) : IPacket<LoginCookieRequestPacket>
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

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x05;
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
