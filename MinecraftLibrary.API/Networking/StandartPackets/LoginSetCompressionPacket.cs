using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;

namespace ProtoLib.API
{

    public sealed class LoginSetCompressionPacket : IPacket
    {
        public int Threshold { get; set; }

        public void Read(IMinecraftStreamReader stream)
        {
            Threshold = stream.ReadVarInt();
        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }
        public LoginSetCompressionPacket()
        {

        }
    }
}
