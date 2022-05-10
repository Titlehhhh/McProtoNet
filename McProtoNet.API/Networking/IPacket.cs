using McProtoNet.API.IO;

namespace McProtoNet.API.Networking
{
    public interface IPacket
    {
        void Read(IMinecraftPrimitiveReader stream);
        void Write(IMinecraftPrimitiveWriter stream);
    }
}
