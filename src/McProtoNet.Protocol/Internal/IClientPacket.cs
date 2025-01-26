using System.Dynamic;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IClientPacket : IPacket
{
    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion);
    public static virtual bool SupportedVersion(int protocolVersion) => throw new NotImplementedException();
}