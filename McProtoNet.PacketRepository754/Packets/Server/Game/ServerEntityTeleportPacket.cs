namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x56, 754, PacketSide.Server)]
    public sealed class ServerEntityTeleportPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityTeleportPacket() { }
    }
}

