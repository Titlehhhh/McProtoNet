using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.chat_command_signed", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Command", "string")]
[PacketField("Timestamp", "long")]
[PacketField("Salt", "long")]
[PacketField("ArgumentSignatures", "ArgumentSignature[]")]
[PacketField("MessageCount", "int")]
[PacketField("Acknowledged", "byte[]")]
[PacketField("Checksum", "int", Group = "V770_Last", From = 770)]
public sealed partial record ChatCommandSignedPacket(string Command, long Timestamp, long Salt, ArgumentSignature[] ArgumentSignatures, int MessageCount, byte[] Acknowledged, ChatCommandSignedPacket.V770_LastLayer? V770_Last = null) : IPacket<ChatCommandSignedPacket>, IPacket
{
    public readonly record struct V770_LastLayer(int Checksum);
    public static ChatCommandSignedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandSignedPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            var command = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int argumentSignaturesCount = reader.ReadVarInt();
            var argumentSignatures = new ArgumentSignature[argumentSignaturesCount];
            for (int i = 0; i < argumentSignatures.Length; i++)
                argumentSignatures[i] = reader.ReadType<ArgumentSignature>(protocolVersion);
            var messageCount = reader.ReadVarInt();
            var acknowledged = reader.ReadFixedBytes(3);
            return new ChatCommandSignedPacket(command, timestamp, salt, argumentSignatures, messageCount, acknowledged);
        }

        if (protocolVersion >= 770)
        {
            var command = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int argumentSignaturesCount = reader.ReadVarInt();
            var argumentSignatures = new ArgumentSignature[argumentSignaturesCount];
            for (int i = 0; i < argumentSignatures.Length; i++)
                argumentSignatures[i] = reader.ReadType<ArgumentSignature>(protocolVersion);
            var messageCount = reader.ReadVarInt();
            var acknowledged = reader.ReadFixedBytes(3);
            var checksum = reader.ReadUnsignedByte();
            return new ChatCommandSignedPacket(command, timestamp, salt, argumentSignatures, messageCount, acknowledged, V770_Last: new V770_LastLayer(checksum));
        }

        throw new System.NotSupportedException($"ChatCommandSignedPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandSignedPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            writer.WriteString(Command);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(ArgumentSignatures.Length);
            foreach (var argumentSignaturesItem in ArgumentSignatures)
                writer.WriteType<ArgumentSignature>(argumentSignaturesItem, protocolVersion);
            writer.WriteVarInt(MessageCount);
            writer.WriteFixedBytes(Acknowledged, 3);
            return;
        }

        if (protocolVersion >= 770)
        {
            var layer = V770_Last ?? throw new WrongLayerException("ChatCommandSignedPacket", protocolVersion, "V770_Last");
            int Checksum = layer.Checksum;
            writer.WriteString(Command);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(ArgumentSignatures.Length);
            foreach (var argumentSignaturesItem in ArgumentSignatures)
                writer.WriteType<ArgumentSignature>(argumentSignaturesItem, protocolVersion);
            writer.WriteVarInt(MessageCount);
            writer.WriteFixedBytes(Acknowledged, 3);
            writer.WriteUnsignedByte((byte)Checksum);
            return;
        }

        throw new System.NotSupportedException($"ChatCommandSignedPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.chat_command_signed", "ChatCommandSigned", PacketPhase.Play, PacketDirection.Serverbound, 7);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x08;
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
