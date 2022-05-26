namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x4B, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerEntitySetPassengersPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntitySetPassengersPacket() { }
    }
}

