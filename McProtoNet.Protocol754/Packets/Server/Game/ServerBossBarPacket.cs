namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x0C, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerBossBarPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBossBarPacket() { }
    }
}

