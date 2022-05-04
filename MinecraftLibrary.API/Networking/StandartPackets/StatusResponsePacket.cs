using ProtoLib.API.IO;
using ProtoLib.API.Networking;

namespace ProtoLib.API
{
    public sealed class StatusResponsePacket : IPacket
    {

        public void Read(IMinecraftStreamReader stream)
        {
            throw new NotImplementedException();
            string debug = stream.ReadString();
        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
