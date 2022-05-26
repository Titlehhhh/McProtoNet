namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x37, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerEntityRemoveEffectPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityRemoveEffectPacket() { }
    }
}

