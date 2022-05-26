namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x26, PacketCategory.Game, 754, PacketSide.Client)]
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
