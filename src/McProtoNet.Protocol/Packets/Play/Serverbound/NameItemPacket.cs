using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("NameItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1F)]
[PacketId(751, 758, 0x20)]
[PacketId(759, 759, 0x22)]
[PacketId(760, 763, 0x23)]
[PacketId(764, 764, 0x26)]
[PacketId(765, 765, 0x27)]
[PacketId(766, 767, 0x2A)]
[PacketId(768, 768, 0x2C)]
[PacketId(769, 770, 0x2E)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2F)]
public sealed partial class NameItemPacket : IClientPacket
{
    public string Name { get; set; } = string.Empty;

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Name);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Name = reader.ReadString();
    }
}