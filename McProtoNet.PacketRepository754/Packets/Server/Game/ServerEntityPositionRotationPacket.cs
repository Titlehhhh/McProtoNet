namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x28, 754, PacketSide.Server)]
    public sealed class ServerEntityPositionRotationPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityPositionRotationPacket() { }
    }
}

