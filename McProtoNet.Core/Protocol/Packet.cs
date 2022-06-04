using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{
    public abstract class Packet
    {
        public abstract void Read(IMinecraftPrimitiveReader stream);
        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
}
