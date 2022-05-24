namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x33, 754, PacketSide.Server)]
    public sealed class ServerPlayerFacingPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerFacingPacket() { }
    }
}

