namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x07, 754, PacketSide.Server)]
    public sealed class ServerPlayerActionAckPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerActionAckPacket() { }
    }
}

