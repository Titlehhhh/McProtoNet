namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x43, 754, PacketSide.Server)]
    public sealed class ServerDisplayScoreboardPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDisplayScoreboardPacket() { }
    }
}

