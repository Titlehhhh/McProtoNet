namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x2B, PacketCategory.Game, 754, PacketSide.Client)]
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
