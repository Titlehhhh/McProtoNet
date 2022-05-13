namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x13, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerWindowItemsPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerWindowItemsPacket() { }
    }
}

