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
    public interface IMinecraftPacket : IOutputPacket, IInputPacket
    {
    }

    public abstract class Packet<TProto> : IMinecraftPacket where TProto : IProtocol
    {
        public abstract void Read(IMinecraftPrimitiveReader stream);

        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
}
