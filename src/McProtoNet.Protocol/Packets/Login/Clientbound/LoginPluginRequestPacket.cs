using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("LoginPluginRequest", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x04)]
public sealed partial class LoginPluginRequestPacket : IPacket
{
    public int MessageId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public byte[]? Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(MessageId);
        writer.WriteString(Channel);
        writer.WriteBuffer(Data);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        MessageId = reader.ReadVarInt();
        Channel = reader.ReadString();
        Data = reader.ReadRestBuffer();
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}