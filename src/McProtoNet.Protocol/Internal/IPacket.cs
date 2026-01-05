using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId => PacketIdentifier.Undefined;
    static virtual bool IsSupportedVersionStatic(int protocolVersion) => false;

    PacketIdentifier GetPacketId();
    bool IsSupportedVersion(int protocolVersion);

    void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => throw new NotSupportedException($"{GetType().Name} does not support serialization.");

    void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => throw new NotSupportedException($"{GetType().Name} does not support deserialization.");
}
