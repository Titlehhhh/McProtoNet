namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x06, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerStatisticsPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerStatisticsPacket() { }
    }
}

