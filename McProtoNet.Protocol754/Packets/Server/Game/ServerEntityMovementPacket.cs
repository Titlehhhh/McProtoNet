namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x2A, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerEntityMovementPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityMovementPacket() { }
    }
}

