namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x50, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntitySoundEffectPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntitySoundEffectPacket() { }
    }
}

