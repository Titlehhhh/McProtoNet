namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x26, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientUpdateCommandBlockPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientUpdateCommandBlockPacket() { }


    }
}
