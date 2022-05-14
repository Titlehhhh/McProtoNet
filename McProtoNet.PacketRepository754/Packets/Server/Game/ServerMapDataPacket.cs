namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x25, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerMapDataPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerMapDataPacket() { }
    }
}

