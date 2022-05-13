namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x20, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerChunkDataPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerChunkDataPacket() { }
    }
}

