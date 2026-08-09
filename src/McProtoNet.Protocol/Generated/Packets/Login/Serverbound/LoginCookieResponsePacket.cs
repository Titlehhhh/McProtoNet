using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.cookie_response", PacketPhase.Login, PacketDirection.Serverbound)]
[PacketField("Key", "string")]
[PacketField("Value", "byte[]?")]
public sealed partial record LoginCookieResponsePacket(string Key, byte[]? Value) : IPacket<LoginCookieResponsePacket>
{
    public static LoginCookieResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCookieResponsePacket>(protocolVersion);
        var key = reader.ReadString();
        byte[]? value = null;
        if (reader.ReadBoolean())
            value = reader.ReadByteArray();
        return new LoginCookieResponsePacket(key, value);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCookieResponsePacket>(protocolVersion);
        writer.WriteString(Key);
        writer.WriteBoolean(Value is not null);
        if (Value is { } valueValue)
            writer.WriteByteArray(valueValue);
    }

    public static PacketIdentity Identity => new("login.toServer.cookie_response", "LoginCookieResponse", PacketPhase.Login, PacketDirection.Serverbound, 0);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x04;
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
