namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x25, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerChangeHeldItemPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPlayerChangeHeldItemPacket() { }


    }
}
