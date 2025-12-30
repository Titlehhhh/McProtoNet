using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Slot
{
    public int? ItemId { get; set; }
    public int? ItemCount { get; set; }
    public NbtTag? Nbt { get; set; }
    public IReadOnlyList<SlotComponent>? Components { get; set; }
    public IReadOnlyList<SlotComponentType>? RemovedComponents { get; set; }

    public static Slot Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadSlot(protocolVersion);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSlot(this, protocolVersion);
    }
}
