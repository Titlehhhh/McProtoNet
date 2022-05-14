namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4A, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerScoreboardObjectivePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerScoreboardObjectivePacket() { }
    }
}

