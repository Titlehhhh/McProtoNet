namespace McProtoNet.Protocol;

/// <summary>
/// Exception thrown when a protocol is not supported
/// </summary>
public class ProtocolNotSupportException(string packetName, int protocolVersion) : Exception
{
    public string PacketName { get; } = packetName;
    public int ProtocolVersion { get; } = protocolVersion;
}

public class PacketDeserializationException(
    string? message,
    Exception innerException,
    string packetName,
    int protocolVersion)
    : Exception(message, innerException)
{
    public int ProtocolVersion { get; } = protocolVersion;
    public string PacketName { get; } = packetName;
}

public class PacketSerializationException(
    string? message,
    Exception innerException,
    string packetName,
    int protocolVersion)
    : Exception(message, innerException)
{
    public string PacketName { get; } = packetName;
    public int ProtocolVersion { get; } = protocolVersion;
}