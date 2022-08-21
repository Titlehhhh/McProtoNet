using McProtoNet.Protocol754.Data.Window;

namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x28, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientCreativeInventoryActionPacket : Packet
    {
        public short Slot { get; private set; }
        public ItemStack Item { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteShort(Slot);
            stream.WriteItem(Item);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientCreativeInventoryActionPacket(short slot, ItemStack? item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            Slot = slot;
            Item = item;
        }

        public ClientCreativeInventoryActionPacket() { }


    }
}
