using ProtoLib.API.IO;

namespace ProtoLib.API.Networking
{
    public interface IPacket
    {
        void Read(IMinecraftStreamReader stream);
        void Write(IMinecraftStreamWriter stream);
    }
}
