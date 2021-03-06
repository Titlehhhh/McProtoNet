namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x14, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerRotationPacket : Packet
    {
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public bool OnGround { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteFloat(Yaw);
            stream.WriteFloat(Pitch);
            stream.WriteBoolean(OnGround);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Yaw = stream.ReadFloat();
            Pitch = stream.ReadFloat();
            OnGround = stream.ReadBoolean();
        }
        public ClientPlayerRotationPacket() { }

        public ClientPlayerRotationPacket(float Yaw, float Pitch, bool OnGround)
        {
            this.Yaw = Yaw;
            this.Pitch = Pitch;
            this.OnGround = OnGround;
        }
    }
}
