namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerRotationPacket : IPacket
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteFloat(Yaw);
            stream.WriteFloat(Pitch);
            stream.WriteBoolean(OnGround);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientPlayerRotationPacket(float yaw, float pitch, bool onGround)
        {
            Yaw = yaw;
            Pitch = pitch;
            OnGround = onGround;
        }
    }
}
