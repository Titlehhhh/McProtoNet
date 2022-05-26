namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x5A, PacketCategory.Game, 754, PacketSide.Server)]
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

