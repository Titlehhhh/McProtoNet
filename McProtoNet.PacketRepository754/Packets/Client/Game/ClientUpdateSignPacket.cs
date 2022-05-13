namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x2B, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientUpdateSignPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientUpdateSignPacket() { }


    }
}
