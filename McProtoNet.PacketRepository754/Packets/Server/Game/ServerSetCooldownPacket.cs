namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x16, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerSetCooldownPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSetCooldownPacket() { }
    }
}

