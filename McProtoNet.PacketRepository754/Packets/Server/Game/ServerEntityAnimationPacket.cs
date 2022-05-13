namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x05, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityAnimationPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityAnimationPacket() { }
    }
}

