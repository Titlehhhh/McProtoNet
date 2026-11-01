namespace McProtoNet.Protocol;

/// <summary>
/// The exception that is thrown when a packet cannot be decoded into its packet type.
/// </summary>
public sealed class PacketDecodeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketDecodeException"/> class with the specified
    /// packet type, error and inner exception.
    /// </summary>
    /// <param name="packetType">The packet type that failed to decode.</param>
    /// <param name="error">The reason the decode failed.</param>
    /// <param name="inner">The exception that caused the failure, or <see langword="null"/> if there
    /// is no such exception.</param>
    public PacketDecodeException(Type packetType, DecodeError error, Exception? inner = null)
        : base($"Failed to decode {packetType.Name}: {error}.", inner)
    {
        PacketType = packetType;
        Error = error;
    }

    /// <summary>
    /// Gets the packet type that failed to decode.
    /// </summary>
    public Type PacketType { get; }

    /// <summary>
    /// Gets the reason the decode failed.
    /// </summary>
    public DecodeError Error { get; }
}
