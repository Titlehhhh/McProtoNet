namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0B, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerBlockChangePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBlockChangePacket() { }
    }
}

