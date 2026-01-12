using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ArmorTrimMaterial
{
    public string? AssetName { get; }
    public string? AssetBase { get; }
    public int? IngredientId { get; }
    public IReadOnlyList<ArmorTrimMaterialOverride> OverrideArmorAssets { get; }
    public NbtTag Description { get; }

    public ArmorTrimMaterial(string? assetName, string? assetBase, int? ingredientId,
        IReadOnlyList<ArmorTrimMaterialOverride> overrideArmorAssets, NbtTag description)
    {
        AssetName = assetName;
        AssetBase = assetBase;
        IngredientId = ingredientId;
        OverrideArmorAssets = overrideArmorAssets;
        Description = description;
    }

    public sealed record ArmorTrimMaterialOverride(string Key, string Value);
}
