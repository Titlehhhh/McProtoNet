namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x1A, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerAbilitiesPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPlayerAbilitiesPacket() { }


    }
}
