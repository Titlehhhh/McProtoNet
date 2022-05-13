namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x5B, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerDeclareTagsPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDeclareTagsPacket() { }
    }
}

