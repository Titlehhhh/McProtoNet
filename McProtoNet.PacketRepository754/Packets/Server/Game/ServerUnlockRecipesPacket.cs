namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x35, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerUnlockRecipesPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUnlockRecipesPacket() { }
    }
}

