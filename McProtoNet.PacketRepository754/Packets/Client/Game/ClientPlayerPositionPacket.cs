using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x12, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerPositionPacket : IPacket
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public bool OnGround { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteBoolean(OnGround);
        }
        public void Read(IMinecraftPrimitiveReader stream)
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
