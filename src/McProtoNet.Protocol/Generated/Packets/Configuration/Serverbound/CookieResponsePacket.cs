using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.cookie_response", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Key", "string")]
[PacketField("Value", "byte[]?")]
public sealed partial record CookieResponsePacket(string Key, byte[]? Value) : IPacket<CookieResponsePacket>, IPacket
{
    public static CookieResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CookieResponsePacket>(protocolVersion);
        var key = reader.ReadString();
        byte[]? value = null;
        if (reader.ReadBoolean())
            value = reader.ReadByteArray();
        return new CookieResponsePacket(key, value);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CookieResponsePacket>(protocolVersion);
        writer.WriteString(Key);
        writer.WriteBoolean(Value is not null);
        if (Value is { } valueValue)
            writer.WriteByteArray(valueValue);
    }

    public static PacketIdentity Identity => new("configuration.toServer.cookie_response", "CookieResponse", PacketPhase.Configuration, PacketDirection.Serverbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x01;
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
