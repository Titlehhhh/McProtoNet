namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4C, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerTeamPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTeamPacket() { }
    }
}

