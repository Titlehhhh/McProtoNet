namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x54, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerNBTResponsePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerNBTResponsePacket() { }
    }
}

