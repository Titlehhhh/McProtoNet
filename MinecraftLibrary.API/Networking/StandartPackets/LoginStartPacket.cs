using ProtoLib.API.IO;
using ProtoLib.API.Networking;

namespace ProtoLib.API
{

    public sealed class LoginStartPacket : IPacket
    {
        public string Nickname { get; set; }
        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteString(Nickname);
        }

        public void Read(IMinecraftStreamReader stream)
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
