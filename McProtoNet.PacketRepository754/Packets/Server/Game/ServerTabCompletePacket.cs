namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0F, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerTabCompletePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTabCompletePacket() { }
    }
}

