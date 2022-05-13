namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x05, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientSettingsPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSettingsPacket() { }


    }
}
