namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x3A, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerEntityHeadLookPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityHeadLookPacket() { }
    }
}

