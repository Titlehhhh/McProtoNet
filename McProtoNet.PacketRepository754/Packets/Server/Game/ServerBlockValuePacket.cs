namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0A, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerBlockValuePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBlockValuePacket() { }
    }
}

