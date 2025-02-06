using System.Dynamic;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IClientPacket : IPacket
{
    void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion);
}