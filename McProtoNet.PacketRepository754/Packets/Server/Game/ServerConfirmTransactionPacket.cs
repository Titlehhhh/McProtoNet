namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x11, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerConfirmTransactionPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerConfirmTransactionPacket() { }
    }
}

