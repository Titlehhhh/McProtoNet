namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x2B, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientUpdateSignPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientUpdateSignPacket() { }


    }
}
