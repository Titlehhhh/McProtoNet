namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x46, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityVelocityPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityVelocityPacket() { }
    }
}

