namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x5A, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerDeclareRecipesPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDeclareRecipesPacket() { }
    }
}

