namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x0D, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientEntityNBTRequestPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientEntityNBTRequestPacket() { }


    }
}
