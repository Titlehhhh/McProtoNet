namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x47, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerEntityEquipmentPacket : Packet
    {
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityEquipmentPacket() { }
    }
}

