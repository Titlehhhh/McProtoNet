namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x11, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientLockDifficultyPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientLockDifficultyPacket() { }


    }
}
