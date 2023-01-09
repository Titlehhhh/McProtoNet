using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{
    public interface IOutputPacket
    {
        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
    public interface IInputPacket
    {
        public abstract void Read(IMinecraftPrimitiveReader stream);
    }

    public abstract class Packet : IOutputPacket, IInputPacket
    {
        public abstract void Read(IMinecraftPrimitiveReader stream);

        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
}
