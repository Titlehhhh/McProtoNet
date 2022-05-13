namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x38, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerResourcePackSendPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerResourcePackSendPacket() { }
    }
}

