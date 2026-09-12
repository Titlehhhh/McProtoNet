using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[ProtocolSupport(764, 764)]
[Packet("configuration.toClient.resource_pack_send", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Url", "string")]
[PacketField("Hash", "string")]
[PacketField("Forced", "bool")]
[PacketField("PromptMessage", "string?")]
public sealed partial record ResourcePackSendPacket(string Url, string Hash, bool Forced, string? PromptMessage) : IPacket<ResourcePackSendPacket>, IPacket
{
    public static ResourcePackSendPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackSendPacket>(protocolVersion);
        var url = reader.ReadString();
        var hash = reader.ReadString();
        var forced = reader.ReadBoolean();
        string? promptMessage = null;
        if (reader.ReadBoolean())
            promptMessage = reader.ReadString();
        return new ResourcePackSendPacket(url, hash, forced, promptMessage);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackSendPacket>(protocolVersion);
        writer.WriteString(Url);
        writer.WriteString(Hash);
        writer.WriteBoolean(Forced);
        writer.WriteBoolean(PromptMessage is not null);
        if (PromptMessage is { } promptMessageValue)
            writer.WriteString(promptMessageValue);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Url");
        writer.WriteStringValue(Url);
        writer.WritePropertyName("Hash");
        writer.WriteStringValue(Hash);
        writer.WritePropertyName("Forced");
        writer.WriteBooleanValue(Forced);
        if (PromptMessage is { } promptMessageValue)
        {
            writer.WritePropertyName("PromptMessage");
            writer.WriteStringValue(promptMessageValue);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.resource_pack_send", "ResourcePackSend", PacketPhase.Configuration, PacketDirection.Clientbound, 14);

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
