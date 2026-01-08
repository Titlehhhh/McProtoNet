using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class IDSet
{
    public RegistryEntryHolder<int>[] Entries { get; }

    public IDSet(RegistryEntryHolder<int>[] entries)
    {
        Entries = entries;
    }
}
