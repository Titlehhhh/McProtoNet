namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x03, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerSpawnPaintingPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSpawnPaintingPacket() { }
    }
}

