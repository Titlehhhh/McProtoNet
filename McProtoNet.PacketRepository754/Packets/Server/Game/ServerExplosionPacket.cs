namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1B, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerExplosionPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerExplosionPacket() { }
    }
}

