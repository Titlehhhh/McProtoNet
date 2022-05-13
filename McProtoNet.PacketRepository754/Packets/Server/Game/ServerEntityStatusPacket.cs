namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1A, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityStatusPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityStatusPacket() { }
    }
}

