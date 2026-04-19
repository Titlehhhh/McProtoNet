using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ClientCommand", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x04)]
[PacketId(751, 758, 0x04)]
[PacketId(759, 759, 0x06)]
[PacketId(760, 760, 0x07)]
[PacketId(761, 761, 0x06)]
[PacketId(762, 763, 0x07)]
[PacketId(764, 765, 0x08)]
[PacketId(766, 767, 0x09)]
[PacketId(768, 770, 0x0A)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x0B)]
public sealed partial class ClientCommandPacket : IPacket
{
    public int Name { get; set; }
    public int ActionId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Name);
        writer.WriteVarInt(ActionId);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Name = reader.ReadVarInt();
        ActionId = reader.ReadVarInt();
    }
}