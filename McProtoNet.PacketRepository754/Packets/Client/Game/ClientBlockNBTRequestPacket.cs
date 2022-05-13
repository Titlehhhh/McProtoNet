namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x01, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientBlockNBTRequestPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientBlockNBTRequestPacket() { }


    }
}
