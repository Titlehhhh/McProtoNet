namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x15, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerMovementPacket : Packet
    {
        public bool OnGround { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteBoolean(OnGround);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            OnGround = stream.ReadBoolean();
        }
        public ClientPlayerMovementPacket() { }

        public ClientPlayerMovementPacket(bool OnGround)
        {
            this.OnGround = OnGround;
        }
    }
}
