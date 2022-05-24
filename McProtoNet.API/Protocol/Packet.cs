using McProtoNet.API.IO;

namespace McProtoNet.API.Protocol
{
    public abstract class Packet
    {
        public abstract void Read(IMinecraftPrimitiveReader stream);
        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
}
