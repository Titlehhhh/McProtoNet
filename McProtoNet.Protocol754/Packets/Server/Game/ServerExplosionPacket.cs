namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x1B, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerExplosionPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerExplosionPacket() { }
    }
}

