using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Tags
{
    public Tag[] Values { get; }

    public Tags(Tag[] values)
    {
        Values = values;
    }
}
