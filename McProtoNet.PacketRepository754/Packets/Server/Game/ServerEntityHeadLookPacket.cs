namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x3A, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityHeadLookPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityHeadLookPacket() { }
    }
}

