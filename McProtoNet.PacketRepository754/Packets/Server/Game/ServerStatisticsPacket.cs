namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x06, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerStatisticsPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerStatisticsPacket() { }
    }
}

