namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x2F, 754, PacketSide.Server)]
    public sealed class ServerPreparedCraftingGridPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPreparedCraftingGridPacket() { }
    }
}

