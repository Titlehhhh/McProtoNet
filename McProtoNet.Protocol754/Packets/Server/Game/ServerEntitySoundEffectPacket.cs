namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x50, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerEntitySoundEffectPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntitySoundEffectPacket() { }
    }
}

