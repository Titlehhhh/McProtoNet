namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x18, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientMoveItemToHotbarPacket : Packet
    {
        public int Slot { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Slot);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Slot = stream.ReadVarInt();
        }
        public ClientMoveItemToHotbarPacket() { }

        public ClientMoveItemToHotbarPacket(int Slot)
        {
            this.Slot = Slot;
        }
    }
}
