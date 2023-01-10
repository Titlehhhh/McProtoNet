namespace McProtoNet.Protocol754.Packets.Client
{


    public sealed class ClientPlayerChangeHeldItemPacket : MinecraftPacket<Protocol754>
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
