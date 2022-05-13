namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x23, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientSelectTradePacket : IPacket
    {
        public int Slot { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Slot);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Slot = stream.ReadVarInt();
        }
        public ClientSelectTradePacket() { }

        public ClientSelectTradePacket(int Slot)
        {
            this.Slot = Slot;
        }
    }
}
