namespace McProtoNet.Protocol;

/// <summary>
/// Thrown when a type or a packet does not exist on the protocol version in play — on read, on
/// write, and on a typed send whose packet has no id for that version.
/// </summary>
public sealed class ProtocolNotSupportException : Exception
{
    public string TypeName { get; }
    public int ActualVersion { get; }
    public IReadOnlyList<ProtocolRange> SupportedRanges { get; }

    public ProtocolNotSupportException(string typeName, int actualVersion)
        : this(typeName, actualVersion, Array.Empty<ProtocolRange>())
    {
    }

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
