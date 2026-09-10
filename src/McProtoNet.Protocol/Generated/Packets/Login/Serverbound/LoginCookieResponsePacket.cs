using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.cookie_response", PacketPhase.Login, PacketDirection.Serverbound)]
[PacketField("Key", "string")]
[PacketField("Value", "byte[]?")]
public sealed partial record LoginCookieResponsePacket(string Key, byte[]? Value) : IPacket<LoginCookieResponsePacket>, IPacket
{
    public static LoginCookieResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCookieResponsePacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 771)
        {
            var key = reader.ReadString();
            byte[]? value = null;
            if (reader.ReadBoolean())
                value = reader.ReadByteArray();
            return new LoginCookieResponsePacket(key, value);
        }

        if (protocolVersion >= 772 && protocolVersion <= 772)
        {
            var key = reader.ReadString();
            var value = reader.ReadByteArray();
            return new LoginCookieResponsePacket(key, value);
        }

        if (protocolVersion >= 773)
        {
            var key = reader.ReadString();
            byte[]? value = null;
            if (reader.ReadBoolean())
                value = reader.ReadByteArray();
            return new LoginCookieResponsePacket(key, value);
        }

        throw new System.NotSupportedException($"LoginCookieResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginCookieResponsePacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 771)
        {
            writer.WriteString(Key);
            writer.WriteBoolean(Value is not null);
            if (Value is { } valueValue)
                writer.WriteByteArray(valueValue);
            return;
        }

        if (protocolVersion >= 772 && protocolVersion <= 772)
        {
            writer.WriteString(Key);
            writer.WriteByteArray((Value ?? throw new System.InvalidOperationException("Value is required at this protocol version.")));
            return;
        }

        if (protocolVersion >= 773)
        {
            writer.WriteString(Key);
            writer.WriteBoolean(Value is not null);
            if (Value is { } valueValue)
                writer.WriteByteArray(valueValue);
            return;
        }

        throw new System.NotSupportedException($"LoginCookieResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("login.toServer.cookie_response", "LoginCookieResponse", PacketPhase.Login, PacketDirection.Serverbound, 0);

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
