namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x42, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerSpawnPositionPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSpawnPositionPacket() { }
    }
}

