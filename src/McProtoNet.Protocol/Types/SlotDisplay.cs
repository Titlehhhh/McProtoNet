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
    public sealed partial record Empty() : SlotDisplayData;
    public sealed partial record AnyFuel() : SlotDisplayData;
    public sealed partial record Item(int ItemId) : SlotDisplayData;
    public sealed partial record ItemStack(Slot Stack) : SlotDisplayData;
    public sealed partial record Tag(string TagId) : SlotDisplayData;

    public sealed partial record SmithingTrim(SlotDisplay Base, SlotDisplay Material, SlotDisplay? PatternDisplay,
        RegistryEntryHolder<ArmorTrimPattern>? Pattern) : SlotDisplayData;

    public sealed partial record WithRemainder(SlotDisplay Input, SlotDisplay Remainder) : SlotDisplayData;
    public sealed partial record Composite(SlotDisplay[] Entries) : SlotDisplayData;
}
