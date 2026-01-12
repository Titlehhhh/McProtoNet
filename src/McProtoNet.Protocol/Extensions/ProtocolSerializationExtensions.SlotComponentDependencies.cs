using System;
using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static ArmorTrimMaterial ReadArmorTrimMaterial(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArmorTrimMaterial>(protocolVersion);
        if (protocolVersion <= 769)
        {
            string assetName = reader.ReadString();
            int ingredientId = reader.ReadVarInt();
            ArmorTrimMaterial.ArmorTrimMaterialOverride[] overrides = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
                new ArmorTrimMaterial.ArmorTrimMaterialOverride(r.ReadString(), r.ReadString()));
            NbtTag description = reader.ReadAnonymousNbtTag(protocolVersion)
                ?? throw new InvalidOperationException("ArmorTrimMaterial.description missing.");
            return new ArmorTrimMaterial(assetName, null, ingredientId, overrides, description);
        }

        string assetBase = reader.ReadString();
        ArmorTrimMaterial.ArmorTrimMaterialOverride[] overrideAssets = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
            new ArmorTrimMaterial.ArmorTrimMaterialOverride(r.ReadString(), r.ReadString()));
        NbtTag updatedDescription = reader.ReadAnonymousNbtTag(protocolVersion)
            ?? throw new InvalidOperationException("ArmorTrimMaterial.description missing.");
        return new ArmorTrimMaterial(null, assetBase, null, overrideAssets, updatedDescription);
    }

    public static void WriteArmorTrimMaterial(this ref MinecraftPrimitiveWriter writer, ArmorTrimMaterial value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArmorTrimMaterial>(protocolVersion);
        if (protocolVersion <= 769)
        {
            writer.WriteString(value.AssetName ?? value.AssetBase ?? string.Empty);
            writer.WriteVarInt(value.IngredientId ?? 0);
            WriteArray(ref writer, value.OverrideArmorAssets,
                (ref MinecraftPrimitiveWriter w, ArmorTrimMaterial.ArmorTrimMaterialOverride entry) =>
                {
                    w.WriteString(entry.Key);
                    w.WriteString(entry.Value);
                });
            writer.WriteAnonymousNbtTag(value.Description, protocolVersion);
            return;
        }

        writer.WriteString(value.AssetBase ?? value.AssetName ?? string.Empty);
        WriteArray(ref writer, value.OverrideArmorAssets,
            (ref MinecraftPrimitiveWriter w, ArmorTrimMaterial.ArmorTrimMaterialOverride entry) =>
            {
                w.WriteString(entry.Key);
                w.WriteString(entry.Value);
            });
        writer.WriteAnonymousNbtTag(value.Description, protocolVersion);
    }

    public static ArmorTrimPattern ReadArmorTrimPattern(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArmorTrimPattern>(protocolVersion);
        string assetId = reader.ReadString();
        int? templateItemId = null;
        if (protocolVersion <= 769)
        {
            templateItemId = reader.ReadVarInt();
        }
        NbtTag description = reader.ReadAnonymousNbtTag(protocolVersion)
            ?? throw new InvalidOperationException("ArmorTrimPattern.description missing.");
        bool decal = reader.ReadBoolean();
        return new ArmorTrimPattern(assetId, templateItemId, description, decal);
    }

    public static void WriteArmorTrimPattern(this ref MinecraftPrimitiveWriter writer, ArmorTrimPattern value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArmorTrimPattern>(protocolVersion);
        writer.WriteString(value.AssetId);
        if (protocolVersion <= 769)
        {
            writer.WriteVarInt(value.TemplateItemId ?? 0);
        }
        writer.WriteAnonymousNbtTag(value.Description, protocolVersion);
        writer.WriteBoolean(value.Decal);
    }

    public static BannerPatternLayer ReadBannerPatternLayer(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BannerPatternLayer>(protocolVersion);
        if (protocolVersion == 768)
        {
            ItemSoundHolder patternSound = reader.ReadItemSoundHolder(protocolVersion);
            int colorId = reader.ReadVarInt();
            return new BannerPatternLayer(null, patternSound, colorId);
        }

        RegistryEntryHolder<BannerPattern> pattern = reader.ReadRegistryEntryHolder<BannerPattern>(protocolVersion);
        int color = reader.ReadVarInt();
        return new BannerPatternLayer(pattern, null, color);
    }

    public static void WriteBannerPatternLayer(this ref MinecraftPrimitiveWriter writer, BannerPatternLayer value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BannerPatternLayer>(protocolVersion);
        if (protocolVersion == 768)
        {
            writer.WriteItemSoundHolder(value.PatternSound ?? throw new InvalidOperationException("patternSound missing"), protocolVersion);
            writer.WriteVarInt(value.ColorId);
            return;
        }

        writer.WriteRegistryEntryHolder(value.Pattern ?? throw new InvalidOperationException("pattern missing"), protocolVersion);
        writer.WriteVarInt(value.ColorId);
    }

    public static EntityMetadataPaintingVariant ReadEntityMetadataPaintingVariant(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMetadataPaintingVariant>(protocolVersion);
        int width = reader.ReadSignedInt();
        int height = reader.ReadSignedInt();
        string assetId = reader.ReadString();
        NbtTag? title = reader.ReadBoolean() ? reader.ReadAnonymousNbtTag(protocolVersion) : null;
        NbtTag? author = reader.ReadBoolean() ? reader.ReadAnonymousNbtTag(protocolVersion) : null;
        return new EntityMetadataPaintingVariant(width, height, assetId, title, author);
    }

    public static void WriteEntityMetadataPaintingVariant(this ref MinecraftPrimitiveWriter writer,
        EntityMetadataPaintingVariant value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMetadataPaintingVariant>(protocolVersion);
        writer.WriteSignedInt(value.Width);
        writer.WriteSignedInt(value.Height);
        writer.WriteString(value.AssetId);
        if (value.Title is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteAnonymousNbtTag(value.Title, protocolVersion);
        }
        if (value.Author is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteAnonymousNbtTag(value.Author, protocolVersion);
        }
    }

    public static IDSet ReadIDSet(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<IDSet>(protocolVersion);
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return new IDSet(Array.Empty<RegistryEntryHolder<int>>());
        }

        var entries = new RegistryEntryHolder<int>[count];
        for (int i = 0; i < count; i++)
        {
            entries[i] = reader.ReadRegistryEntryHolder<int>(protocolVersion);
        }
        return new IDSet(entries);
    }

    public static void WriteIDSet(this ref MinecraftPrimitiveWriter writer, IDSet value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<IDSet>(protocolVersion);
        writer.WriteVarInt(value.Entries.Length);
        for (int i = 0; i < value.Entries.Length; i++)
        {
            writer.WriteRegistryEntryHolder(value.Entries[i], protocolVersion);
        }
    }

    public static InstrumentData ReadInstrumentData(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<InstrumentData>(protocolVersion);
        ItemSoundHolder soundEvent = reader.ReadItemSoundHolder(protocolVersion);
        float useDuration = reader.ReadFloat();
        float range = reader.ReadFloat();
        NbtTag description = reader.ReadAnonymousNbtTag(protocolVersion)
            ?? throw new InvalidOperationException("InstrumentData.description missing.");
        return new InstrumentData(soundEvent, useDuration, range, description);
    }

    public static void WriteInstrumentData(this ref MinecraftPrimitiveWriter writer, InstrumentData value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<InstrumentData>(protocolVersion);
        writer.WriteItemSoundHolder(value.SoundEvent, protocolVersion);
        writer.WriteFloat(value.UseDuration);
        writer.WriteFloat(value.Range);
        writer.WriteAnonymousNbtTag(value.Description, protocolVersion);
    }

    public static ItemBlockPredicate ReadItemBlockPredicate(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemBlockPredicate>(protocolVersion);
        IDSet? blockSet = null;
        if (reader.ReadBoolean())
        {
            blockSet = reader.ReadIDSet(protocolVersion);
        }
        IReadOnlyList<ItemBlockProperty>? properties = null;
        if (reader.ReadBoolean())
        {
            properties = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemBlockProperty(protocolVersion));
        }
        NbtTag? nbt = reader.ReadAnonOptionalNbtTag(protocolVersion);
        DataComponentMatchers? components = null;
        if (protocolVersion >= 770)
        {
            components = reader.ReadDataComponentMatchers(protocolVersion);
        }
        return new ItemBlockPredicate(blockSet, properties, nbt, components);
    }

    public static void WriteItemBlockPredicate(this ref MinecraftPrimitiveWriter writer, ItemBlockPredicate value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemBlockPredicate>(protocolVersion);
        if (value.BlockSet is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteIDSet(value.BlockSet, protocolVersion);
        }
        if (value.Properties is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            WriteArray(ref writer, value.Properties,
                (ref MinecraftPrimitiveWriter w, ItemBlockProperty property) => w.WriteItemBlockProperty(property, protocolVersion));
        }
        writer.WriteAnonOptionalNbtTag(value.Nbt, protocolVersion);
        if (protocolVersion >= 770)
        {
            writer.WriteDataComponentMatchers(value.Components ?? throw new InvalidOperationException("components missing"), protocolVersion);
        }
    }

    public static ItemBookPage ReadItemBookPage(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemBookPage>(protocolVersion);
        string content = reader.ReadString();
        string? filteredContent = reader.ReadBoolean() ? reader.ReadString() : null;
        return new ItemBookPage(content, filteredContent);
    }

    public static void WriteItemBookPage(this ref MinecraftPrimitiveWriter writer, ItemBookPage value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemBookPage>(protocolVersion);
        writer.WriteString(value.Content);
        if (value.FilteredContent is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteString(value.FilteredContent);
        }
    }

    public static ItemConsumeEffect ReadItemConsumeEffect(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemConsumeEffect>(protocolVersion);
        string type = ReadItemConsumeEffectType(reader.ReadVarInt());
        ItemPotionEffect[]? effects = null;
        float? probability = null;
        IDSet? removedEffects = null;
        float? diameter = null;
        ItemSoundHolder? sound = null;

        switch (type)
        {
            case "apply_effects":
                effects = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemPotionEffect(protocolVersion));
                probability = reader.ReadFloat();
                break;
            case "remove_effects":
                removedEffects = reader.ReadIDSet(protocolVersion);
                break;
            case "clear_all_effects":
                break;
            case "teleport_randomly":
                diameter = reader.ReadFloat();
                break;
            case "play_sound":
                sound = reader.ReadItemSoundHolder(protocolVersion);
                break;
            default:
                throw new InvalidOperationException($"Unknown ItemConsumeEffect type {type}");
        }

        return new ItemConsumeEffect(type, effects, probability, removedEffects, diameter, sound);
    }

    public static void WriteItemConsumeEffect(this ref MinecraftPrimitiveWriter writer, ItemConsumeEffect value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemConsumeEffect>(protocolVersion);
        writer.WriteVarInt(WriteItemConsumeEffectType(value.Type));
        switch (value.Type)
        {
            case "apply_effects":
                WriteArray(ref writer, value.Effects ?? Array.Empty<ItemPotionEffect>(),
                    (ref MinecraftPrimitiveWriter w, ItemPotionEffect effect) => w.WriteItemPotionEffect(effect, protocolVersion));
                writer.WriteFloat(value.Probability ?? 0f);
                break;
            case "remove_effects":
                writer.WriteIDSet(value.RemovedEffects ?? throw new InvalidOperationException("removedEffects missing"), protocolVersion);
                break;
            case "clear_all_effects":
                break;
            case "teleport_randomly":
                writer.WriteFloat(value.Diameter ?? 0f);
                break;
            case "play_sound":
                writer.WriteItemSoundHolder(value.Sound ?? throw new InvalidOperationException("sound missing"), protocolVersion);
                break;
            default:
                throw new InvalidOperationException($"Unknown ItemConsumeEffect type {value.Type}");
        }
    }

    public static ItemFireworkExplosion ReadItemFireworkExplosion(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemFireworkExplosion>(protocolVersion);
        string shape = ReadItemFireworkExplosionShape(reader.ReadVarInt());
        int[] colors = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSignedInt());
        int[] fadeColors = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSignedInt());
        bool hasTrail = reader.ReadBoolean();
        bool hasTwinkle = reader.ReadBoolean();
        return new ItemFireworkExplosion(shape, colors, fadeColors, hasTrail, hasTwinkle);
    }

    public static void WriteItemFireworkExplosion(this ref MinecraftPrimitiveWriter writer, ItemFireworkExplosion value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemFireworkExplosion>(protocolVersion);
        writer.WriteVarInt(WriteItemFireworkExplosionShape(value.Shape));
        WriteArray(ref writer, value.Colors, (ref MinecraftPrimitiveWriter w, int color) => w.WriteSignedInt(color));
        WriteArray(ref writer, value.FadeColors, (ref MinecraftPrimitiveWriter w, int color) => w.WriteSignedInt(color));
        writer.WriteBoolean(value.HasTrail);
        writer.WriteBoolean(value.HasTwinkle);
    }

    public static ItemPotionEffect ReadItemPotionEffect(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemPotionEffect>(protocolVersion);
        int id = reader.ReadVarInt();
        ItemEffectDetail details = reader.ReadItemEffectDetail(protocolVersion);
        return new ItemPotionEffect(id, details);
    }

    public static void WriteItemPotionEffect(this ref MinecraftPrimitiveWriter writer, ItemPotionEffect value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemPotionEffect>(protocolVersion);
        writer.WriteVarInt(value.Id);
        writer.WriteItemEffectDetail(value.Details, protocolVersion);
    }

    public static ItemSoundHolder ReadItemSoundHolder(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemSoundHolder>(protocolVersion);
        bool hasInline = reader.ReadBoolean();
        if (!hasInline)
        {
            int registryId = reader.ReadVarInt();
            return new ItemSoundHolder(registryId);
        }

        ItemSoundEvent inline = reader.ReadItemSoundEvent(protocolVersion);
        return new ItemSoundHolder(inline);
    }

    public static void WriteItemSoundHolder(this ref MinecraftPrimitiveWriter writer, ItemSoundHolder value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemSoundHolder>(protocolVersion);
        writer.WriteBoolean(value.HasInline);
        if (!value.HasInline)
        {
            writer.WriteVarInt(value.RegistryId ?? 0);
            return;
        }

        writer.WriteItemSoundEvent(value.Inline ?? throw new InvalidOperationException("ItemSoundHolder inline missing."),
            protocolVersion);
    }

    public static ItemWrittenBookPage ReadItemWrittenBookPage(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemWrittenBookPage>(protocolVersion);
        NbtTag content = reader.ReadAnonymousNbtTag(protocolVersion)
            ?? throw new InvalidOperationException("ItemWrittenBookPage.content missing.");
        NbtTag? filteredContent = reader.ReadAnonOptionalNbtTag(protocolVersion);
        return new ItemWrittenBookPage(content, filteredContent);
    }

    public static void WriteItemWrittenBookPage(this ref MinecraftPrimitiveWriter writer, ItemWrittenBookPage value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemWrittenBookPage>(protocolVersion);
        writer.WriteAnonymousNbtTag(value.Content, protocolVersion);
        writer.WriteAnonOptionalNbtTag(value.FilteredContent, protocolVersion);
    }

    public static JukeboxSongData ReadJukeboxSongData(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<JukeboxSongData>(protocolVersion);
        ItemSoundHolder soundEvent = reader.ReadItemSoundHolder(protocolVersion);
        NbtTag description = reader.ReadAnonymousNbtTag(protocolVersion)
            ?? throw new InvalidOperationException("JukeboxSongData.description missing.");
        float lengthInSeconds = reader.ReadFloat();
        int comparatorOutput = reader.ReadVarInt();
        return new JukeboxSongData(soundEvent, description, lengthInSeconds, comparatorOutput);
    }

    public static void WriteJukeboxSongData(this ref MinecraftPrimitiveWriter writer, JukeboxSongData value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<JukeboxSongData>(protocolVersion);
        writer.WriteItemSoundHolder(value.SoundEvent, protocolVersion);
        writer.WriteAnonymousNbtTag(value.Description, protocolVersion);
        writer.WriteFloat(value.LengthInSeconds);
        writer.WriteVarInt(value.ComparatorOutput);
    }

    private static string ReadItemConsumeEffectType(int id)
    {
        return id switch
        {
            0 => "apply_effects",
            1 => "remove_effects",
            2 => "clear_all_effects",
            3 => "teleport_randomly",
            4 => "play_sound",
            _ => throw new InvalidOperationException($"Unknown ItemConsumeEffect type id {id}")
        };
    }

    private static int WriteItemConsumeEffectType(string value)
    {
        return value switch
        {
            "apply_effects" => 0,
            "remove_effects" => 1,
            "clear_all_effects" => 2,
            "teleport_randomly" => 3,
            "play_sound" => 4,
            _ => throw new InvalidOperationException($"Unknown ItemConsumeEffect type {value}")
        };
    }

    private static string ReadItemFireworkExplosionShape(int id)
    {
        return id switch
        {
            0 => "small_ball",
            1 => "large_ball",
            2 => "star",
            3 => "creeper",
            4 => "burst",
            _ => throw new InvalidOperationException($"Unknown ItemFireworkExplosion shape id {id}")
        };
    }

    private static int WriteItemFireworkExplosionShape(string value)
    {
        return value switch
        {
            "small_ball" => 0,
            "large_ball" => 1,
            "star" => 2,
            "creeper" => 3,
            "burst" => 4,
            _ => throw new InvalidOperationException($"Unknown ItemFireworkExplosion shape {value}")
        };
    }
}
