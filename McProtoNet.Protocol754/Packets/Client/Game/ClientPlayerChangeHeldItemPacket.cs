namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x25, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerChangeHeldItemPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPlayerChangeHeldItemPacket() { }


    }
}
