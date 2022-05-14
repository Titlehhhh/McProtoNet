namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1C, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerUnloadChunkPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUnloadChunkPacket() { }
    }
}

