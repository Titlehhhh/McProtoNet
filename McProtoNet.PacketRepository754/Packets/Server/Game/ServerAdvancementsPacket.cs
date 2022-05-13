namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x57, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerAdvancementsPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerAdvancementsPacket() { }
    }
}

