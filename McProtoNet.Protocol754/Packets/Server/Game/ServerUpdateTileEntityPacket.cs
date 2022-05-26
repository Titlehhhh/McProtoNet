namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x09, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerUpdateTileEntityPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUpdateTileEntityPacket() { }
    }
}

