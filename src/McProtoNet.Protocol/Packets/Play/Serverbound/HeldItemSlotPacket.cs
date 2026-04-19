using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("HeldItemSlot", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x24)]
[PacketId(751, 758, 0x25)]
[PacketId(759, 759, 0x27)]
[PacketId(760, 763, 0x28)]
[PacketId(764, 764, 0x2B)]
[PacketId(765, 765, 0x2C)]
[PacketId(766, 767, 0x2F)]
[PacketId(768, 768, 0x31)]
[PacketId(769, 770, 0x33)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x34)]
public sealed partial class HeldItemSlotPacket : IClientPacket
{
    public short SlotId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedShort(SlotId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => SlotId = reader.ReadSignedShort();
}