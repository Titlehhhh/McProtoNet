namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x40, 754, PacketSide.Server)]
    public sealed class ServerUpdateViewPositionPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateViewPositionPacket() { }
    }
}

