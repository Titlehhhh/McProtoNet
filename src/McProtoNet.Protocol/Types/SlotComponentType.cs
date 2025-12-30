using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public enum SlotComponentType
{
    AttributeModifiers,
    AxolotlVariant,
    BannerPatterns,
    BaseColor,
    Bees,
    BlockEntityData,
    BlockState,
    BlocksAttacks,
    BreakSound,
    BucketEntityData,
    BundleContents,
    CanBreak,
    CanPlaceOn,
    CatCollar,
    CatVariant,
    ChargedProjectiles,
    ChickenVariant,
    Consumable,
    Container,
    ContainerLoot,
    CowVariant,
    CreativeSlotLock,
    CustomData,
    CustomModelData,
    CustomName,
    Damage,
    DamageResistant,
    DeathProtection,
    DebugStickState,
    DyedColor,
    Enchantable,
    EnchantmentGlintOverride,
    Enchantments,
    EntityData,
    Equippable,
    FireResistant,
    FireworkExplosion,
    Fireworks,
    Food,
    FoxVariant,
    FrogVariant,
    Glider,
    HideAdditionalTooltip,
    HideTooltip,
    HorseVariant,
    Instrument,
    IntangibleProjectile,
    ItemModel,
    ItemName,
    JukeboxPlayable,
    LlamaVariant,
    Lock,
    LodestoneTracker,
    Lore,
    MapColor,
    MapDecorations,
    MapId,
    MapPostProcessing,
    MaxDamage,
    MaxStackSize,
    MooshroomVariant,
    NoteBlockSound,
    OminousBottleAmplifier,
    PaintingVariant,
    ParrotVariant,
    PigVariant,
    PotDecorations,
    PotionContents,
    PotionDurationScale,
    Profile,
    ProvidesBannerPatterns,
    ProvidesTrimMaterial,
    RabbitVariant,
    Rarity,
    Recipes,
    RepairCost,
    Repairable,
    SalmonSize,
    SheepColor,
    ShulkerColor,
    StoredEnchantments,
    SuspiciousStewEffects,
    Tool,
    TooltipDisplay,
    TooltipStyle,
    Trim,
    TropicalFishBaseColor,
    TropicalFishPattern,
    TropicalFishPatternColor,
    Unbreakable,
    UseCooldown,
    UseRemainder,
    VillagerVariant,
    Weapon,
    WolfCollar,
    WolfSoundVariant,
    WolfVariant,
    WritableBookContent,
    WrittenBookContent
}

