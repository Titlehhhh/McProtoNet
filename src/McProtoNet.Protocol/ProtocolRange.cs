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
}