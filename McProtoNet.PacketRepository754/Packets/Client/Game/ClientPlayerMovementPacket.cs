namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x15, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerMovementPacket : IPacket
    {
        public bool OnGround { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteBoolean(OnGround);
        }
        public void Read(IMinecraftPrimitiveReader stream)
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
