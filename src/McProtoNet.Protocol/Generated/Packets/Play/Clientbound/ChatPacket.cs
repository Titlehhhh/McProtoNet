using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[Packet("play.toClient.chat", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Message", "string")]
[PacketField("Position", "int")]
[PacketField("Sender", "Guid")]
public sealed partial record ChatPacket(string Message, int Position, Guid Sender) : IPacket<ChatPacket>, IPacket
{
    public static ChatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPacket>(protocolVersion);
        var message = reader.ReadString();
        var position = reader.ReadSignedByte();
        var sender = reader.ReadUUID();
        return new ChatPacket(message, position, sender);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatPacket>(protocolVersion);
        writer.WriteString(Message);
        writer.WriteSignedByte((sbyte)Position);
        writer.WriteUUID(Sender);
    }

    public static PacketIdentity Identity => new("play.toClient.chat", "Chat", PacketPhase.Play, PacketDirection.Clientbound, 10);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0F;
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
