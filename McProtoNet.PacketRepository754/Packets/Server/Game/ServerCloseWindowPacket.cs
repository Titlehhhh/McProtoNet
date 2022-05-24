namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x12, 754, PacketSide.Server)]
    public sealed class ServerCloseWindowPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerCloseWindowPacket() { }
    }
}

