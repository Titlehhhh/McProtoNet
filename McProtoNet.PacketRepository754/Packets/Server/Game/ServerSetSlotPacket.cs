namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x15, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerSetSlotPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSetSlotPacket() { }
    }
}

