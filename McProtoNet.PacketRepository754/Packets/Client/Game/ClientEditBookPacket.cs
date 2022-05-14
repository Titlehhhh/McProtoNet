namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x0C, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientEditBookPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientEditBookPacket() { }


    }
}
