namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x08, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerBlockBreakAnimPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBlockBreakAnimPacket() { }
    }
}

