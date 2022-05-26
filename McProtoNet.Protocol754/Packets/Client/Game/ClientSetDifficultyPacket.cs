namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x02, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientSetDifficultyPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSetDifficultyPacket() { }


    }
}
