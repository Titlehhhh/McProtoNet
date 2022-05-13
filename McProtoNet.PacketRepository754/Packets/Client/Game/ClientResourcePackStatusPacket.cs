namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x21, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientResourcePackStatusPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientResourcePackStatusPacket() { }


    }
}
