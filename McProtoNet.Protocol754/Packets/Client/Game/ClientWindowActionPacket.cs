using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x09, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientWindowActionPacket : Packet
    {
        public byte WindowId { get; set; }
        public short Slot { get; set; }       

        public byte Button { get; set; }
        public short ActionId { get; set; }
        public byte Mode { get; set; }

        public ItemStack? ClickedItem { get; set; }
        // public WindowActionParam MyProperty { get;  set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteUnsignedByte(WindowId);
            stream.WriteShort(Slot);


            stream.WriteByte(0);
            stream.WriteShort(ActionId);
            stream.WriteByte(0);

            stream.WriteItem(ClickedItem);

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientWindowActionPacket(byte windowId, short slot, byte button, short actionId, byte mode, ItemStack? clickedItem)
        {
            WindowId = windowId;
            Slot = slot;
            Button = button;
            ActionId = actionId;
            Mode = mode;
            ClickedItem = clickedItem;
        }

        public ClientWindowActionPacket() { }


    }
}
