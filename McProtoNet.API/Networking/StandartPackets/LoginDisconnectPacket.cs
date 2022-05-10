using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.API
{

    public sealed class LoginDisconnectPacket : IPacket
    {
        public string Message { get; set; }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
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
