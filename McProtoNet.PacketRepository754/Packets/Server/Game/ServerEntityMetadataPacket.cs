namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x44, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityMetadataPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityMetadataPacket() { }
    }
}

