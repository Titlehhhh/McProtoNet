namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x0D, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientEntityNBTRequestPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientEntityNBTRequestPacket() { }


    }
}
