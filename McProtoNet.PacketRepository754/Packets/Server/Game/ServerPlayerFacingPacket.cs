namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x33, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerPlayerFacingPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerFacingPacket() { }
    }
}

