using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Client
{

    public sealed class LoginStartPacket : IMinecraftPacket
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
