namespace McProtoNet.Protocol;

public readonly struct ProtocolRange
{
    public int From { get; }
    public int To { get; }

    public ProtocolRange(int from, int to)
    {
        From = from;
        To = to;
    }

    public override string ToString()
        => From == To ? $"{From}" : $"{From}-{To}";

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
