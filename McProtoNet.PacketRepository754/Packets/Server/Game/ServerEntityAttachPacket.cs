namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x45, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityAttachPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityAttachPacket() { }
    }
}

