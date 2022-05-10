using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerPositionPacket : IPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool OnGround { get; set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteDouble(X);
            stream.WriteDouble(Y);
            stream.WriteDouble(Z);
            stream.WriteBoolean(OnGround);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientPlayerPositionPacket(double x, double y, double z, bool onGround)
        {
            X = x;
            Y = y;
            Z = z;
            OnGround = onGround;
        }
    }
}
