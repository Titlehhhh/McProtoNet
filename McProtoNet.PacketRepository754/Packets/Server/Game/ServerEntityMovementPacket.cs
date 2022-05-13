namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x2A, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityMovementPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityMovementPacket() { }
    }
}

