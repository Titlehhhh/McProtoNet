using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemBlockProperty
{
    public string Name { get; }
    public bool IsExactMatch { get; }
    public string? ExactValue { get; }
    public string? MinValue { get; }
    public string? MaxValue { get; }

    public ItemBlockProperty(string name, bool isExactMatch, string? exactValue, string? minValue, string? maxValue)
    {
        Name = name;
        IsExactMatch = isExactMatch;
        ExactValue = exactValue;
        MinValue = minValue;
        MaxValue = maxValue;
    }
}
