using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.chat_command", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Command", "string")]
public sealed partial record ChatCommandPacket(string Command) : IPacket<ChatCommandPacket>, IPacket
{
    public static ChatCommandPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandPacket>(protocolVersion);
        var command = reader.ReadString();
        return new ChatCommandPacket(command);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatCommandPacket>(protocolVersion);
        writer.WriteString(Command);
    }

    public static PacketIdentity Identity => new("play.toServer.chat_command", "ChatCommand", PacketPhase.Play, PacketDirection.Serverbound, 7);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x03;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 767)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x07;
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
