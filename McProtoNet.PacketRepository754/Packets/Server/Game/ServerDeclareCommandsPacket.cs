namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x10, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerDeclareCommandsPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDeclareCommandsPacket() { }
    }
}

