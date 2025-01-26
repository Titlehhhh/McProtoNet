using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IServerPacket : IPacket
{
    public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    public static virtual bool VersionSupported(int protocolVersion)
    {
        throw new NotImplementedException();
    }
}