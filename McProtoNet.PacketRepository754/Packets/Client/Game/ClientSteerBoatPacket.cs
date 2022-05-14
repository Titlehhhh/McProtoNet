namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x17, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientSteerBoatPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSteerBoatPacket() { }


    }
}
