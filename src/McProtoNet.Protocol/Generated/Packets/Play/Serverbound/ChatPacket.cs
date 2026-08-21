using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[Packet("play.toServer.chat", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Message", "string")]
public sealed partial record ChatPacket(string Message) : IPacket<ChatPacket>, IPacket
{
    public static ChatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPacket>(protocolVersion);
        var message = reader.ReadString();
        return new ChatPacket(message);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPacket>(protocolVersion);
        writer.WriteString(Message);
    }

    public static PacketIdentity Identity => new("play.toServer.chat", "Chat", PacketPhase.Play, PacketDirection.Serverbound, 6);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x03;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x03;
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
