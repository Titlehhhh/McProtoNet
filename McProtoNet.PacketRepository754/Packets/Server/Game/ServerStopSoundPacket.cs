namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x52, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerStopSoundPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerStopSoundPacket() { }
    }
}

