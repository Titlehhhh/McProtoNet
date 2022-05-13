namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x26, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientUpdateCommandBlockPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientUpdateCommandBlockPacket() { }


    }
}
