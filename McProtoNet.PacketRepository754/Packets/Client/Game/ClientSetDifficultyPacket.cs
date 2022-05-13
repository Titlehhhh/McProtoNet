namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x02, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientSetDifficultyPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSetDifficultyPacket() { }


    }
}
