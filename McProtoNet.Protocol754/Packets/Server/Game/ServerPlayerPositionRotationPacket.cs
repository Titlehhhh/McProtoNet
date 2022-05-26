namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x34, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerPlayerPositionRotationPacket : Packet
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool IsGround { get; set; }

        public int TeleportId { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerPlayerPositionRotationPacket() { }
    }
}

