namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x26, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerTradeListPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTradeListPacket() { }
    }
}

