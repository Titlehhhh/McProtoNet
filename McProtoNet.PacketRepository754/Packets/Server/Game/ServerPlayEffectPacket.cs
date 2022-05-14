namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x21, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerPlayEffectPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayEffectPacket() { }
    }
}

