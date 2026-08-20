using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(759, 760)]
[Packet("play.toClient.should_display_chat_preview", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ShouldDisplayChatPreview", "bool")]
public sealed partial record ShouldDisplayChatPreviewPacket(bool ShouldDisplayChatPreview) : IPacket<ShouldDisplayChatPreviewPacket>, IPacket
{
    public static ShouldDisplayChatPreviewPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ShouldDisplayChatPreviewPacket>(protocolVersion);
        var shouldDisplayChatPreview = reader.ReadBoolean();
        return new ShouldDisplayChatPreviewPacket(shouldDisplayChatPreview);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ShouldDisplayChatPreviewPacket>(protocolVersion);
        writer.WriteBoolean(ShouldDisplayChatPreview);
    }

    public static PacketIdentity Identity => new("play.toClient.should_display_chat_preview", "ShouldDisplayChatPreview", PacketPhase.Play, PacketDirection.Clientbound, 87);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x4B;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x4E;
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
