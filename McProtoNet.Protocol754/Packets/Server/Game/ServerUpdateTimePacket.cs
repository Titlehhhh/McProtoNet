namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x4E, PacketCategory.Game, 754, PacketSide.Server)]
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

