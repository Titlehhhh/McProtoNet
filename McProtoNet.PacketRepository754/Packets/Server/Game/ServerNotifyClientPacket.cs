namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1D, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerNotifyClientPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerNotifyClientPacket() { }
    }
}

