using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemBlockPredicate
{
    public IDSet? BlockSet { get; }
    public IReadOnlyList<ItemBlockProperty>? Properties { get; }
    public NbtTag? Nbt { get; }
    public DataComponentMatchers? Components { get; }

    public ItemBlockPredicate(IDSet? blockSet, IReadOnlyList<ItemBlockProperty>? properties, NbtTag? nbt,
        DataComponentMatchers? components)
    {
        BlockSet = blockSet;
        Properties = properties;
        Nbt = nbt;
        Components = components;
    }
}
