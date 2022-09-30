using McProtoNet.Protocol754.Data;

namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x13, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerWindowItemsPacket : Packet
    {
        public byte WindowId { get; private set; }
        public ItemStack?[] Items { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            WindowId = stream.ReadUnsignedByte();
            
            Items = new ItemStack?[stream.ReadShort()];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = stream.ReadItem();
            }
        }
        public ServerWindowItemsPacket() { }
    }
}

