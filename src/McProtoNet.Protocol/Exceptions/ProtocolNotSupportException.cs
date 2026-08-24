namespace McProtoNet.Protocol;

/// <summary>
/// The exception that is thrown when a protocol type or a packet does not exist on the protocol
/// version in use.
/// </summary>
/// <remarks>
/// The exception is raised on read, on write, and on a typed send whose packet has no id for the
/// protocol version of the connection.
/// </remarks>
public sealed class ProtocolNotSupportException : Exception
{
    /// <summary>
    /// Gets the name of the type or packet that is not supported.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the protocol number that was in use when the exception was raised.
    /// </summary>
    public int ActualVersion { get; }

    /// <summary>
    /// Gets the protocol ranges on which the type or packet exists.
    /// </summary>
    /// <value>
    /// The supported ranges, or an empty list when they are not known.
    /// </value>
    public IReadOnlyList<ProtocolRange> SupportedRanges { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtocolNotSupportException"/> class with the
    /// specified type name and protocol number.
    /// </summary>
    /// <param name="typeName">The name of the type or packet that is not supported.</param>
    /// <param name="actualVersion">The protocol number that was in use.</param>
    public ProtocolNotSupportException(string typeName, int actualVersion)
        : this(typeName, actualVersion, Array.Empty<ProtocolRange>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtocolNotSupportException"/> class with the
    /// specified type name, protocol number and supported ranges.
    /// </summary>
    /// <param name="typeName">The name of the type or packet that is not supported.</param>
    /// <param name="actualVersion">The protocol number that was in use.</param>
    /// <param name="supportedRanges">The protocol ranges on which the type or packet exists. The
    /// ranges are listed in the message of the exception.</param>
    public ProtocolNotSupportException(
        string typeName,
        int actualVersion,
        IReadOnlyList<ProtocolRange> supportedRanges)
        : base(BuildMessage(typeName, actualVersion, supportedRanges))
    {
        TypeName = typeName;
        ActualVersion = actualVersion;
        SupportedRanges = supportedRanges;
    }

    private static string BuildMessage(
        string typeName,
        int actualVersion,
        IReadOnlyList<ProtocolRange> ranges)
    {
        if (ranges.Count == 0) return $"{typeName} is not supported for protocol {actualVersion}";

        var supported = string.Join(", ", ranges);
        return $"{typeName} is not supported for protocol {actualVersion}. Supported: {supported}";
    }
}
