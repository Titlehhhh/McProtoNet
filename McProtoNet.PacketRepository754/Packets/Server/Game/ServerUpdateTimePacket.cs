namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4E, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerUpdateTimePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateTimePacket() { }
    }
}

