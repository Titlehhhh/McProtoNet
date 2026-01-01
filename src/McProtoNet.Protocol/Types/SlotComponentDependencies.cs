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

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ArmorTrimPattern
{
    public string AssetId { get; }
    public int? TemplateItemId { get; }
    public NbtTag Description { get; }
    public bool Decal { get; }

    public ArmorTrimPattern(string assetId, int? templateItemId, NbtTag description, bool decal)
    {
        AssetId = assetId;
        TemplateItemId = templateItemId;
        Description = description;
        Decal = decal;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class BannerPatternLayer
{
    public RegistryEntryHolder<BannerPattern>? Pattern { get; }
    public ItemSoundHolder? PatternSound { get; }
    public int ColorId { get; }

    public BannerPatternLayer(RegistryEntryHolder<BannerPattern>? pattern, ItemSoundHolder? patternSound, int colorId)
    {
        Pattern = pattern;
        PatternSound = patternSound;
        ColorId = colorId;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class EntityMetadataPaintingVariant
{
    public int Width { get; }
    public int Height { get; }
    public string AssetId { get; }
    public NbtTag? Title { get; }
    public NbtTag? Author { get; }

    public EntityMetadataPaintingVariant(int width, int height, string assetId, NbtTag? title, NbtTag? author)
    {
        Width = width;
        Height = height;
        AssetId = assetId;
        Title = title;
        Author = author;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class IDSet
{
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class InstrumentData
{
    public ItemSoundHolder SoundEvent { get; }
    public float UseDuration { get; }
    public float Range { get; }
    public NbtTag Description { get; }

    public InstrumentData(ItemSoundHolder soundEvent, float useDuration, float range, NbtTag description)
    {
        SoundEvent = soundEvent;
        UseDuration = useDuration;
        Range = range;
        Description = description;
    }
}

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

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemBookPage
{
    public string Content { get; }
    public string? FilteredContent { get; }

    public ItemBookPage(string content, string? filteredContent)
    {
        Content = content;
        FilteredContent = filteredContent;
    }
}

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemConsumeEffect
{
    public string Type { get; }
    public ItemPotionEffect[]? Effects { get; }
    public float? Probability { get; }
    public IDSet? RemovedEffects { get; }
    public float? Diameter { get; }
    public ItemSoundHolder? Sound { get; }

    public ItemConsumeEffect(string type, ItemPotionEffect[]? effects, float? probability, IDSet? removedEffects,
        float? diameter, ItemSoundHolder? sound)
    {
        Type = type;
        Effects = effects;
        Probability = probability;
        RemovedEffects = removedEffects;
        Diameter = diameter;
        Sound = sound;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemFireworkExplosion
{
    public string Shape { get; }
    public int[] Colors { get; }
    public int[] FadeColors { get; }
    public bool HasTrail { get; }
    public bool HasTwinkle { get; }

    public ItemFireworkExplosion(string shape, int[] colors, int[] fadeColors, bool hasTrail, bool hasTwinkle)
    {
        Shape = shape;
        Colors = colors;
        FadeColors = fadeColors;
        HasTrail = hasTrail;
        HasTwinkle = hasTwinkle;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemPotionEffect
{
    public int Id { get; }
    public ItemEffectDetail Details { get; }

    public ItemPotionEffect(int id, ItemEffectDetail details)
    {
        Id = id;
        Details = details;
    }
}

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemSoundHolder
{
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemWrittenBookPage
{
    public NbtTag Content { get; }
    public NbtTag? FilteredContent { get; }

    public ItemWrittenBookPage(NbtTag content, NbtTag? filteredContent)
    {
        Content = content;
        FilteredContent = filteredContent;
    }
}

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class JukeboxSongData
{
    public ItemSoundHolder SoundEvent { get; }
    public NbtTag Description { get; }
    public float LengthInSeconds { get; }
    public int ComparatorOutput { get; }

    public JukeboxSongData(ItemSoundHolder soundEvent, NbtTag description, float lengthInSeconds, int comparatorOutput)
    {
        SoundEvent = soundEvent;
        Description = description;
        LengthInSeconds = lengthInSeconds;
        ComparatorOutput = comparatorOutput;
    }
}
