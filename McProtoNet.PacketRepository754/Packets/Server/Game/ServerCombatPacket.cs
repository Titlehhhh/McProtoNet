namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x31, 754, PacketSide.Server)]
    public sealed class ServerCombatPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerCombatPacket() { }
    }
}

