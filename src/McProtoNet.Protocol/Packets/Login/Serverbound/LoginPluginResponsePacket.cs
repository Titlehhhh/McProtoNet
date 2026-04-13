using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginPluginResponse", PacketState.Login, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class LoginPluginResponsePacket : IPacket
{
    public int MessageId { get; set; }
    public byte[]? Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(MessageId);
        if (Data is not null)
        {
            writer.WriteBuffer(Data);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        MessageId = reader.ReadVarInt();
        Data = reader.ReadRestBuffer();
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}