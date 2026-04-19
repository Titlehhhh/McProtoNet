using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SelectTrade", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x22)]
[PacketId(751, 758, 0x23)]
[PacketId(759, 759, 0x25)]
[PacketId(760, 763, 0x26)]
[PacketId(764, 764, 0x29)]
[PacketId(765, 765, 0x2A)]
[PacketId(766, 767, 0x2D)]
[PacketId(768, 768, 0x2F)]
[PacketId(769, 770, 0x31)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x32)]
public sealed partial class SelectTradePacket : IClientPacket
{
    public Slot Name { get; set; } = default!;

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteType<Slot>(Name, protocolVersion);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadType<Slot>(protocolVersion);
}