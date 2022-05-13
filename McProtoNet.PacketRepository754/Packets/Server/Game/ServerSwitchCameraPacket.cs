namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x3E, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerSwitchCameraPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSwitchCameraPacket() { }
    }
}

