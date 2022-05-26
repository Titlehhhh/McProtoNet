namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x1D, PacketCategory.Game, 754, PacketSide.Client)]
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
