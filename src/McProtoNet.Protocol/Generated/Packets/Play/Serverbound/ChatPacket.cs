using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Message");
        writer.WriteStringValue(Message);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.chat", "Chat", PacketPhase.Play, PacketDirection.Serverbound, 7);

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
