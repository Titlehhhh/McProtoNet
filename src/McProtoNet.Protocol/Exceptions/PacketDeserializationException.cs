namespace McProtoNet.Protocol;

/// <summary>
/// Exception thrown when a packet cannot be deserialized
/// </summary>
/// <param name="message"></param>
/// <param name="innerException"></param>
/// <param name="packetName"></param>
/// <param name="protocolVersion"></param>
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