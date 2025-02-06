using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IServerPacket : IPacket
{
    void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}