namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x16, 754, PacketSide.Server)]
    public sealed class ServerSetCooldownPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerSetCooldownPacket() { }
    }
}