public static class SlotComponentTypeExtensions
{
    public static SlotComponentType Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotComponentType>(protocolVersion);
        int id = reader.ReadVarInt();
        return protocolVersion switch
        {
            766 => FromId766(id, protocolVersion),
            767 => FromId767(id, protocolVersion),
            >= 768 and <= 769 => FromId768To769(id, protocolVersion),
            >= 770 and <= 772 => FromId770To772(id, protocolVersion),
            _ => throw new InvalidOperationException($"Unknown SlotComponentType id {id} for protocol {protocolVersion}.")
        };
    }

    public static void Write(this SlotComponentType value, ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotComponentType>(protocolVersion);
        int id = protocolVersion switch
        {
            766 => ToId766(value, protocolVersion),
            767 => ToId767(value, protocolVersion),
            >= 768 and <= 769 => ToId768To769(value, protocolVersion),
            >= 770 and <= 772 => ToId770To772(value, protocolVersion),
            _ => throw new InvalidOperationException($"Unknown SlotComponentType {value} for protocol {protocolVersion}.")
        };
        writer.WriteVarInt(id);
    }

    private static SlotComponentType FromId766(int id, int protocolVersion)
    {
        switch (id)
        {
            case 0: return SlotComponentType.CustomData;
            case 1: return SlotComponentType.MaxStackSize;
            case 2: return SlotComponentType.MaxDamage;
            case 3: return SlotComponentType.Damage;
            case 4: return SlotComponentType.Unbreakable;
            case 5: return SlotComponentType.CustomName;
            case 6: return SlotComponentType.ItemName;
            case 7: return SlotComponentType.Lore;
            case 8: return SlotComponentType.Rarity;
            case 9: return SlotComponentType.Enchantments;
            case 10: return SlotComponentType.CanPlaceOn;
            case 11: return SlotComponentType.CanBreak;
            case 12: return SlotComponentType.AttributeModifiers;
            case 13: return SlotComponentType.CustomModelData;
            case 14: return SlotComponentType.HideAdditionalTooltip;
            case 15: return SlotComponentType.HideTooltip;
            case 16: return SlotComponentType.RepairCost;
            case 17: return SlotComponentType.CreativeSlotLock;
            case 18: return SlotComponentType.EnchantmentGlintOverride;
            case 19: return SlotComponentType.IntangibleProjectile;
            case 20: return SlotComponentType.Food;
            case 21: return SlotComponentType.FireResistant;
            case 22: return SlotComponentType.Tool;
            case 23: return SlotComponentType.StoredEnchantments;
            case 24: return SlotComponentType.DyedColor;
            case 25: return SlotComponentType.MapColor;
            case 26: return SlotComponentType.MapId;
            case 27: return SlotComponentType.MapDecorations;
            case 28: return SlotComponentType.MapPostProcessing;
            case 29: return SlotComponentType.ChargedProjectiles;
            case 30: return SlotComponentType.BundleContents;
            case 31: return SlotComponentType.PotionContents;
            case 32: return SlotComponentType.SuspiciousStewEffects;
            case 33: return SlotComponentType.WritableBookContent;
            case 34: return SlotComponentType.WrittenBookContent;
            case 35: return SlotComponentType.Trim;
            case 36: return SlotComponentType.DebugStickState;
            case 37: return SlotComponentType.EntityData;
            case 38: return SlotComponentType.BucketEntityData;
            case 39: return SlotComponentType.BlockEntityData;
            case 40: return SlotComponentType.Instrument;
            case 41: return SlotComponentType.OminousBottleAmplifier;
            case 42: return SlotComponentType.Recipes;
            case 43: return SlotComponentType.LodestoneTracker;
            case 44: return SlotComponentType.FireworkExplosion;
            case 45: return SlotComponentType.Fireworks;
            case 46: return SlotComponentType.Profile;
            case 47: return SlotComponentType.NoteBlockSound;
            case 48: return SlotComponentType.BannerPatterns;
            case 49: return SlotComponentType.BaseColor;
            case 50: return SlotComponentType.PotDecorations;
            case 51: return SlotComponentType.Container;
            case 52: return SlotComponentType.BlockState;
            case 53: return SlotComponentType.Bees;
            case 54: return SlotComponentType.Lock;
            case 55: return SlotComponentType.ContainerLoot;
            default: throw new InvalidOperationException($"Unknown SlotComponentType id {id} for protocol {protocolVersion}.");
        }
    }

    private static SlotComponentType FromId767(int id, int protocolVersion)
    {
        switch (id)
        {
            case 0: return SlotComponentType.CustomData;
            case 1: return SlotComponentType.MaxStackSize;
            case 2: return SlotComponentType.MaxDamage;
            case 3: return SlotComponentType.Damage;
            case 4: return SlotComponentType.Unbreakable;
            case 5: return SlotComponentType.CustomName;
            case 6: return SlotComponentType.ItemName;
            case 7: return SlotComponentType.Lore;
            case 8: return SlotComponentType.Rarity;
            case 9: return SlotComponentType.Enchantments;
            case 10: return SlotComponentType.CanPlaceOn;
            case 11: return SlotComponentType.CanBreak;
            case 12: return SlotComponentType.AttributeModifiers;
            case 13: return SlotComponentType.CustomModelData;
            case 14: return SlotComponentType.HideAdditionalTooltip;
            case 15: return SlotComponentType.HideTooltip;
            case 16: return SlotComponentType.RepairCost;
            case 17: return SlotComponentType.CreativeSlotLock;
            case 18: return SlotComponentType.EnchantmentGlintOverride;
            case 19: return SlotComponentType.IntangibleProjectile;
            case 20: return SlotComponentType.Food;
            case 21: return SlotComponentType.FireResistant;
            case 22: return SlotComponentType.Tool;
            case 23: return SlotComponentType.StoredEnchantments;
            case 24: return SlotComponentType.DyedColor;
            case 25: return SlotComponentType.MapColor;
            case 26: return SlotComponentType.MapId;
            case 27: return SlotComponentType.MapDecorations;
            case 28: return SlotComponentType.MapPostProcessing;
            case 29: return SlotComponentType.ChargedProjectiles;
            case 30: return SlotComponentType.BundleContents;
            case 31: return SlotComponentType.PotionContents;
            case 32: return SlotComponentType.SuspiciousStewEffects;
            case 33: return SlotComponentType.WritableBookContent;
            case 34: return SlotComponentType.WrittenBookContent;
            case 35: return SlotComponentType.Trim;
            case 36: return SlotComponentType.DebugStickState;
            case 37: return SlotComponentType.EntityData;
            case 38: return SlotComponentType.BucketEntityData;
            case 39: return SlotComponentType.BlockEntityData;
            case 40: return SlotComponentType.Instrument;
            case 41: return SlotComponentType.OminousBottleAmplifier;
            case 42: return SlotComponentType.JukeboxPlayable;
            case 43: return SlotComponentType.Recipes;
            case 44: return SlotComponentType.LodestoneTracker;
            case 45: return SlotComponentType.FireworkExplosion;
            case 46: return SlotComponentType.Fireworks;
            case 47: return SlotComponentType.Profile;
            case 48: return SlotComponentType.NoteBlockSound;
            case 49: return SlotComponentType.BannerPatterns;
            case 50: return SlotComponentType.BaseColor;
            case 51: return SlotComponentType.PotDecorations;
            case 52: return SlotComponentType.Container;
            case 53: return SlotComponentType.BlockState;
            case 54: return SlotComponentType.Bees;
            case 55: return SlotComponentType.Lock;
            case 56: return SlotComponentType.ContainerLoot;
            default: throw new InvalidOperationException($"Unknown SlotComponentType id {id} for protocol {protocolVersion}.");
        }
    }

    private static SlotComponentType FromId768To769(int id, int protocolVersion)
    {
        switch (id)
        {
            case 0: return SlotComponentType.CustomData;
            case 1: return SlotComponentType.MaxStackSize;
            case 2: return SlotComponentType.MaxDamage;
            case 3: return SlotComponentType.Damage;
            case 4: return SlotComponentType.Unbreakable;
            case 5: return SlotComponentType.CustomName;
            case 6: return SlotComponentType.ItemName;
            case 7: return SlotComponentType.ItemModel;
            case 8: return SlotComponentType.Lore;
            case 9: return SlotComponentType.Rarity;
            case 10: return SlotComponentType.Enchantments;
            case 11: return SlotComponentType.CanPlaceOn;
            case 12: return SlotComponentType.CanBreak;
            case 13: return SlotComponentType.AttributeModifiers;
            case 14: return SlotComponentType.CustomModelData;
            case 15: return SlotComponentType.HideAdditionalTooltip;
            case 16: return SlotComponentType.HideTooltip;
            case 17: return SlotComponentType.RepairCost;
            case 18: return SlotComponentType.CreativeSlotLock;
            case 19: return SlotComponentType.EnchantmentGlintOverride;
            case 20: return SlotComponentType.IntangibleProjectile;
            case 21: return SlotComponentType.Food;
            case 22: return SlotComponentType.Consumable;
            case 23: return SlotComponentType.UseRemainder;
            case 24: return SlotComponentType.UseCooldown;
            case 25: return SlotComponentType.DamageResistant;
            case 26: return SlotComponentType.Tool;
            case 27: return SlotComponentType.Enchantable;
            case 28: return SlotComponentType.Equippable;
            case 29: return SlotComponentType.Repairable;
            case 30: return SlotComponentType.Glider;
            case 31: return SlotComponentType.TooltipStyle;
            case 32: return SlotComponentType.DeathProtection;
            case 33: return SlotComponentType.StoredEnchantments;
            case 34: return SlotComponentType.DyedColor;
            case 35: return SlotComponentType.MapColor;
            case 36: return SlotComponentType.MapId;
            case 37: return SlotComponentType.MapDecorations;
            case 38: return SlotComponentType.MapPostProcessing;
            case 39: return SlotComponentType.ChargedProjectiles;
            case 40: return SlotComponentType.BundleContents;
            case 41: return SlotComponentType.PotionContents;
            case 42: return SlotComponentType.SuspiciousStewEffects;
            case 43: return SlotComponentType.WritableBookContent;
            case 44: return SlotComponentType.WrittenBookContent;
            case 45: return SlotComponentType.Trim;
            case 46: return SlotComponentType.DebugStickState;
            case 47: return SlotComponentType.EntityData;
            case 48: return SlotComponentType.BucketEntityData;
            case 49: return SlotComponentType.BlockEntityData;
            case 50: return SlotComponentType.Instrument;
            case 51: return SlotComponentType.OminousBottleAmplifier;
            case 52: return SlotComponentType.JukeboxPlayable;
            case 53: return SlotComponentType.Recipes;
            case 54: return SlotComponentType.LodestoneTracker;
            case 55: return SlotComponentType.FireworkExplosion;
            case 56: return SlotComponentType.Fireworks;
            case 57: return SlotComponentType.Profile;
            case 58: return SlotComponentType.NoteBlockSound;
            case 59: return SlotComponentType.BannerPatterns;
            case 60: return SlotComponentType.BaseColor;
            case 61: return SlotComponentType.PotDecorations;
            case 62: return SlotComponentType.Container;
            case 63: return SlotComponentType.BlockState;
            case 64: return SlotComponentType.Bees;
            case 65: return SlotComponentType.Lock;
            case 66: return SlotComponentType.ContainerLoot;
            default: throw new InvalidOperationException($"Unknown SlotComponentType id {id} for protocol {protocolVersion}.");
        }
    }

    private static SlotComponentType FromId770To772(int id, int protocolVersion)
    {
        switch (id)
        {
            case 0: return SlotComponentType.CustomData;
            case 1: return SlotComponentType.MaxStackSize;
            case 2: return SlotComponentType.MaxDamage;
            case 3: return SlotComponentType.Damage;
            case 4: return SlotComponentType.Unbreakable;
            case 5: return SlotComponentType.CustomName;
            case 6: return SlotComponentType.ItemName;
            case 7: return SlotComponentType.ItemModel;
            case 8: return SlotComponentType.Lore;
            case 9: return SlotComponentType.Rarity;
            case 10: return SlotComponentType.Enchantments;
            case 11: return SlotComponentType.CanPlaceOn;
            case 12: return SlotComponentType.CanBreak;
            case 13: return SlotComponentType.AttributeModifiers;
            case 14: return SlotComponentType.CustomModelData;
            case 15: return SlotComponentType.TooltipDisplay;
            case 16: return SlotComponentType.RepairCost;
            case 17: return SlotComponentType.CreativeSlotLock;
            case 18: return SlotComponentType.EnchantmentGlintOverride;
            case 19: return SlotComponentType.IntangibleProjectile;
            case 20: return SlotComponentType.Food;
            case 21: return SlotComponentType.Consumable;
            case 22: return SlotComponentType.UseRemainder;
            case 23: return SlotComponentType.UseCooldown;
            case 24: return SlotComponentType.DamageResistant;
            case 25: return SlotComponentType.Tool;
            case 26: return SlotComponentType.Weapon;
            case 27: return SlotComponentType.Enchantable;
            case 28: return SlotComponentType.Equippable;
            case 29: return SlotComponentType.Repairable;
            case 30: return SlotComponentType.Glider;
            case 31: return SlotComponentType.TooltipStyle;
            case 32: return SlotComponentType.DeathProtection;
            case 33: return SlotComponentType.BlocksAttacks;
            case 34: return SlotComponentType.StoredEnchantments;
            case 35: return SlotComponentType.DyedColor;
            case 36: return SlotComponentType.MapColor;
            case 37: return SlotComponentType.MapId;
            case 38: return SlotComponentType.MapDecorations;
            case 39: return SlotComponentType.MapPostProcessing;
            case 40: return SlotComponentType.PotionDurationScale;
            case 41: return SlotComponentType.ChargedProjectiles;
            case 42: return SlotComponentType.BundleContents;
            case 43: return SlotComponentType.PotionContents;
            case 44: return SlotComponentType.SuspiciousStewEffects;
            case 45: return SlotComponentType.WritableBookContent;
            case 46: return SlotComponentType.WrittenBookContent;
            case 47: return SlotComponentType.Trim;
            case 48: return SlotComponentType.DebugStickState;
            case 49: return SlotComponentType.EntityData;
            case 50: return SlotComponentType.BucketEntityData;
            case 51: return SlotComponentType.BlockEntityData;
            case 52: return SlotComponentType.Instrument;
            case 53: return SlotComponentType.ProvidesTrimMaterial;
            case 54: return SlotComponentType.OminousBottleAmplifier;
            case 55: return SlotComponentType.JukeboxPlayable;
            case 56: return SlotComponentType.ProvidesBannerPatterns;
            case 57: return SlotComponentType.Recipes;
            case 58: return SlotComponentType.LodestoneTracker;
            case 59: return SlotComponentType.FireworkExplosion;
            case 60: return SlotComponentType.Fireworks;
            case 61: return SlotComponentType.Profile;
            case 62: return SlotComponentType.NoteBlockSound;
            case 63: return SlotComponentType.BannerPatterns;
            case 64: return SlotComponentType.BaseColor;
            case 65: return SlotComponentType.PotDecorations;
            case 66: return SlotComponentType.Container;
            case 67: return SlotComponentType.BlockState;
            case 68: return SlotComponentType.Bees;
            case 69: return SlotComponentType.Lock;
            case 70: return SlotComponentType.ContainerLoot;
            case 71: return SlotComponentType.BreakSound;
            case 72: return SlotComponentType.VillagerVariant;
            case 73: return SlotComponentType.WolfVariant;
            case 74: return SlotComponentType.WolfSoundVariant;
            case 75: return SlotComponentType.WolfCollar;
            case 76: return SlotComponentType.FoxVariant;
            case 77: return SlotComponentType.SalmonSize;
            case 78: return SlotComponentType.ParrotVariant;
            case 79: return SlotComponentType.TropicalFishPattern;
            case 80: return SlotComponentType.TropicalFishBaseColor;
            case 81: return SlotComponentType.TropicalFishPatternColor;
            case 82: return SlotComponentType.MooshroomVariant;
            case 83: return SlotComponentType.RabbitVariant;
            case 84: return SlotComponentType.PigVariant;
            case 85: return SlotComponentType.CowVariant;
            case 86: return SlotComponentType.ChickenVariant;
            case 87: return SlotComponentType.FrogVariant;
            case 88: return SlotComponentType.HorseVariant;
            case 89: return SlotComponentType.PaintingVariant;
            case 90: return SlotComponentType.LlamaVariant;
            case 91: return SlotComponentType.AxolotlVariant;
            case 92: return SlotComponentType.CatVariant;
            case 93: return SlotComponentType.CatCollar;
            case 94: return SlotComponentType.SheepColor;
            case 95: return SlotComponentType.ShulkerColor;
            default: throw new InvalidOperationException($"Unknown SlotComponentType id {id} for protocol {protocolVersion}.");
        }
    }

    private static int ToId766(SlotComponentType value, int protocolVersion)
    {
        return value switch
        {
            SlotComponentType.CustomData => 0,
            SlotComponentType.MaxStackSize => 1,
            SlotComponentType.MaxDamage => 2,
            SlotComponentType.Damage => 3,
            SlotComponentType.Unbreakable => 4,
            SlotComponentType.CustomName => 5,
            SlotComponentType.ItemName => 6,
            SlotComponentType.Lore => 7,
            SlotComponentType.Rarity => 8,
            SlotComponentType.Enchantments => 9,
            SlotComponentType.CanPlaceOn => 10,
            SlotComponentType.CanBreak => 11,
            SlotComponentType.AttributeModifiers => 12,
            SlotComponentType.CustomModelData => 13,
            SlotComponentType.HideAdditionalTooltip => 14,
            SlotComponentType.HideTooltip => 15,
            SlotComponentType.RepairCost => 16,
            SlotComponentType.CreativeSlotLock => 17,
            SlotComponentType.EnchantmentGlintOverride => 18,
            SlotComponentType.IntangibleProjectile => 19,
            SlotComponentType.Food => 20,
            SlotComponentType.FireResistant => 21,
            SlotComponentType.Tool => 22,
            SlotComponentType.StoredEnchantments => 23,
            SlotComponentType.DyedColor => 24,
            SlotComponentType.MapColor => 25,
            SlotComponentType.MapId => 26,
            SlotComponentType.MapDecorations => 27,
            SlotComponentType.MapPostProcessing => 28,
            SlotComponentType.ChargedProjectiles => 29,
            SlotComponentType.BundleContents => 30,
            SlotComponentType.PotionContents => 31,
            SlotComponentType.SuspiciousStewEffects => 32,
            SlotComponentType.WritableBookContent => 33,
            SlotComponentType.WrittenBookContent => 34,
            SlotComponentType.Trim => 35,
            SlotComponentType.DebugStickState => 36,
            SlotComponentType.EntityData => 37,
            SlotComponentType.BucketEntityData => 38,
            SlotComponentType.BlockEntityData => 39,
            SlotComponentType.Instrument => 40,
            SlotComponentType.OminousBottleAmplifier => 41,
            SlotComponentType.Recipes => 42,
            SlotComponentType.LodestoneTracker => 43,
            SlotComponentType.FireworkExplosion => 44,
            SlotComponentType.Fireworks => 45,
            SlotComponentType.Profile => 46,
            SlotComponentType.NoteBlockSound => 47,
            SlotComponentType.BannerPatterns => 48,
            SlotComponentType.BaseColor => 49,
            SlotComponentType.PotDecorations => 50,
            SlotComponentType.Container => 51,
            SlotComponentType.BlockState => 52,
            SlotComponentType.Bees => 53,
            SlotComponentType.Lock => 54,
            SlotComponentType.ContainerLoot => 55,
            _ => throw new InvalidOperationException($"Unknown SlotComponentType {value} for protocol {protocolVersion}.")
        };
    }

    private static int ToId767(SlotComponentType value, int protocolVersion)
    {
        return value switch
        {
            SlotComponentType.CustomData => 0,
            SlotComponentType.MaxStackSize => 1,
            SlotComponentType.MaxDamage => 2,
            SlotComponentType.Damage => 3,
            SlotComponentType.Unbreakable => 4,
            SlotComponentType.CustomName => 5,
            SlotComponentType.ItemName => 6,
            SlotComponentType.Lore => 7,
            SlotComponentType.Rarity => 8,
            SlotComponentType.Enchantments => 9,
            SlotComponentType.CanPlaceOn => 10,
            SlotComponentType.CanBreak => 11,
            SlotComponentType.AttributeModifiers => 12,
            SlotComponentType.CustomModelData => 13,
            SlotComponentType.HideAdditionalTooltip => 14,
            SlotComponentType.HideTooltip => 15,
            SlotComponentType.RepairCost => 16,
            SlotComponentType.CreativeSlotLock => 17,
            SlotComponentType.EnchantmentGlintOverride => 18,
            SlotComponentType.IntangibleProjectile => 19,
            SlotComponentType.Food => 20,
            SlotComponentType.FireResistant => 21,
            SlotComponentType.Tool => 22,
            SlotComponentType.StoredEnchantments => 23,
            SlotComponentType.DyedColor => 24,
            SlotComponentType.MapColor => 25,
            SlotComponentType.MapId => 26,
            SlotComponentType.MapDecorations => 27,
            SlotComponentType.MapPostProcessing => 28,
            SlotComponentType.ChargedProjectiles => 29,
            SlotComponentType.BundleContents => 30,
            SlotComponentType.PotionContents => 31,
            SlotComponentType.SuspiciousStewEffects => 32,
            SlotComponentType.WritableBookContent => 33,
            SlotComponentType.WrittenBookContent => 34,
            SlotComponentType.Trim => 35,
            SlotComponentType.DebugStickState => 36,
            SlotComponentType.EntityData => 37,
            SlotComponentType.BucketEntityData => 38,
            SlotComponentType.BlockEntityData => 39,
            SlotComponentType.Instrument => 40,
            SlotComponentType.OminousBottleAmplifier => 41,
            SlotComponentType.JukeboxPlayable => 42,
            SlotComponentType.Recipes => 43,
            SlotComponentType.LodestoneTracker => 44,
            SlotComponentType.FireworkExplosion => 45,
            SlotComponentType.Fireworks => 46,
            SlotComponentType.Profile => 47,
            SlotComponentType.NoteBlockSound => 48,
            SlotComponentType.BannerPatterns => 49,
            SlotComponentType.BaseColor => 50,
            SlotComponentType.PotDecorations => 51,
            SlotComponentType.Container => 52,
            SlotComponentType.BlockState => 53,
            SlotComponentType.Bees => 54,
            SlotComponentType.Lock => 55,
            SlotComponentType.ContainerLoot => 56,
            _ => throw new InvalidOperationException($"Unknown SlotComponentType {value} for protocol {protocolVersion}.")
        };
    }

    private static int ToId768To769(SlotComponentType value, int protocolVersion)
    {
        return value switch
        {
            SlotComponentType.CustomData => 0,
            SlotComponentType.MaxStackSize => 1,
            SlotComponentType.MaxDamage => 2,
            SlotComponentType.Damage => 3,
            SlotComponentType.Unbreakable => 4,
            SlotComponentType.CustomName => 5,
            SlotComponentType.ItemName => 6,
            SlotComponentType.ItemModel => 7,
            SlotComponentType.Lore => 8,
            SlotComponentType.Rarity => 9,
            SlotComponentType.Enchantments => 10,
            SlotComponentType.CanPlaceOn => 11,
            SlotComponentType.CanBreak => 12,
            SlotComponentType.AttributeModifiers => 13,
            SlotComponentType.CustomModelData => 14,
            SlotComponentType.HideAdditionalTooltip => 15,
            SlotComponentType.HideTooltip => 16,
            SlotComponentType.RepairCost => 17,
            SlotComponentType.CreativeSlotLock => 18,
            SlotComponentType.EnchantmentGlintOverride => 19,
            SlotComponentType.IntangibleProjectile => 20,
            SlotComponentType.Food => 21,
            SlotComponentType.Consumable => 22,
            SlotComponentType.UseRemainder => 23,
            SlotComponentType.UseCooldown => 24,
            SlotComponentType.DamageResistant => 25,
            SlotComponentType.Tool => 26,
            SlotComponentType.Enchantable => 27,
            SlotComponentType.Equippable => 28,
            SlotComponentType.Repairable => 29,
            SlotComponentType.Glider => 30,
            SlotComponentType.TooltipStyle => 31,
            SlotComponentType.DeathProtection => 32,
            SlotComponentType.StoredEnchantments => 33,
            SlotComponentType.DyedColor => 34,
            SlotComponentType.MapColor => 35,
            SlotComponentType.MapId => 36,
            SlotComponentType.MapDecorations => 37,
            SlotComponentType.MapPostProcessing => 38,
            SlotComponentType.ChargedProjectiles => 39,
            SlotComponentType.BundleContents => 40,
            SlotComponentType.PotionContents => 41,
            SlotComponentType.SuspiciousStewEffects => 42,
            SlotComponentType.WritableBookContent => 43,
            SlotComponentType.WrittenBookContent => 44,
            SlotComponentType.Trim => 45,
            SlotComponentType.DebugStickState => 46,
            SlotComponentType.EntityData => 47,
            SlotComponentType.BucketEntityData => 48,
            SlotComponentType.BlockEntityData => 49,
            SlotComponentType.Instrument => 50,
            SlotComponentType.OminousBottleAmplifier => 51,
            SlotComponentType.JukeboxPlayable => 52,
            SlotComponentType.Recipes => 53,
            SlotComponentType.LodestoneTracker => 54,
            SlotComponentType.FireworkExplosion => 55,
            SlotComponentType.Fireworks => 56,
            SlotComponentType.Profile => 57,
            SlotComponentType.NoteBlockSound => 58,
            SlotComponentType.BannerPatterns => 59,
            SlotComponentType.BaseColor => 60,
            SlotComponentType.PotDecorations => 61,
            SlotComponentType.Container => 62,
            SlotComponentType.BlockState => 63,
            SlotComponentType.Bees => 64,
            SlotComponentType.Lock => 65,
            SlotComponentType.ContainerLoot => 66,
            _ => throw new InvalidOperationException($"Unknown SlotComponentType {value} for protocol {protocolVersion}.")
        };
    }

    private static int ToId770To772(SlotComponentType value, int protocolVersion)
    {
        return value switch
        {
            SlotComponentType.CustomData => 0,
            SlotComponentType.MaxStackSize => 1,
            SlotComponentType.MaxDamage => 2,
            SlotComponentType.Damage => 3,
            SlotComponentType.Unbreakable => 4,
            SlotComponentType.CustomName => 5,
            SlotComponentType.ItemName => 6,
            SlotComponentType.ItemModel => 7,
            SlotComponentType.Lore => 8,
            SlotComponentType.Rarity => 9,
            SlotComponentType.Enchantments => 10,
            SlotComponentType.CanPlaceOn => 11,
            SlotComponentType.CanBreak => 12,
            SlotComponentType.AttributeModifiers => 13,
            SlotComponentType.CustomModelData => 14,
            SlotComponentType.TooltipDisplay => 15,
            SlotComponentType.RepairCost => 16,
            SlotComponentType.CreativeSlotLock => 17,
            SlotComponentType.EnchantmentGlintOverride => 18,
            SlotComponentType.IntangibleProjectile => 19,
            SlotComponentType.Food => 20,
            SlotComponentType.Consumable => 21,
            SlotComponentType.UseRemainder => 22,
            SlotComponentType.UseCooldown => 23,
            SlotComponentType.DamageResistant => 24,
            SlotComponentType.Tool => 25,
            SlotComponentType.Weapon => 26,
            SlotComponentType.Enchantable => 27,
            SlotComponentType.Equippable => 28,
            SlotComponentType.Repairable => 29,
            SlotComponentType.Glider => 30,
            SlotComponentType.TooltipStyle => 31,
            SlotComponentType.DeathProtection => 32,
            SlotComponentType.BlocksAttacks => 33,
            SlotComponentType.StoredEnchantments => 34,
            SlotComponentType.DyedColor => 35,
            SlotComponentType.MapColor => 36,
            SlotComponentType.MapId => 37,
            SlotComponentType.MapDecorations => 38,
            SlotComponentType.MapPostProcessing => 39,
            SlotComponentType.PotionDurationScale => 40,
            SlotComponentType.ChargedProjectiles => 41,
            SlotComponentType.BundleContents => 42,
            SlotComponentType.PotionContents => 43,
            SlotComponentType.SuspiciousStewEffects => 44,
            SlotComponentType.WritableBookContent => 45,
            SlotComponentType.WrittenBookContent => 46,
            SlotComponentType.Trim => 47,
            SlotComponentType.DebugStickState => 48,
            SlotComponentType.EntityData => 49,
            SlotComponentType.BucketEntityData => 50,
            SlotComponentType.BlockEntityData => 51,
            SlotComponentType.Instrument => 52,
            SlotComponentType.ProvidesTrimMaterial => 53,
            SlotComponentType.OminousBottleAmplifier => 54,
            SlotComponentType.JukeboxPlayable => 55,
            SlotComponentType.ProvidesBannerPatterns => 56,
            SlotComponentType.Recipes => 57,
            SlotComponentType.LodestoneTracker => 58,
            SlotComponentType.FireworkExplosion => 59,
            SlotComponentType.Fireworks => 60,
            SlotComponentType.Profile => 61,
            SlotComponentType.NoteBlockSound => 62,
            SlotComponentType.BannerPatterns => 63,
            SlotComponentType.BaseColor => 64,
            SlotComponentType.PotDecorations => 65,
            SlotComponentType.Container => 66,
            SlotComponentType.BlockState => 67,
            SlotComponentType.Bees => 68,
            SlotComponentType.Lock => 69,
            SlotComponentType.ContainerLoot => 70,
            SlotComponentType.BreakSound => 71,
            SlotComponentType.VillagerVariant => 72,
            SlotComponentType.WolfVariant => 73,
            SlotComponentType.WolfSoundVariant => 74,
            SlotComponentType.WolfCollar => 75,
            SlotComponentType.FoxVariant => 76,
            SlotComponentType.SalmonSize => 77,
            SlotComponentType.ParrotVariant => 78,
            SlotComponentType.TropicalFishPattern => 79,
            SlotComponentType.TropicalFishBaseColor => 80,
            SlotComponentType.TropicalFishPatternColor => 81,
            SlotComponentType.MooshroomVariant => 82,
            SlotComponentType.RabbitVariant => 83,
            SlotComponentType.PigVariant => 84,
            SlotComponentType.CowVariant => 85,
            SlotComponentType.ChickenVariant => 86,
            SlotComponentType.FrogVariant => 87,
            SlotComponentType.HorseVariant => 88,
            SlotComponentType.PaintingVariant => 89,
            SlotComponentType.LlamaVariant => 90,
            SlotComponentType.AxolotlVariant => 91,
            SlotComponentType.CatVariant => 92,
            SlotComponentType.CatCollar => 93,
            SlotComponentType.SheepColor => 94,
            SlotComponentType.ShulkerColor => 95,
            _ => throw new InvalidOperationException($"Unknown SlotComponentType {value} for protocol {protocolVersion}.")
        };
    }
}
