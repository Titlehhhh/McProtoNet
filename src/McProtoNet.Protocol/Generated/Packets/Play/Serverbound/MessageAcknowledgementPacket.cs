using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.message_acknowledgement", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V760", From = 760, To = 760)]
[PacketField("LastRejectedMessage", "LastRejectedMessage?", Group = "V760", From = 760, To = 760)]
[PacketField("Count", "int", Group = "V761_Last", From = 761)]
public sealed partial record MessageAcknowledgementPacket(MessageAcknowledgementPacket.V760Layer? V760 = null, MessageAcknowledgementPacket.V761_LastLayer? V761_Last = null) : IPacket<MessageAcknowledgementPacket>, IPacket
{
    public readonly record struct V760Layer(PreviousMessage[] PreviousMessages, LastRejectedMessage? LastRejectedMessage);
    public readonly record struct V761_LastLayer(int Count);
    public static MessageAcknowledgementPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MessageAcknowledgementPacket>(protocolVersion);
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            LastRejectedMessage? lastRejectedMessage = null;
            if (reader.ReadBoolean())
                lastRejectedMessage = reader.ReadType<LastRejectedMessage>(protocolVersion);
            return new MessageAcknowledgementPacket(V760: new V760Layer(previousMessages, lastRejectedMessage));
        }

        if (protocolVersion >= 761)
        {
            var count = reader.ReadVarInt();
            return new MessageAcknowledgementPacket(V761_Last: new V761_LastLayer(count));
        }

        throw new System.NotSupportedException($"MessageAcknowledgementPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MessageAcknowledgementPacket>(protocolVersion);
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var layer = V760 ?? throw new WrongLayerException("MessageAcknowledgementPacket", protocolVersion, "V760");
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            LastRejectedMessage? LastRejectedMessage = layer.LastRejectedMessage;
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(LastRejectedMessage is not null);
            if (LastRejectedMessage is { } lastRejectedMessageValue)
                writer.WriteType<LastRejectedMessage>(lastRejectedMessageValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 761)
        {
            var layer = V761_Last ?? throw new WrongLayerException("MessageAcknowledgementPacket", protocolVersion, "V761_Last");
            int Count = layer.Count;
            writer.WriteVarInt(Count);
            return;
        }

        throw new System.NotSupportedException($"MessageAcknowledgementPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.message_acknowledgement", "MessageAcknowledgement", PacketPhase.Play, PacketDirection.Serverbound, 33);

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
