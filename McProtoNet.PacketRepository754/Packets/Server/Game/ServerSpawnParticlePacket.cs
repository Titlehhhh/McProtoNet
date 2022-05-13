namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x22, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerSpawnParticlePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSpawnParticlePacket() { }
    }
}

