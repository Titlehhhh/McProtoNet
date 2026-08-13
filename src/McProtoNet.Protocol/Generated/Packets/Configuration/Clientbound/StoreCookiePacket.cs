using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.store_cookie", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Key", "string")]
[PacketField("Value", "byte[]")]
public sealed partial record StoreCookiePacket(string Key, byte[] Value) : IPacket<StoreCookiePacket>, IPacket
{
    public static StoreCookiePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StoreCookiePacket>(protocolVersion);
        var key = reader.ReadString();
        var value = reader.ReadByteArray();
        return new StoreCookiePacket(key, value);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StoreCookiePacket>(protocolVersion);
        writer.WriteString(Key);
        writer.WriteByteArray(Value);
    }

    public static PacketIdentity Identity => new("configuration.toClient.store_cookie", "StoreCookie", PacketPhase.Configuration, PacketDirection.Clientbound, 15);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x0A;
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
