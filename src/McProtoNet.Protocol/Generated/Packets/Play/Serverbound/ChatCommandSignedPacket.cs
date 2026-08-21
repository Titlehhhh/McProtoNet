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
            // TODO(codegen): read 'Acknowledged' (FixedBytes 3)
            throw new System.NotImplementedException("TODO(codegen): ChatCommandSignedPacket wire layout is not fully generated for this protocol version.");
        }

        if (protocolVersion >= 770)
        {
            // TODO(codegen): read 'Acknowledged' (FixedBytes 3)
            throw new System.NotImplementedException("TODO(codegen): ChatCommandSignedPacket wire layout is not fully generated for this protocol version.");
        }

        throw new System.NotSupportedException($"ChatCommandSignedPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandSignedPacket>(protocolVersion);
        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            // TODO(codegen): write 'Acknowledged' (FixedBytes 3)
            throw new System.NotImplementedException("TODO(codegen): ChatCommandSignedPacket wire layout is not fully generated for this protocol version.");
        }

        if (protocolVersion >= 770)
        {
            // TODO(codegen): write 'Acknowledged' (FixedBytes 3)
            throw new System.NotImplementedException("TODO(codegen): ChatCommandSignedPacket wire layout is not fully generated for this protocol version.");
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
