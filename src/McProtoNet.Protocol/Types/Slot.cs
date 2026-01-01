using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Slot
{
    public int? ItemId { get; set; }
    public int? ItemCount { get; set; }
    public NbtTag? Nbt { get; set; }
    public IReadOnlyList<SlotComponent>? Components { get; set; }
    public IReadOnlyList<SlotComponentType>? RemovedComponents { get; set; }
}
