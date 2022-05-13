namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x2E, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerOpenTileEntityEditorPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerOpenTileEntityEditorPacket() { }
    }
}

