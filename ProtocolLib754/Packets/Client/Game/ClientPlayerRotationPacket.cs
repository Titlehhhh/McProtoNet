using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x14, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerRotationPacket : IPacket
    {
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public bool OnGround { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteFloat(Yaw);
            stream.WriteFloat(Pitch);
            stream.WriteBoolean(OnGround);
        }
        public void Read(IMinecraftStreamReader stream)
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
