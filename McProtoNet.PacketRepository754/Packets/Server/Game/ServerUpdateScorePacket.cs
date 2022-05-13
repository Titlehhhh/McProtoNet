namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4D, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerUpdateScorePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateScorePacket() { }
    }
}

