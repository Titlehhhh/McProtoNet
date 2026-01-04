using Dunet;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class SlotDisplay
{
    public string Type { get; }
    public SlotDisplayData Data { get; }

    public SlotDisplay(string type, SlotDisplayData data)
    {
        Type = type;
        Data = data;
    }
}

[Union]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public partial record SlotDisplayData
{
    public sealed record Empty() : SlotDisplayData;
    public sealed record AnyFuel() : SlotDisplayData;
    public sealed record Item(int ItemId) : SlotDisplayData;
    public sealed record ItemStack(Slot ItemStack) : SlotDisplayData;
    public sealed record Tag(string Tag) : SlotDisplayData;

    public sealed record SmithingTrim(SlotDisplay Base, SlotDisplay Material, SlotDisplay? PatternDisplay,
        RegistryEntryHolder<ArmorTrimPattern>? Pattern) : SlotDisplayData;

    public sealed record WithRemainder(SlotDisplay Input, SlotDisplay Remainder) : SlotDisplayData;
    public sealed record Composite(SlotDisplay[] Entries) : SlotDisplayData;
}
