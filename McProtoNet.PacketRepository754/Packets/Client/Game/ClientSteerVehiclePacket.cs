namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x1D, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientSteerVehiclePacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSteerVehiclePacket() { }


    }
}
