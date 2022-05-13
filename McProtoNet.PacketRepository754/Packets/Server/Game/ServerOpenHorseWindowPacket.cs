namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1E, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerOpenHorseWindowPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerOpenHorseWindowPacket() { }
    }
}

