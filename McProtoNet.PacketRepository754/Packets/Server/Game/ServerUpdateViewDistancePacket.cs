namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x41, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerUpdateViewDistancePacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateViewDistancePacket() { }
    }
}
