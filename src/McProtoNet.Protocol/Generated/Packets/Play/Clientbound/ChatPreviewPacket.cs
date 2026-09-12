using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(759, 760)]
[Packet("play.toClient.chat_preview", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("QueryId", "int")]
[PacketField("Message", "string?")]
public sealed partial record ChatPreviewPacket(int QueryId, string? Message) : IPacket<ChatPreviewPacket>, IPacket
{
    public static ChatPreviewPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPreviewPacket>(protocolVersion);
        var queryId = reader.ReadSignedInt();
        string? message = null;
        if (reader.ReadBoolean())
            message = reader.ReadString();
        return new ChatPreviewPacket(queryId, message);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPreviewPacket>(protocolVersion);
        writer.WriteSignedInt(QueryId);
        writer.WriteBoolean(Message is not null);
        if (Message is { } messageValue)
            writer.WriteString(messageValue);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("QueryId");
        writer.WriteNumberValue(QueryId);
        if (Message is { } messageValue)
        {
            writer.WritePropertyName("Message");
            writer.WriteStringValue(messageValue);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.chat_preview", "ChatPreview", PacketPhase.Play, PacketDirection.Clientbound, 12);

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
