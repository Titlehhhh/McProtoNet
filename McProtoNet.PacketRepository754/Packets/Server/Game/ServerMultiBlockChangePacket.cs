namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x3B, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerMultiBlockChangePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerMultiBlockChangePacket() { }
    }
}

