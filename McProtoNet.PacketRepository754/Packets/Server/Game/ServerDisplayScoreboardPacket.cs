namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x43, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerDisplayScoreboardPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDisplayScoreboardPacket() { }
    }
}

