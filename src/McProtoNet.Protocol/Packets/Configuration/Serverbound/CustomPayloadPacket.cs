using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("CustomPayload", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x01)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class CustomPayloadPacket : IClientPacket
{
    public string Channel { get; set; }
    public byte[] Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Channel);
        writer.WriteBuffer(Data);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Channel = reader.ReadString();
        Data = reader.ReadRestBuffer();
    }
}