namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x5A, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerDeclareRecipesPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerDeclareRecipesPacket() { }
    }
}
