namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x27, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityPositionPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityPositionPacket() { }
    }
}

