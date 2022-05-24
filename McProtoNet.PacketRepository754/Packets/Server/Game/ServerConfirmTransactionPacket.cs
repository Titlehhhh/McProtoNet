namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x11, 754, PacketSide.Server)]
    public sealed class ServerConfirmTransactionPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerConfirmTransactionPacket() { }
    }
}

