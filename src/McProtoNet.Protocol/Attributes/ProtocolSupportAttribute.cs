namespace McProtoNet.Protocol.Attributes;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true)]
public sealed class ProtocolSupportAttribute : Attribute
{
    public int From { get; }
    public int To { get; }

    public ProtocolSupportAttribute(int from, int to)
    {
        From = from;
        To = to;
    }
}