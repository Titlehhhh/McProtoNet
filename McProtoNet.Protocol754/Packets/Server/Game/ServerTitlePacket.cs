namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x4F, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerTitlePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTitlePacket() { }
    }
}

