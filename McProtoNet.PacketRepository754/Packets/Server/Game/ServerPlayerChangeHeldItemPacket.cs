namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x3F, 754, PacketSide.Server)]
    public sealed class ServerPlayerChangeHeldItemPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerChangeHeldItemPacket() { }
    }
}

