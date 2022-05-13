namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x24, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerJoinGamePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerJoinGamePacket() { }
    }
}

