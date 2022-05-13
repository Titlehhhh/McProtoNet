namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1D, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerNotifyClientPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerNotifyClientPacket() { }
    }
}

