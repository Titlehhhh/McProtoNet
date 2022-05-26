namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x22, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientAdvancementTabPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientAdvancementTabPacket() { }


    }
}
