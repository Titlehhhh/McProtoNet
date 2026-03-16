using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public interface IPacket
{
    static virtual PacketIdentifier PacketId => PacketIdentifier.Undefined;
    static virtual bool IsSupportedVersion(int protocolVersion) => false;
    static virtual ProtocolRange[] SupportedVersions => System.Array.Empty<ProtocolRange>();

    PacketIdentifier GetPacketId();

    /// <summary>Returns true if this packet exists in the given protocol version.</summary>
    bool IsVersionSupported(int protocolVersion) => false;

    /// <summary>Protocol version ranges in which this packet exists.</summary>
    ProtocolRange[] GetSupportedVersions() => System.Array.Empty<ProtocolRange>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => throw new NotSupportedException($"{GetType().Name} does not support serialization.");

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => throw new NotSupportedException($"{GetType().Name} does not support deserialization.");
}