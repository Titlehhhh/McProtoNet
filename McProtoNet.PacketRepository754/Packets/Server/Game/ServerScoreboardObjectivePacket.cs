namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4A, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerScoreboardObjectivePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerScoreboardObjectivePacket() { }
    }
}

