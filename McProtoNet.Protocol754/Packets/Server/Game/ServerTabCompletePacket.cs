namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x0F, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerTabCompletePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerTabCompletePacket() { }
    }
}

