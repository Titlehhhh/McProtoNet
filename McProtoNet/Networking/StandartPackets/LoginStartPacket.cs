using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class LoginStartPacket : IPacket
    {
        public string Nickname { get; private set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Nickname);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public LoginStartPacket(string nickname)
        {
            Nickname = nickname;
        }
        public LoginStartPacket()
        {

        }
    }
}
