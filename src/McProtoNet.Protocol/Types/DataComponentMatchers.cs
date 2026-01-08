using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial class DataComponentMatchers
{
    public ExactComponentMatcher ExactMatchers { get; }
    public int[] PartialMatchers { get; }

    public DataComponentMatchers(ExactComponentMatcher exactMatchers, int[] partialMatchers)
    {
        ExactMatchers = exactMatchers;
        PartialMatchers = partialMatchers;
    }
}
