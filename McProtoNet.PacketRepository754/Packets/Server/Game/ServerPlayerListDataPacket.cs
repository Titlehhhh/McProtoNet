namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x53, 754, PacketSide.Server)]
    public sealed class ServerPlayerListDataPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerListDataPacket() { }
    }
}

