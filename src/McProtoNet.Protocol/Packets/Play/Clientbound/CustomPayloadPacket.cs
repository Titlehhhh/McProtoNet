using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CustomPayload", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x18)]
[PacketId(751, 754, 0x17)]
[PacketId(755, 758, 0x18)]
[PacketId(759, 759, 0x15)]
[PacketId(760, 760, 0x16)]
[PacketId(761, 761, 0x15)]
[PacketId(762, 763, 0x17)]
[PacketId(764, 765, 0x18)]
[PacketId(766, 769, 0x19)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x18)]
public sealed partial class CustomPayloadPacket : IServerPacket
{
    public string Channel { get; set; } = string.Empty;
    public byte[]? Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Channel);
        if (Data != null)
        {
            writer.WriteBuffer(Data);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Channel = reader.ReadString();
        Data = reader.ReadRestBuffer();
    }
}