namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x18, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerPlaySoundPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlaySoundPacket() { }
    }
}

