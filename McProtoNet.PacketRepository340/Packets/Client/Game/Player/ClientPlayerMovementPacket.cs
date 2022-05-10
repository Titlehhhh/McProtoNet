using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerMovementPacket : IPacket
    {
        public bool OnGround { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteBoolean(OnGround);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientPlayerMovementPacket()
        {

        }
        public ClientPlayerMovementPacket(bool onGround)
        {
            OnGround = onGround;
        }
    }
}
