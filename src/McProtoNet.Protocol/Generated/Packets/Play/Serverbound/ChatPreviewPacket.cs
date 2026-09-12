using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(759, 760)]
[Packet("play.toServer.chat_preview", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Query", "int")]
[PacketField("Message", "string")]
public sealed partial record ChatPreviewPacket(int Query, string Message) : IPacket<ChatPreviewPacket>, IPacket
{
    public static ChatPreviewPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPreviewPacket>(protocolVersion);
        var query = reader.ReadSignedInt();
        var message = reader.ReadString();
        return new ChatPreviewPacket(query, message);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPreviewPacket>(protocolVersion);
        writer.WriteSignedInt(Query);
        writer.WriteString(Message);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Query");
        writer.WriteNumberValue(Query);
        writer.WritePropertyName("Message");
        writer.WriteStringValue(Message);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.chat_preview", "ChatPreview", PacketPhase.Play, PacketDirection.Serverbound, 11);

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
