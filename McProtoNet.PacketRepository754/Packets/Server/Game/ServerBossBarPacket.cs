namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0C, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerBossBarPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBossBarPacket() { }
    }
}

