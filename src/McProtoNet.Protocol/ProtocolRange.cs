namespace McProtoNet.Protocol;

/// <summary>
/// Represents an inclusive range of protocol numbers.
/// </summary>
public readonly struct ProtocolRange
{
    /// <summary>
    /// Gets the lowest protocol number in the range.
    /// </summary>
    public int From { get; }

    /// <summary>
    /// Gets the highest protocol number in the range.
    /// </summary>
    public int To { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtocolRange"/> structure with the specified bounds.
    /// </summary>
    /// <param name="from">The lowest protocol number in the range.</param>
    /// <param name="to">The highest protocol number in the range. This value must be greater than or
    /// equal to <paramref name="from"/>.</param>
    public ProtocolRange(int from, int to)
    {
        From = from;
        To = to;
    }

    /// <summary>
    /// Returns the string representation of the range.
    /// </summary>
    /// <returns>
    /// The single protocol number when both bounds are equal; otherwise, the two bounds separated by a
    /// hyphen.
    /// </returns>
    public override string ToString()
        => From == To ? $"{From}" : $"{From}-{To}";

    /// <summary>
    /// Determines whether the specified protocol number falls within any of the specified ranges.
    /// </summary>
    /// <param name="ranges">The ranges to test against.</param>
    /// <param name="protocol">The protocol number to locate.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="protocol"/> falls within one of
    /// <paramref name="ranges"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsSupported(ProtocolRange[] ranges, int protocol)
    {
        for (int i = 0; i < ranges.Length; i++)
        {
            var range = ranges[i];
            if ((uint)(protocol - range.From) <= (uint)(range.To - range.From))
            {
                return true;
            }
        }

        return false;
    }
}
