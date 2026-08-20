using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toServer.chat_preview", "ChatPreview", PacketPhase.Play, PacketDirection.Serverbound, 8);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x06;
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
