namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4F, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerTitlePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTitlePacket() { }
    }
}

