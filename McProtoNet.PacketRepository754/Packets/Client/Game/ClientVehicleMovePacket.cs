namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x16, 754, PacketCategory.Game, PacketSide.Client)]
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
