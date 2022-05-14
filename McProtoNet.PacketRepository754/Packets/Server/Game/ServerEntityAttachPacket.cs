namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x45, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerEntityAttachPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityAttachPacket() { }
    }
}

