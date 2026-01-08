using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial class ExactComponentMatcher
{
    public SlotComponent[] Components { get; }

    public ExactComponentMatcher(SlotComponent[] components)
    {
        Components = components;
    }
}
