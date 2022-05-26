namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x1E, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerOpenHorseWindowPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerOpenHorseWindowPacket() { }
    }
}

