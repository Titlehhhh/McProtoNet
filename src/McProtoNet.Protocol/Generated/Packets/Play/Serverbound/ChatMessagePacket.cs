using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.chat_message", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Message", "string")]
[PacketField("Timestamp", "long")]
[PacketField("Salt", "long")]
[PacketField("Signature", "byte[]?")]
[PacketField("SignedPreview", "bool", Group = "V759", From = 759, To = 759)]
[PacketField("SignedPreview", "bool", Group = "V760", From = 760, To = 760)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V760", From = 760, To = 760)]
[PacketField("LastRejectedMessage", "LastRejectedMessage?", Group = "V760", From = 760, To = 760)]
[PacketField("Offset", "int", Group = "V761_769", From = 761, To = 769)]
[PacketField("Acknowledged", "byte[]", Group = "V761_769", From = 761, To = 769)]
[PacketField("Offset", "int", Group = "V770_Last", From = 770)]
[PacketField("Acknowledged", "byte[]", Group = "V770_Last", From = 770)]
[PacketField("Checksum", "int", Group = "V770_Last", From = 770)]
public sealed partial record ChatMessagePacket(string Message, long Timestamp, long Salt, byte[]? Signature, ChatMessagePacket.V759Layer? V759 = null, ChatMessagePacket.V760Layer? V760 = null, ChatMessagePacket.V761_769Layer? V761_769 = null, ChatMessagePacket.V770_LastLayer? V770_Last = null) : IPacket<ChatMessagePacket>, IPacket
{
    public readonly record struct V759Layer(bool SignedPreview);
    public readonly record struct V760Layer(bool SignedPreview, PreviousMessage[] PreviousMessages, LastRejectedMessage? LastRejectedMessage);
    public readonly record struct V761_769Layer(int Offset, byte[] Acknowledged);
    public readonly record struct V770_LastLayer(int Offset, byte[] Acknowledged, int Checksum);
    public static ChatMessagePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatMessagePacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var message = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            var signature = reader.ReadByteArray();
            var signedPreview = reader.ReadBoolean();
            return new ChatMessagePacket(message, timestamp, salt, signature, V759: new V759Layer(signedPreview));
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var message = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            var signature = reader.ReadByteArray();
            var signedPreview = reader.ReadBoolean();
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            LastRejectedMessage? lastRejectedMessage = null;
            if (reader.ReadBoolean())
                lastRejectedMessage = reader.ReadType<LastRejectedMessage>(protocolVersion);
            return new ChatMessagePacket(message, timestamp, salt, signature, V760: new V760Layer(signedPreview, previousMessages, lastRejectedMessage));
        }

        if (protocolVersion >= 761 && protocolVersion <= 769)
        {
            var message = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            byte[]? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadFixedBytes(256);
            var offset = reader.ReadVarInt();
            var acknowledged = reader.ReadFixedBytes(3);
            return new ChatMessagePacket(message, timestamp, salt, signature, V761_769: new V761_769Layer(offset, acknowledged));
        }

        if (protocolVersion >= 770)
        {
            var message = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            byte[]? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadFixedBytes(256);
            var offset = reader.ReadVarInt();
            var acknowledged = reader.ReadFixedBytes(3);
            var checksum = reader.ReadUnsignedByte();
            return new ChatMessagePacket(message, timestamp, salt, signature, V770_Last: new V770_LastLayer(offset, acknowledged, checksum));
        }

        throw new System.NotSupportedException($"ChatMessagePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatMessagePacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var layer = V759 ?? throw new WrongLayerException("ChatMessagePacket", protocolVersion, "V759");
            bool SignedPreview = layer.SignedPreview;
            writer.WriteString(Message);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteByteArray((Signature ?? throw new System.InvalidOperationException("Signature is required at this protocol version.")));
            writer.WriteBoolean(SignedPreview);
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var layer = V760 ?? throw new WrongLayerException("ChatMessagePacket", protocolVersion, "V760");
            bool SignedPreview = layer.SignedPreview;
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            LastRejectedMessage? LastRejectedMessage = layer.LastRejectedMessage;
            writer.WriteString(Message);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteByteArray((Signature ?? throw new System.InvalidOperationException("Signature is required at this protocol version.")));
            writer.WriteBoolean(SignedPreview);
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(LastRejectedMessage is not null);
            if (LastRejectedMessage is { } lastRejectedMessageValue)
                writer.WriteType<LastRejectedMessage>(lastRejectedMessageValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 761 && protocolVersion <= 769)
        {
            var layer = V761_769 ?? throw new WrongLayerException("ChatMessagePacket", protocolVersion, "V761_769");
            int Offset = layer.Offset;
            byte[] Acknowledged = layer.Acknowledged;
            writer.WriteString(Message);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteFixedBytes(signatureValue, 256);
            writer.WriteVarInt(Offset);
            writer.WriteFixedBytes(Acknowledged, 3);
            return;
        }

        if (protocolVersion >= 770)
        {
            var layer = V770_Last ?? throw new WrongLayerException("ChatMessagePacket", protocolVersion, "V770_Last");
            int Offset = layer.Offset;
            byte[] Acknowledged = layer.Acknowledged;
            int Checksum = layer.Checksum;
            writer.WriteString(Message);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteFixedBytes(signatureValue, 256);
            writer.WriteVarInt(Offset);
            writer.WriteFixedBytes(Acknowledged, 3);
            writer.WriteUnsignedByte((byte)Checksum);
            return;
        }

        throw new System.NotSupportedException($"ChatMessagePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.chat_message", "ChatMessage", PacketPhase.Play, PacketDirection.Serverbound, 9);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 765)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x09;
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
