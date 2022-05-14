namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x4E, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerUpdateTimePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateTimePacket() { }
    }
}

