namespace McProtoNet.Protocol.Attributes;

/// <summary>
/// Specifies a range of protocol versions that the target type supports.
/// </summary>
/// <remarks>
/// Apply the attribute more than once to declare several ranges. The source generator reads the
/// declared ranges and emits a version check for the target type.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum, AllowMultiple = true)]
public sealed class ProtocolSupportAttribute : Attribute
{
    /// <summary>
    /// Gets the first protocol version of the range.
    /// </summary>
    public int From { get; }

    /// <summary>
    /// Gets the last protocol version of the range.
    /// </summary>
    public int To { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtocolSupportAttribute"/> class with the
    /// specified protocol version range.
    /// </summary>
    /// <param name="from">The first protocol version of the range.</param>
    /// <param name="to">The last protocol version of the range.</param>
    public ProtocolSupportAttribute(int from, int to)
    {
        From = from;
        To = to;
    }
}
