namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x2D, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientSpectatePacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSpectatePacket() { }


    }
}
