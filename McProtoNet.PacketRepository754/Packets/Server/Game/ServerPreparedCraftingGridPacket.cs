namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x2F, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerPreparedCraftingGridPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPreparedCraftingGridPacket() { }
    }
}

