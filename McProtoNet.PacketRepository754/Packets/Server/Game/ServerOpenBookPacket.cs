namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x2C, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerOpenBookPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerOpenBookPacket() { }
    }
}

