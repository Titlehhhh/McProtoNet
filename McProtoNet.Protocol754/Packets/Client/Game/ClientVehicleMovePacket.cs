namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x16, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientVehicleMovePacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientVehicleMovePacket() { }


    }
}
