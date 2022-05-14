namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x44, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerEntityMetadataPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityMetadataPacket() { }
    }
}

