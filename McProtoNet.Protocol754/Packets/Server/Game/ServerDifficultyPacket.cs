namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x0D, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerDifficultyPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDifficultyPacket() { }
    }
}

