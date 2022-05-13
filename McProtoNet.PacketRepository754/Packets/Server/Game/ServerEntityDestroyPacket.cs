namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x36, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityDestroyPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityDestroyPacket() { }
    }
}

