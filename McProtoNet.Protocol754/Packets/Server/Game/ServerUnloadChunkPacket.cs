namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x1C, PacketCategory.Game, 754, PacketSide.Server)]
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

