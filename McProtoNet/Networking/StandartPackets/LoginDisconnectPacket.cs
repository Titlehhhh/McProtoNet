using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class LoginDisconnectPacket : Packet
    {
        public string Message { get; set; }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public LoginDisconnectPacket()
        {

        }

        public LoginDisconnectPacket(string message)
        {
            Message = message;
        }
    }
}
