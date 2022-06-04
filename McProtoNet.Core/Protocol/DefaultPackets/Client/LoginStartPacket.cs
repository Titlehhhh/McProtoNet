using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{

    public sealed class LoginStartPacket : Packet
    {
        public string Nickname { get; private set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Nickname);
        }

        public override void Read(IMinecraftPrimitiveReader stream)
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
