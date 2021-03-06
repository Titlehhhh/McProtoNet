namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x12, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerPositionPacket : Packet
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public bool OnGround { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteBoolean(OnGround);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            X = stream.ReadDouble();
            Y = stream.ReadDouble();
            Z = stream.ReadDouble();
            OnGround = stream.ReadBoolean();
        }
        public ClientPlayerPositionPacket() { }

        public ClientPlayerPositionPacket(double X, double Y, double Z, bool OnGround)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.OnGround = OnGround;
        }
    }
}
