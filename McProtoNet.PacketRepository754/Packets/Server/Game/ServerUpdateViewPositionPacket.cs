namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x40, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerUpdateViewPositionPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateViewPositionPacket() { }
    }
}

