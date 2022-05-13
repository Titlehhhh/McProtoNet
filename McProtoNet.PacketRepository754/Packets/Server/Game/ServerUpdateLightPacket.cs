namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x23, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerUpdateLightPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateLightPacket() { }
    }
}

