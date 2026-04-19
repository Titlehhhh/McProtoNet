using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityEquipment", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x47)]
[PacketId(751, 754, 0x47)]
[PacketId(755, 759, 0x50)]
[PacketId(760, 760, 0x53)]
[PacketId(761, 761, 0x51)]
[PacketId(762, 763, 0x55)]
[PacketId(764, 764, 0x57)]
[PacketId(765, 765, 0x59)]
[PacketId(766, 767, 0x5B)]
[PacketId(768, 769, 0x60)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x5F)]
public sealed partial class EntityEquipmentPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Equipment[] Equipments { get; set; }

    public struct Equipment
    {
        public sbyte Slot { get; set; }
        public Slot Item { get; set; }
    }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        for (int i = 0; i < Equipments.Length; i++)
        {
            var entry = Equipments[i];
            bool isLast = i == Equipments.Length - 1;
            sbyte slotByte = entry.Slot;
            if (!isLast)
                slotByte = (sbyte)(slotByte | 0x80);
            writer.WriteSignedByte(slotByte);
            writer.WriteType<Slot>(entry.Item, protocolVersion);
        }
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EntityEquipmentPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        var list = new System.Collections.Generic.List<Equipment>();
        while (true)
        {
            sbyte slotByte = reader.ReadSignedByte();
            bool hasMore = (slotByte & 0x80) != 0;
            sbyte slot = (sbyte)(slotByte & 0x7F);
            var item = reader.ReadType<Slot>(protocolVersion);
            list.Add(new Equipment { Slot = slot, Item = item });
            if (!hasMore)
                break;
        }
        Equipments = list.ToArray();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EntityEquipmentPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}