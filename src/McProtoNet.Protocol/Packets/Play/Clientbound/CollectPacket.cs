using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Collect", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x55)]
[PacketId(751, 754, 0x55)]
[PacketId(755, 756, 0x60)]
[PacketId(757, 758, 0x61)]
[PacketId(759, 759, 0x62)]
[PacketId(760, 760, 0x65)]
[PacketId(761, 761, 0x63)]
[PacketId(762, 763, 0x67)]
[PacketId(764, 764, 0x6A)]
[PacketId(765, 765, 0x6C)]
[PacketId(766, 767, 0x6F)]
[PacketId(768, 769, 0x76)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x75)]
public sealed partial class CollectPacket : IServerPacket
{
    public int CollectedEntityId { get; set; }
    public int CollectorEntityId { get; set; }
    public int PickupItemCount { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(CollectedEntityId);
        writer.WriteVarInt(CollectorEntityId);
        writer.WriteVarInt(PickupItemCount);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        CollectedEntityId = reader.ReadVarInt();
        CollectorEntityId = reader.ReadVarInt();
        PickupItemCount = reader.ReadVarInt();
    }
}