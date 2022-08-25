namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x25, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerChangeHeldItemPacket : Packet
    {
        public short Slot { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteShort(Slot);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientPlayerChangeHeldItemPacket(short slot)
        {
            Slot = slot;
        }

        public ClientPlayerChangeHeldItemPacket() { }


    }
}
