namespace McProtoNet.Protocol;

/// <summary>
/// Exception thrown when a protocol is not supported
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
        var supported = string.Join(", ", ranges);
        return $"Type {typeName} is not supported for protocol {actualVersion}. Supported: {supported}";
    }
}
