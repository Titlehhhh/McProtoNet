namespace McProtoNet.Protocol;

/// <summary>Typed decode failed; <see cref="Error" /> says how, inner exception says where.</summary>
public sealed class PacketDecodeException : Exception
{
    public PacketDecodeException(Type packetType, DecodeError error, Exception? inner = null)
        : base($"Failed to decode {packetType.Name}: {error}.", inner)
    {
        PacketType = packetType;
        Error = error;
    }

    public Type PacketType { get; }
    public DecodeError Error { get; }
}

/// <summary>
///     Write of a version-layered packet whose group for the target version is not filled:
///     the packet was assembled wrongly, there is nothing to send.
/// </summary>
public sealed class WrongLayerException : Exception
{
    public WrongLayerException(string packetName, int protocolVersion, string expectedLayer)
        : base($"{packetName}: protocol {protocolVersion} requires layer {expectedLayer}, but it is null.")
    {
        PacketName = packetName;
        ProtocolVersion = protocolVersion;
        ExpectedLayer = expectedLayer;
    }

    public string PacketName { get; }
    public int ProtocolVersion { get; }
    public string ExpectedLayer { get; }
}
