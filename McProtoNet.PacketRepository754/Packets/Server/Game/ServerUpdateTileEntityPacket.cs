namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x09, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerUpdateTileEntityPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateTileEntityPacket() { }
    }
}

