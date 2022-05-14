namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x35, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerUnlockRecipesPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerUnlockRecipesPacket() { }
    }
}

