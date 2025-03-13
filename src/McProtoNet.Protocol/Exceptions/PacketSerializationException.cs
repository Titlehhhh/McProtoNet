namespace McProtoNet.Protocol;
/// <summary>
/// Exception thrown when a packet fails to serialize
/// </summary>
/// <param name="message"></param>
/// <param name="innerException"></param>
/// <param name="packetName"></param>
/// <param name="protocolVersion"></param>
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