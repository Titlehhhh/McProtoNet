using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId => PacketIdentifier.Undefined;
    static virtual bool IsSupportedVersionStatic(int protocolVersion) => false;
    static virtual ProtocolRange[] SupportedVersionsStatic => System.Array.Empty<ProtocolRange>();

    PacketIdentifier GetPacketId();
    bool IsSupportedVersion(int protocolVersion) => IsSupportedVersionStatic(protocolVersion);
    ProtocolRange[] SupportedVersions => SupportedVersionsStatic;

    void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => throw new NotSupportedException($"{GetType().Name} does not support serialization.");

    void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => throw new NotSupportedException($"{GetType().Name} does not support deserialization.");
}
