using McProtoNet.API.IO;

namespace McProtoNet.API.Networking
{
    public interface IPacket
    {
        void Read(IMinecraftStreamReader stream);
        void Write(IMinecraftStreamWriter stream);
    }
}
