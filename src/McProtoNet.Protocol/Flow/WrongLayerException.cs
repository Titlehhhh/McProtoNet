namespace McProtoNet.Protocol;

/// <summary>
/// The exception that is thrown when a version-layered packet is written and the layer required for
/// the target protocol version is <see langword="null"/>.
/// </summary>
public sealed class WrongLayerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WrongLayerException"/> class with the specified
    /// packet name, protocol version and expected layer.
    /// </summary>
    /// <param name="packetName">The name of the packet that was written.</param>
    /// <param name="protocolVersion">The protocol version the packet was written for.</param>
    /// <param name="expectedLayer">The name of the layer that the protocol version requires.</param>
    public WrongLayerException(string packetName, int protocolVersion, string expectedLayer)
        : base($"{packetName}: protocol {protocolVersion} requires layer {expectedLayer}, but it is null.")
    {
        PacketName = packetName;
        ProtocolVersion = protocolVersion;
        ExpectedLayer = expectedLayer;
    }

    /// <summary>
    /// Gets the name of the packet that was written.
    /// </summary>
    public string PacketName { get; }

    /// <summary>
    /// Gets the protocol version the packet was written for.
    /// </summary>
    public int ProtocolVersion { get; }

    /// <summary>
    /// Gets the name of the layer that the protocol version requires.
    /// </summary>
    public string ExpectedLayer { get; }
}
