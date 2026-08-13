using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.chat_preview", "ChatPreview", PacketPhase.Play, PacketDirection.Clientbound, 11);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            id = 0x0C;
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
