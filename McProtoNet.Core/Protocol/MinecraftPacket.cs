using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{
    public abstract class MinecraftPacket : IOutputPacket, IInputPacket
    {       
        public abstract void Read(IMinecraftPrimitiveReader stream);

        public abstract void Write(IMinecraftPrimitiveWriter stream);
    }
    public abstract class MinecraftPacket<TProto> : MinecraftPacket where TProto : IProtocol
    {

    }
}
