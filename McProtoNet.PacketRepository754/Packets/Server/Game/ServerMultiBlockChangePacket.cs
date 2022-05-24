namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x3B, 754, PacketSide.Server)]
    public sealed class ServerMultiBlockChangePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerMultiBlockChangePacket() { }
    }
}

