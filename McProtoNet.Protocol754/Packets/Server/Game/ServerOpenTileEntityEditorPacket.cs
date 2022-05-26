namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x2E, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerOpenTileEntityEditorPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerOpenTileEntityEditorPacket() { }
    }
}

