namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x53, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerPlayerListDataPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerListDataPacket() { }
    }
}

