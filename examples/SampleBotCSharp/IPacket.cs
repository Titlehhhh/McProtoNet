using McProtoNet.Serialization;

namespace SampleBotCSharp;

public interface IPacket
{
    void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion);
}