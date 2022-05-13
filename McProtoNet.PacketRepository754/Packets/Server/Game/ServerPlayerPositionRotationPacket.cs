namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x34, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerPlayerPositionRotationPacket : IPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool IsGround { get; set; }

        public int TeleportId { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerPositionRotationPacket() { }
    }
}

