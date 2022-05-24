namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0B, 754, PacketSide.Server)]
    public sealed class ServerBlockChangePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBlockChangePacket() { }
    }
}

