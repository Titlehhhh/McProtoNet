using McProtoNet.IO;

namespace McProtoNet.Networking
{
    public interface IPacket
    {
        void Read(IMinecraftPrimitiveReader stream);
        void Write(IMinecraftPrimitiveWriter stream);
    }
}
