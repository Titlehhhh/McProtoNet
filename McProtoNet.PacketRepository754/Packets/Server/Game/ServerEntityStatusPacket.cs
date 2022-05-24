namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1A, 754, PacketSide.Server)]
    public sealed class ServerEntityStatusPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityStatusPacket() { }
    }
}

