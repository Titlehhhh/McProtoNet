namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x29, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityRotationPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityRotationPacket() { }
    }
}

