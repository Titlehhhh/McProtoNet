namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x31, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerCombatPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerCombatPacket() { }
    }
}

