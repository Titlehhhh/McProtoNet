namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4B, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntitySetPassengersPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntitySetPassengersPacket() { }
    }
}

