using Dunet;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class RecipeDisplay
{
    public string Type { get; }
    public RecipeDisplayData Data { get; }

    public RecipeDisplay(string type, RecipeDisplayData data)
    {
        Type = type;
        Data = data;
    }
}

[Union]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public partial record RecipeDisplayData
{
    public sealed record CraftingShapeless(SlotDisplay[] Ingredients, SlotDisplay Result,
        SlotDisplay CraftingStation) : RecipeDisplayData;

    public sealed record CraftingShaped(int Width, int Height, SlotDisplay[] Ingredients, SlotDisplay Result,
        SlotDisplay CraftingStation) : RecipeDisplayData;

    public sealed record Furnace(SlotDisplay Ingredient, SlotDisplay Fuel, SlotDisplay Result,
        SlotDisplay CraftingStation, int Duration, float Experience) : RecipeDisplayData;

    public sealed record Stonecutter(SlotDisplay Ingredient, SlotDisplay Result,
        SlotDisplay CraftingStation) : RecipeDisplayData;

    public sealed record Smithing(SlotDisplay Template, SlotDisplay Base, SlotDisplay Addition,
        SlotDisplay Result, SlotDisplay CraftingStation) : RecipeDisplayData;
}
