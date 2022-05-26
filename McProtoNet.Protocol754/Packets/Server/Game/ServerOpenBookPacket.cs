namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x2C, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerOpenBookPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerOpenBookPacket() { }
    }
}

