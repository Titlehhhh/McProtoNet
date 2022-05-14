namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x51, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerPlayBuiltinSoundPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayBuiltinSoundPacket() { }
    }
}

