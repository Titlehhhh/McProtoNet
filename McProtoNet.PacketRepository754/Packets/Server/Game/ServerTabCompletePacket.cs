namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0F, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerTabCompletePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTabCompletePacket() { }
    }
}

