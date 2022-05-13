namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x16, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientVehicleMovePacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientVehicleMovePacket() { }


    }
}
