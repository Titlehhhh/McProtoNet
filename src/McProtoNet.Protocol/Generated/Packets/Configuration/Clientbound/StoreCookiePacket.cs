using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Key");
        writer.WriteStringValue(Key);
        writer.WritePropertyName("Value");
        writer.WriteBase64StringValue(Value);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.store_cookie", "StoreCookie", PacketPhase.Configuration, PacketDirection.Clientbound, 18);

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
