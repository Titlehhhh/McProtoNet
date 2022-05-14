using McProtoNet.IO;

namespace McProtoNet.Networking
{
    public abstract class Packet
    {
        public abstract void Read(IMinecraftPrimitiveReader stream);
        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
}
