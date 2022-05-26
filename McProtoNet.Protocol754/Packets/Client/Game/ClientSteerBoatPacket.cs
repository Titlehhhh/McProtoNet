namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x17, PacketCategory.Game, 754, PacketSide.Client)]
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
