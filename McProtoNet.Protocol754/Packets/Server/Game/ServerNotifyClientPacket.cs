namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x1D, PacketCategory.Game, 754, PacketSide.Server)]
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

