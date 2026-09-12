using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Message");
        writer.WriteStringValue(Message);
        writer.WritePropertyName("Position");
        writer.WriteNumberValue(Position);
        writer.WritePropertyName("Sender");
        writer.WriteStringValue(Sender);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.chat", "Chat", PacketPhase.Play, PacketDirection.Clientbound, 11);

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
