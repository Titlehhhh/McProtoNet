namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x2A, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientUpdateStructureBlockPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientUpdateStructureBlockPacket() { }


    }
}
