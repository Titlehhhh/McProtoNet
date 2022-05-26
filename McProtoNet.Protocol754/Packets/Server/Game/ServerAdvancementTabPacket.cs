namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x3C, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerAdvancementTabPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerAdvancementTabPacket() { }
    }
}

