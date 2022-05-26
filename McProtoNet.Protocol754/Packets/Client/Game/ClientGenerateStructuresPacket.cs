namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x0F, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientGenerateStructuresPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientGenerateStructuresPacket() { }


    }
}
