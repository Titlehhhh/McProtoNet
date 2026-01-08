using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Serialization;
using static McProtoNet.Protocol.SlotComponent;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static SlotComponent ReadSlotComponent(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotComponent>(protocolVersion);
        var type = reader.ReadSlotComponentType(protocolVersion);
        return type switch
        {
            SlotComponentType.CustomData => new CustomData(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("custom_data missing")),
            SlotComponentType.MaxStackSize => new MaxStackSize(reader.ReadVarInt()),
            SlotComponentType.MaxDamage => new MaxDamage(reader.ReadVarInt()),
            SlotComponentType.Damage => new Damage(reader.ReadVarInt()),
            SlotComponentType.Unbreakable => protocolVersion >= 770
                ? new Unbreakable(null)
                : new Unbreakable(reader.ReadBoolean()),
            SlotComponentType.CustomName => new CustomName(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("custom_name missing")),
            SlotComponentType.ItemName => new ItemName(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("item_name missing")),
            SlotComponentType.ItemModel => protocolVersion >= 768
                ? new ItemModel(reader.ReadString())
                : throw new InvalidOperationException("item_model is not supported before protocol 768"),
            SlotComponentType.Lore => new Lore(ReadLore(ref reader, protocolVersion)),
            SlotComponentType.Rarity => new Rarity(ReadRarity(reader.ReadVarInt())),
            SlotComponentType.Enchantments => new Enchantments(ReadEnchantments(ref reader),
                protocolVersion <= 769 ? reader.ReadBoolean() : null),
            SlotComponentType.CanPlaceOn => new CanPlaceOn(ReadItemBlockPredicates(ref reader, protocolVersion),
                protocolVersion <= 769 ? reader.ReadBoolean() : null),
            SlotComponentType.CanBreak => new CanBreak(ReadItemBlockPredicates(ref reader, protocolVersion),
                protocolVersion <= 769 ? reader.ReadBoolean() : null),
            SlotComponentType.AttributeModifiers => ReadAttributeModifiers(ref reader, protocolVersion),
            SlotComponentType.CustomModelData => ReadCustomModelData(ref reader, protocolVersion),
            SlotComponentType.HideAdditionalTooltip => protocolVersion <= 769
                ? new HideAdditionalTooltip()
                : throw new InvalidOperationException("hide_additional_tooltip not supported in protocol 770+"),
            SlotComponentType.HideTooltip => protocolVersion <= 769
                ? new HideTooltip()
                : throw new InvalidOperationException("hide_tooltip not supported in protocol 770+"),
            SlotComponentType.TooltipDisplay => protocolVersion >= 770
                ? new TooltipDisplay(ReadTooltipDisplay(ref reader))
                : throw new InvalidOperationException("tooltip_display is not supported before protocol 770"),
            SlotComponentType.RepairCost => new RepairCost(reader.ReadVarInt()),
            SlotComponentType.CreativeSlotLock => new CreativeSlotLock(),
            SlotComponentType.EnchantmentGlintOverride => new EnchantmentGlintOverride(reader.ReadBoolean()),
            SlotComponentType.IntangibleProjectile => protocolVersion >= 770
                ? new IntangibleProjectile(null)
                : new IntangibleProjectile(reader.ReadAnonymousNbtTag(protocolVersion)),
            SlotComponentType.Food => new Food(ReadFood(ref reader, protocolVersion)),
            SlotComponentType.FireResistant => protocolVersion <= 769
                ? new FireResistant()
                : throw new InvalidOperationException("fire_resistant not supported in protocol 770+"),
            SlotComponentType.Tool => new Tool(ReadTool(ref reader, protocolVersion)),
            SlotComponentType.StoredEnchantments => new StoredEnchantments(ReadEnchantments(ref reader),
                protocolVersion <= 769 ? reader.ReadBoolean() : null),
            SlotComponentType.DyedColor => protocolVersion >= 770
                ? new DyedColor(reader.ReadSignedInt(), null)
                : new DyedColor(reader.ReadSignedInt(), reader.ReadBoolean()),
            SlotComponentType.MapColor => new MapColor(reader.ReadSignedInt()),
            SlotComponentType.MapId => new MapId(reader.ReadVarInt()),
            SlotComponentType.MapDecorations => new MapDecorations(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("map_decorations missing")),
            SlotComponentType.MapPostProcessing => new MapPostProcessing(reader.ReadVarInt()),
            SlotComponentType.ChargedProjectiles => new ChargedProjectiles(ReadSlotArray(ref reader, protocolVersion)),
            SlotComponentType.BundleContents => new BundleContents(ReadSlotArray(ref reader, protocolVersion)),
            SlotComponentType.PotionContents => new PotionContents(ReadPotionContents(ref reader, protocolVersion)),
            SlotComponentType.SuspiciousStewEffects => new SuspiciousStewEffects(ReadSuspiciousStewEffects(ref reader)),
            SlotComponentType.WritableBookContent => new WritableBookContent(ReadItemBookPages(ref reader, protocolVersion)),
            SlotComponentType.WrittenBookContent => new WrittenBookContent(ReadWrittenBookContent(ref reader, protocolVersion)),
            SlotComponentType.Trim => new Trim(ReadTrim(ref reader, protocolVersion)),
            SlotComponentType.DebugStickState => new DebugStickState(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("debug_stick_state missing")),
            SlotComponentType.EntityData => new EntityData(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("entity_data missing")),
            SlotComponentType.BucketEntityData => new BucketEntityData(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("bucket_entity_data missing")),
            SlotComponentType.BlockEntityData => new BlockEntityData(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("block_entity_data missing")),
            SlotComponentType.Instrument => new Instrument(ReadInstrument(ref reader, protocolVersion)),
            SlotComponentType.OminousBottleAmplifier => new OminousBottleAmplifier(reader.ReadVarInt()),
            SlotComponentType.Recipes => new Recipes(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("recipes missing")),
            SlotComponentType.LodestoneTracker => new LodestoneTracker(ReadLodestoneTracker(ref reader, protocolVersion)),
            SlotComponentType.FireworkExplosion => new FireworkExplosion(reader.ReadItemFireworkExplosion(protocolVersion)),
            SlotComponentType.Fireworks => new Fireworks(ReadFireworks(ref reader, protocolVersion)),
            SlotComponentType.Profile => new Profile(ReadProfile(ref reader, protocolVersion)),
            SlotComponentType.NoteBlockSound => new NoteBlockSound(reader.ReadString()),
            SlotComponentType.BannerPatterns => new BannerPatterns(ReadBannerPatterns(ref reader, protocolVersion)),
            SlotComponentType.BaseColor => new BaseColor(reader.ReadVarInt()),
            SlotComponentType.PotDecorations => new PotDecorations(ReadVarIntArray(ref reader)),
            SlotComponentType.Container => new Container(ReadSlotArray(ref reader, protocolVersion)),
            SlotComponentType.BlockState => new BlockState(ReadBlockState(ref reader, protocolVersion)),
            SlotComponentType.Bees => new Bees(ReadBees(ref reader, protocolVersion)),
            SlotComponentType.Lock => new SlotComponent.Lock(reader.ReadAnonymousNbtTag(protocolVersion)
                ?? throw new InvalidOperationException("lock missing")),
            SlotComponentType.ContainerLoot => new ContainerLoot(reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("container_loot missing")),
            SlotComponentType.JukeboxPlayable => protocolVersion >= 767
                ? new JukeboxPlayable(ReadJukeboxPlayable(ref reader, protocolVersion))
                : throw new InvalidOperationException("jukebox_playable is not supported before protocol 767"),
            SlotComponentType.Consumable => protocolVersion >= 768
                ? new Consumable(ReadConsumable(ref reader, protocolVersion))
                : throw new InvalidOperationException("consumable is not supported before protocol 768"),
            SlotComponentType.UseRemainder => protocolVersion >= 768
                ? new UseRemainder(reader.ReadSlot(protocolVersion))
                : throw new InvalidOperationException("use_remainder is not supported before protocol 768"),
            SlotComponentType.UseCooldown => protocolVersion >= 768
                ? new UseCooldown(ReadUseCooldown(ref reader))
                : throw new InvalidOperationException("use_cooldown is not supported before protocol 768"),
            SlotComponentType.DamageResistant => protocolVersion >= 768
                ? new DamageResistant(reader.ReadString())
                : throw new InvalidOperationException("damage_resistant is not supported before protocol 768"),
            SlotComponentType.Enchantable => protocolVersion >= 768
                ? new Enchantable(reader.ReadVarInt())
                : throw new InvalidOperationException("enchantable is not supported before protocol 768"),
            SlotComponentType.Equippable => protocolVersion >= 768
                ? new Equippable(ReadEquippable(ref reader, protocolVersion))
                : throw new InvalidOperationException("equippable is not supported before protocol 768"),
            SlotComponentType.Repairable => protocolVersion >= 768
                ? new Repairable(reader.ReadIDSet(protocolVersion))
                : throw new InvalidOperationException("repairable is not supported before protocol 768"),
            SlotComponentType.Glider => protocolVersion >= 768
                ? new Glider()
                : throw new InvalidOperationException("glider is not supported before protocol 768"),
            SlotComponentType.TooltipStyle => protocolVersion >= 768
                ? new TooltipStyle(reader.ReadString())
                : throw new InvalidOperationException("tooltip_style is not supported before protocol 768"),
            SlotComponentType.DeathProtection => protocolVersion >= 768
                ? new DeathProtection(ReadItemConsumeEffects(ref reader, protocolVersion))
                : throw new InvalidOperationException("death_protection is not supported before protocol 768"),
            SlotComponentType.Weapon => protocolVersion >= 770
                ? new Weapon(ReadWeapon(ref reader))
                : throw new InvalidOperationException("weapon is not supported before protocol 770"),
            SlotComponentType.BlocksAttacks => protocolVersion >= 770
                ? new BlocksAttacks(ReadBlocksAttacks(ref reader, protocolVersion))
                : throw new InvalidOperationException("blocks_attacks is not supported before protocol 770"),
            SlotComponentType.PotionDurationScale => protocolVersion >= 770
                ? new PotionDurationScale(reader.ReadFloat())
                : throw new InvalidOperationException("potion_duration_scale is not supported before protocol 770"),
            SlotComponentType.ProvidesTrimMaterial => protocolVersion >= 770
                ? new ProvidesTrimMaterial(ReadProvidesTrimMaterial(ref reader, protocolVersion))
                : throw new InvalidOperationException("provides_trim_material is not supported before protocol 770"),
            SlotComponentType.ProvidesBannerPatterns => protocolVersion >= 770
                ? new ProvidesBannerPatterns(reader.ReadString())
                : throw new InvalidOperationException("provides_banner_patterns is not supported before protocol 770"),
            SlotComponentType.BreakSound => protocolVersion >= 770
                ? new BreakSound(reader.ReadItemSoundHolder(protocolVersion))
                : throw new InvalidOperationException("break_sound is not supported before protocol 770"),
            SlotComponentType.VillagerVariant => protocolVersion >= 770
                ? new VillagerVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("villager/variant is not supported before protocol 770"),
            SlotComponentType.WolfVariant => protocolVersion >= 770
                ? new WolfVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("wolf/variant is not supported before protocol 770"),
            SlotComponentType.WolfSoundVariant => protocolVersion >= 770
                ? new WolfSoundVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("wolf/sound_variant is not supported before protocol 770"),
            SlotComponentType.WolfCollar => protocolVersion >= 770
                ? new WolfCollar(reader.ReadVarInt())
                : throw new InvalidOperationException("wolf/collar is not supported before protocol 770"),
            SlotComponentType.FoxVariant => protocolVersion >= 770
                ? new FoxVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("fox/variant is not supported before protocol 770"),
            SlotComponentType.SalmonSize => protocolVersion >= 770
                ? new SalmonSize(reader.ReadVarInt())
                : throw new InvalidOperationException("salmon/size is not supported before protocol 770"),
            SlotComponentType.ParrotVariant => protocolVersion >= 770
                ? new ParrotVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("parrot/variant is not supported before protocol 770"),
            SlotComponentType.TropicalFishPattern => protocolVersion >= 770
                ? new TropicalFishPattern(reader.ReadVarInt())
                : throw new InvalidOperationException("tropical_fish/pattern is not supported before protocol 770"),
            SlotComponentType.TropicalFishBaseColor => protocolVersion >= 770
                ? new TropicalFishBaseColor(reader.ReadVarInt())
                : throw new InvalidOperationException("tropical_fish/base_color is not supported before protocol 770"),
            SlotComponentType.TropicalFishPatternColor => protocolVersion >= 770
                ? new TropicalFishPatternColor(reader.ReadVarInt())
                : throw new InvalidOperationException("tropical_fish/pattern_color is not supported before protocol 770"),
            SlotComponentType.MooshroomVariant => protocolVersion >= 770
                ? new MooshroomVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("mooshroom/variant is not supported before protocol 770"),
            SlotComponentType.RabbitVariant => protocolVersion >= 770
                ? new RabbitVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("rabbit/variant is not supported before protocol 770"),
            SlotComponentType.PigVariant => protocolVersion >= 770
                ? new PigVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("pig/variant is not supported before protocol 770"),
            SlotComponentType.CowVariant => protocolVersion >= 770
                ? new CowVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("cow/variant is not supported before protocol 770"),
            SlotComponentType.ChickenVariant => protocolVersion >= 770
                ? new ChickenVariant(reader.ReadRegistryEntryHolder<string>(protocolVersion))
                : throw new InvalidOperationException("chicken/variant is not supported before protocol 770"),
            SlotComponentType.FrogVariant => protocolVersion >= 770
                ? new FrogVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("frog/variant is not supported before protocol 770"),
            SlotComponentType.HorseVariant => protocolVersion >= 770
                ? new HorseVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("horse/variant is not supported before protocol 770"),
            SlotComponentType.PaintingVariant => protocolVersion >= 770
                ? new PaintingVariant(reader.ReadRegistryEntryHolder<EntityMetadataPaintingVariant>(protocolVersion))
                : throw new InvalidOperationException("painting/variant is not supported before protocol 770"),
            SlotComponentType.LlamaVariant => protocolVersion >= 770
                ? new LlamaVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("llama/variant is not supported before protocol 770"),
            SlotComponentType.AxolotlVariant => protocolVersion >= 770
                ? new AxolotlVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("axolotl/variant is not supported before protocol 770"),
            SlotComponentType.CatVariant => protocolVersion >= 770
                ? new CatVariant(reader.ReadVarInt())
                : throw new InvalidOperationException("cat/variant is not supported before protocol 770"),
            SlotComponentType.CatCollar => protocolVersion >= 770
                ? new CatCollar(reader.ReadVarInt())
                : throw new InvalidOperationException("cat/collar is not supported before protocol 770"),
            SlotComponentType.SheepColor => protocolVersion >= 770
                ? new SheepColor(reader.ReadVarInt())
                : throw new InvalidOperationException("sheep/color is not supported before protocol 770"),
            SlotComponentType.ShulkerColor => protocolVersion >= 770
                ? new ShulkerColor(reader.ReadVarInt())
                : throw new InvalidOperationException("shulker/color is not supported before protocol 770"),
            _ => throw new InvalidOperationException($"Unhandled SlotComponentType {type}")
        };
    }

    public static void WriteSlotComponent(this ref MinecraftPrimitiveWriter writer, SlotComponent component, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotComponent>(protocolVersion);
        var type = GetComponentType(component);
        writer.WriteSlotComponentType(type, protocolVersion);
        switch (component)
        {
            case CustomData customData:
                writer.WriteAnonymousNbtTag(customData.Data, protocolVersion);
                break;
            case MaxStackSize maxStackSize:
                writer.WriteVarInt(maxStackSize.Value);
                break;
            case MaxDamage maxDamage:
                writer.WriteVarInt(maxDamage.Value);
                break;
            case Damage damage:
                writer.WriteVarInt(damage.Value);
                break;
            case Unbreakable unbreakable:
                if (protocolVersion <= 769)
                {
                    writer.WriteBoolean(unbreakable.Value ?? false);
                }
                break;
            case CustomName customName:
                writer.WriteAnonymousNbtTag(customName.Data, protocolVersion);
                break;
            case ItemName itemName:
                writer.WriteAnonymousNbtTag(itemName.Data, protocolVersion);
                break;
            case ItemModel itemModel:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("item_model is not supported before protocol 768");
                }
                writer.WriteString(itemModel.Model);
                break;
            case Lore lore:
                WriteLore(ref writer, lore.Lines, protocolVersion);
                break;
            case Rarity rarity:
                writer.WriteVarInt(WriteRarity(rarity.Value));
                break;
            case Enchantments enchantments:
                WriteEnchantments(ref writer, enchantments.Entries);
                if (protocolVersion <= 769)
                {
                    writer.WriteBoolean(enchantments.ShowTooltip ?? false);
                }
                break;
            case CanPlaceOn canPlaceOn:
                WriteItemBlockPredicates(ref writer, canPlaceOn.Predicates, protocolVersion);
                if (protocolVersion <= 769)
                {
                    writer.WriteBoolean(canPlaceOn.ShowTooltip ?? false);
                }
                break;
            case CanBreak canBreak:
                WriteItemBlockPredicates(ref writer, canBreak.Predicates, protocolVersion);
                if (protocolVersion <= 769)
                {
                    writer.WriteBoolean(canBreak.ShowTooltip ?? false);
                }
                break;
            case AttributeModifiers attributeModifiers:
                WriteAttributeModifiers(ref writer, attributeModifiers, protocolVersion);
                break;
            case CustomModelData customModelData:
                WriteCustomModelData(ref writer, customModelData, protocolVersion);
                break;
            case HideAdditionalTooltip:
                if (protocolVersion >= 770)
                {
                    throw new InvalidOperationException("hide_additional_tooltip not supported in protocol 770+");
                }
                break;
            case HideTooltip:
                if (protocolVersion >= 770)
                {
                    throw new InvalidOperationException("hide_tooltip not supported in protocol 770+");
                }
                break;
            case TooltipDisplay tooltipDisplay:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("tooltip_display is not supported before protocol 770");
                }
                WriteTooltipDisplay(ref writer, tooltipDisplay.Data);
                break;
            case RepairCost repairCost:
                writer.WriteVarInt(repairCost.Value);
                break;
            case CreativeSlotLock:
                break;
            case EnchantmentGlintOverride glintOverride:
                writer.WriteBoolean(glintOverride.Value);
                break;
            case IntangibleProjectile intangibleProjectile:
                if (protocolVersion <= 769)
                {
                    writer.WriteAnonymousNbtTag(intangibleProjectile.Data ?? throw new InvalidOperationException("intangible_projectile missing"),
                        protocolVersion);
                }
                break;
            case Food food:
                WriteFood(ref writer, food.Data, protocolVersion);
                break;
            case FireResistant:
                if (protocolVersion >= 770)
                {
                    throw new InvalidOperationException("fire_resistant not supported in protocol 770+");
                }
                break;
            case Tool tool:
                WriteTool(ref writer, tool.Data, protocolVersion);
                break;
            case StoredEnchantments storedEnchantments:
                WriteEnchantments(ref writer, storedEnchantments.Entries);
                if (protocolVersion <= 769)
                {
                    writer.WriteBoolean(storedEnchantments.ShowInTooltip ?? false);
                }
                break;
            case DyedColor dyedColor:
                writer.WriteSignedInt(dyedColor.Color);
                if (protocolVersion <= 769)
                {
                    writer.WriteBoolean(dyedColor.ShowTooltip ?? false);
                }
                break;
            case MapColor mapColor:
                writer.WriteSignedInt(mapColor.Color);
                break;
            case MapId mapId:
                writer.WriteVarInt(mapId.Value);
                break;
            case MapDecorations mapDecorations:
                writer.WriteAnonymousNbtTag(mapDecorations.Data, protocolVersion);
                break;
            case MapPostProcessing mapPostProcessing:
                writer.WriteVarInt(mapPostProcessing.Value);
                break;
            case ChargedProjectiles chargedProjectiles:
                WriteSlotArray(ref writer, chargedProjectiles.Projectiles, protocolVersion);
                break;
            case BundleContents bundleContents:
                WriteSlotArray(ref writer, bundleContents.Contents, protocolVersion);
                break;
            case PotionContents potionContents:
                WritePotionContents(ref writer, potionContents.Data, protocolVersion);
                break;
            case SuspiciousStewEffects suspiciousStewEffects:
                WriteSuspiciousStewEffects(ref writer, suspiciousStewEffects.Effects);
                break;
            case WritableBookContent writableBookContent:
                WriteItemBookPages(ref writer, writableBookContent.Pages, protocolVersion);
                break;
            case WrittenBookContent writtenBookContent:
                WriteWrittenBookContent(ref writer, writtenBookContent.Data, protocolVersion);
                break;
            case Trim trim:
                WriteTrim(ref writer, trim.Data, protocolVersion);
                break;
            case DebugStickState debugStickState:
                writer.WriteAnonymousNbtTag(debugStickState.Data, protocolVersion);
                break;
            case EntityData entityData:
                writer.WriteAnonymousNbtTag(entityData.Data, protocolVersion);
                break;
            case BucketEntityData bucketEntityData:
                writer.WriteAnonymousNbtTag(bucketEntityData.Data, protocolVersion);
                break;
            case BlockEntityData blockEntityData:
                writer.WriteAnonymousNbtTag(blockEntityData.Data, protocolVersion);
                break;
            case Instrument instrument:
                WriteInstrument(ref writer, instrument.Data, protocolVersion);
                break;
            case OminousBottleAmplifier ominousBottleAmplifier:
                writer.WriteVarInt(ominousBottleAmplifier.Value);
                break;
            case Recipes recipes:
                writer.WriteAnonymousNbtTag(recipes.Data, protocolVersion);
                break;
            case LodestoneTracker lodestoneTracker:
                WriteLodestoneTracker(ref writer, lodestoneTracker.Data, protocolVersion);
                break;
            case FireworkExplosion fireworkExplosion:
                writer.WriteItemFireworkExplosion(fireworkExplosion.Explosion, protocolVersion);
                break;
            case Fireworks fireworks:
                WriteFireworks(ref writer, fireworks.Data, protocolVersion);
                break;
            case Profile profile:
                WriteProfile(ref writer, profile.Data, protocolVersion);
                break;
            case NoteBlockSound noteBlockSound:
                writer.WriteString(noteBlockSound.Value);
                break;
            case BannerPatterns bannerPatterns:
                WriteBannerPatterns(ref writer, bannerPatterns.Layers, protocolVersion);
                break;
            case BaseColor baseColor:
                writer.WriteVarInt(baseColor.Value);
                break;
            case PotDecorations potDecorations:
                WriteVarIntArray(ref writer, potDecorations.Decorations);
                break;
            case Container container:
                WriteSlotArray(ref writer, container.Contents, protocolVersion);
                break;
            case BlockState blockState:
                WriteBlockState(ref writer, blockState.Properties, protocolVersion);
                break;
            case Bees bees:
                WriteBees(ref writer, bees.BeeEntries, protocolVersion);
                break;
            case SlotComponent.Lock lockData:
                writer.WriteAnonymousNbtTag(lockData.Data, protocolVersion);
                break;
            case ContainerLoot containerLoot:
                writer.WriteAnonymousNbtTag(containerLoot.Data, protocolVersion);
                break;
            case JukeboxPlayable jukeboxPlayable:
                if (protocolVersion < 767)
                {
                    throw new InvalidOperationException("jukebox_playable is not supported before protocol 767");
                }
                WriteJukeboxPlayable(ref writer, jukeboxPlayable.Data, protocolVersion);
                break;
            case Consumable consumable:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("consumable is not supported before protocol 768");
                }
                WriteConsumable(ref writer, consumable.Data, protocolVersion);
                break;
            case UseRemainder useRemainder:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("use_remainder is not supported before protocol 768");
                }
                writer.WriteSlot(useRemainder.Value, protocolVersion);
                break;
            case UseCooldown useCooldown:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("use_cooldown is not supported before protocol 768");
                }
                WriteUseCooldown(ref writer, useCooldown.Data);
                break;
            case DamageResistant damageResistant:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("damage_resistant is not supported before protocol 768");
                }
                writer.WriteString(damageResistant.Value);
                break;
            case Enchantable enchantable:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("enchantable is not supported before protocol 768");
                }
                writer.WriteVarInt(enchantable.Value);
                break;
            case Equippable equippable:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("equippable is not supported before protocol 768");
                }
                WriteEquippable(ref writer, equippable.Data, protocolVersion);
                break;
            case Repairable repairable:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("repairable is not supported before protocol 768");
                }
                writer.WriteIDSet(repairable.Items, protocolVersion);
                break;
            case Glider:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("glider is not supported before protocol 768");
                }
                break;
            case TooltipStyle tooltipStyle:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("tooltip_style is not supported before protocol 768");
                }
                writer.WriteString(tooltipStyle.Value);
                break;
            case DeathProtection deathProtection:
                if (protocolVersion < 768)
                {
                    throw new InvalidOperationException("death_protection is not supported before protocol 768");
                }
                WriteItemConsumeEffects(ref writer, deathProtection.Effects, protocolVersion);
                break;
            case Weapon weapon:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("weapon is not supported before protocol 770");
                }
                WriteWeapon(ref writer, weapon.Data);
                break;
            case BlocksAttacks blocksAttacks:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("blocks_attacks is not supported before protocol 770");
                }
                WriteBlocksAttacks(ref writer, blocksAttacks.Data, protocolVersion);
                break;
            case PotionDurationScale potionDurationScale:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("potion_duration_scale is not supported before protocol 770");
                }
                writer.WriteFloat(potionDurationScale.Value);
                break;
            case ProvidesTrimMaterial providesTrimMaterial:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("provides_trim_material is not supported before protocol 770");
                }
                WriteProvidesTrimMaterial(ref writer, providesTrimMaterial.Data, protocolVersion);
                break;
            case ProvidesBannerPatterns providesBannerPatterns:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("provides_banner_patterns is not supported before protocol 770");
                }
                writer.WriteString(providesBannerPatterns.Value);
                break;
            case BreakSound breakSound:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("break_sound is not supported before protocol 770");
                }
                writer.WriteItemSoundHolder(breakSound.Sound, protocolVersion);
                break;
            case VillagerVariant villagerVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("villager/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(villagerVariant.Value);
                break;
            case WolfVariant wolfVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("wolf/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(wolfVariant.Value);
                break;
            case WolfSoundVariant wolfSoundVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("wolf/sound_variant is not supported before protocol 770");
                }
                writer.WriteVarInt(wolfSoundVariant.Value);
                break;
            case WolfCollar wolfCollar:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("wolf/collar is not supported before protocol 770");
                }
                writer.WriteVarInt(wolfCollar.Value);
                break;
            case FoxVariant foxVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("fox/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(foxVariant.Value);
                break;
            case SalmonSize salmonSize:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("salmon/size is not supported before protocol 770");
                }
                writer.WriteVarInt(salmonSize.Value);
                break;
            case ParrotVariant parrotVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("parrot/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(parrotVariant.Value);
                break;
            case TropicalFishPattern tropicalFishPattern:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("tropical_fish/pattern is not supported before protocol 770");
                }
                writer.WriteVarInt(tropicalFishPattern.Value);
                break;
            case TropicalFishBaseColor tropicalFishBaseColor:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("tropical_fish/base_color is not supported before protocol 770");
                }
                writer.WriteVarInt(tropicalFishBaseColor.Value);
                break;
            case TropicalFishPatternColor tropicalFishPatternColor:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("tropical_fish/pattern_color is not supported before protocol 770");
                }
                writer.WriteVarInt(tropicalFishPatternColor.Value);
                break;
            case MooshroomVariant mooshroomVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("mooshroom/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(mooshroomVariant.Value);
                break;
            case RabbitVariant rabbitVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("rabbit/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(rabbitVariant.Value);
                break;
            case PigVariant pigVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("pig/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(pigVariant.Value);
                break;
            case CowVariant cowVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("cow/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(cowVariant.Value);
                break;
            case ChickenVariant chickenVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("chicken/variant is not supported before protocol 770");
                }
                writer.WriteRegistryEntryHolder(chickenVariant.Variant, protocolVersion);
                break;
            case FrogVariant frogVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("frog/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(frogVariant.Value);
                break;
            case HorseVariant horseVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("horse/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(horseVariant.Value);
                break;
            case PaintingVariant paintingVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("painting/variant is not supported before protocol 770");
                }
                writer.WriteRegistryEntryHolder(paintingVariant.Variant, protocolVersion);
                break;
            case LlamaVariant llamaVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("llama/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(llamaVariant.Value);
                break;
            case AxolotlVariant axolotlVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("axolotl/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(axolotlVariant.Value);
                break;
            case CatVariant catVariant:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("cat/variant is not supported before protocol 770");
                }
                writer.WriteVarInt(catVariant.Value);
                break;
            case CatCollar catCollar:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("cat/collar is not supported before protocol 770");
                }
                writer.WriteVarInt(catCollar.Value);
                break;
            case SheepColor sheepColor:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("sheep/color is not supported before protocol 770");
                }
                writer.WriteVarInt(sheepColor.Value);
                break;
            case ShulkerColor shulkerColor:
                if (protocolVersion < 770)
                {
                    throw new InvalidOperationException("shulker/color is not supported before protocol 770");
                }
                writer.WriteVarInt(shulkerColor.Value);
                break;
        }
    }

    private static SlotComponentType GetComponentType(SlotComponent component)
        => component switch
        {
            AttributeModifiers => SlotComponentType.AttributeModifiers,
            AxolotlVariant => SlotComponentType.AxolotlVariant,
            BannerPatterns => SlotComponentType.BannerPatterns,
            BaseColor => SlotComponentType.BaseColor,
            Bees => SlotComponentType.Bees,
            BlockEntityData => SlotComponentType.BlockEntityData,
            BlockState => SlotComponentType.BlockState,
            BlocksAttacks => SlotComponentType.BlocksAttacks,
            BreakSound => SlotComponentType.BreakSound,
            BucketEntityData => SlotComponentType.BucketEntityData,
            BundleContents => SlotComponentType.BundleContents,
            CanBreak => SlotComponentType.CanBreak,
            CanPlaceOn => SlotComponentType.CanPlaceOn,
            CatCollar => SlotComponentType.CatCollar,
            CatVariant => SlotComponentType.CatVariant,
            ChargedProjectiles => SlotComponentType.ChargedProjectiles,
            ChickenVariant => SlotComponentType.ChickenVariant,
            Consumable => SlotComponentType.Consumable,
            Container => SlotComponentType.Container,
            ContainerLoot => SlotComponentType.ContainerLoot,
            CowVariant => SlotComponentType.CowVariant,
            CreativeSlotLock => SlotComponentType.CreativeSlotLock,
            CustomData => SlotComponentType.CustomData,
            CustomModelData => SlotComponentType.CustomModelData,
            CustomName => SlotComponentType.CustomName,
            Damage => SlotComponentType.Damage,
            DamageResistant => SlotComponentType.DamageResistant,
            DeathProtection => SlotComponentType.DeathProtection,
            DebugStickState => SlotComponentType.DebugStickState,
            DyedColor => SlotComponentType.DyedColor,
            Enchantable => SlotComponentType.Enchantable,
            EnchantmentGlintOverride => SlotComponentType.EnchantmentGlintOverride,
            Enchantments => SlotComponentType.Enchantments,
            EntityData => SlotComponentType.EntityData,
            Equippable => SlotComponentType.Equippable,
            FireResistant => SlotComponentType.FireResistant,
            FireworkExplosion => SlotComponentType.FireworkExplosion,
            Fireworks => SlotComponentType.Fireworks,
            Food => SlotComponentType.Food,
            FoxVariant => SlotComponentType.FoxVariant,
            FrogVariant => SlotComponentType.FrogVariant,
            Glider => SlotComponentType.Glider,
            HideAdditionalTooltip => SlotComponentType.HideAdditionalTooltip,
            HideTooltip => SlotComponentType.HideTooltip,
            HorseVariant => SlotComponentType.HorseVariant,
            Instrument => SlotComponentType.Instrument,
            IntangibleProjectile => SlotComponentType.IntangibleProjectile,
            ItemModel => SlotComponentType.ItemModel,
            ItemName => SlotComponentType.ItemName,
            JukeboxPlayable => SlotComponentType.JukeboxPlayable,
            LlamaVariant => SlotComponentType.LlamaVariant,
            SlotComponent.Lock => SlotComponentType.Lock,
            LodestoneTracker => SlotComponentType.LodestoneTracker,
            Lore => SlotComponentType.Lore,
            MapColor => SlotComponentType.MapColor,
            MapDecorations => SlotComponentType.MapDecorations,
            MapId => SlotComponentType.MapId,
            MapPostProcessing => SlotComponentType.MapPostProcessing,
            MaxDamage => SlotComponentType.MaxDamage,
            MaxStackSize => SlotComponentType.MaxStackSize,
            MooshroomVariant => SlotComponentType.MooshroomVariant,
            NoteBlockSound => SlotComponentType.NoteBlockSound,
            OminousBottleAmplifier => SlotComponentType.OminousBottleAmplifier,
            PaintingVariant => SlotComponentType.PaintingVariant,
            ParrotVariant => SlotComponentType.ParrotVariant,
            PigVariant => SlotComponentType.PigVariant,
            PotDecorations => SlotComponentType.PotDecorations,
            PotionContents => SlotComponentType.PotionContents,
            PotionDurationScale => SlotComponentType.PotionDurationScale,
            Profile => SlotComponentType.Profile,
            ProvidesBannerPatterns => SlotComponentType.ProvidesBannerPatterns,
            ProvidesTrimMaterial => SlotComponentType.ProvidesTrimMaterial,
            RabbitVariant => SlotComponentType.RabbitVariant,
            Rarity => SlotComponentType.Rarity,
            Recipes => SlotComponentType.Recipes,
            RepairCost => SlotComponentType.RepairCost,
            Repairable => SlotComponentType.Repairable,
            SalmonSize => SlotComponentType.SalmonSize,
            SheepColor => SlotComponentType.SheepColor,
            ShulkerColor => SlotComponentType.ShulkerColor,
            StoredEnchantments => SlotComponentType.StoredEnchantments,
            SuspiciousStewEffects => SlotComponentType.SuspiciousStewEffects,
            Tool => SlotComponentType.Tool,
            TooltipDisplay => SlotComponentType.TooltipDisplay,
            TooltipStyle => SlotComponentType.TooltipStyle,
            Trim => SlotComponentType.Trim,
            TropicalFishBaseColor => SlotComponentType.TropicalFishBaseColor,
            TropicalFishPattern => SlotComponentType.TropicalFishPattern,
            TropicalFishPatternColor => SlotComponentType.TropicalFishPatternColor,
            Unbreakable => SlotComponentType.Unbreakable,
            UseCooldown => SlotComponentType.UseCooldown,
            UseRemainder => SlotComponentType.UseRemainder,
            VillagerVariant => SlotComponentType.VillagerVariant,
            Weapon => SlotComponentType.Weapon,
            WolfCollar => SlotComponentType.WolfCollar,
            WolfSoundVariant => SlotComponentType.WolfSoundVariant,
            WolfVariant => SlotComponentType.WolfVariant,
            WritableBookContent => SlotComponentType.WritableBookContent,
            WrittenBookContent => SlotComponentType.WrittenBookContent,
            _ => throw new InvalidOperationException($"Unknown SlotComponent {component.GetType()}")
        };


    private static AttributeModifiers ReadAttributeModifiers(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var attributes = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => ReadAttributeModifierEntry(ref r, protocolVersion));
        bool? showTooltip = null;
        AttributeModifierDisplay? display = null;
        if (protocolVersion <= 770)
        {
            showTooltip = reader.ReadBoolean();
        }
        else
        {
            display = ReadAttributeModifierDisplay(ref reader, protocolVersion);
        }

        return new AttributeModifiers(attributes, showTooltip, display);
    }

    private static AttributeModifierEntry ReadAttributeModifierEntry(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int typeId = reader.ReadVarInt();
        Guid? uuid = null;
        string name;
        if (protocolVersion == 766)
        {
            uuid = reader.ReadUUID();
            name = reader.ReadString();
        }
        else
        {
            name = reader.ReadString();
        }
        double value = reader.ReadDouble();
        string operation = ReadOperation(reader.ReadVarInt());
        string slot = ReadAttributeSlot(reader.ReadVarInt(), protocolVersion);
        return new AttributeModifierEntry(typeId, uuid, name, value, operation, slot);
    }

    private static AttributeModifierDisplay ReadAttributeModifierDisplay(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string type = ReadDisplayType(reader.ReadVarInt());
        NbtTag? component = null;
        if (type == "override")
        {
            component = reader.ReadAnonymousNbtTag(protocolVersion);
        }
        return new AttributeModifierDisplay(type, component);
    }

    private static void WriteAttributeModifiers(ref MinecraftPrimitiveWriter writer, AttributeModifiers data, int protocolVersion)
    {
        WriteArray(ref writer, data.Attributes, (ref MinecraftPrimitiveWriter w, AttributeModifierEntry entry) =>
        {
            WriteAttributeModifierEntry(ref w, entry, protocolVersion);
        });

        if (protocolVersion <= 770)
        {
            writer.WriteBoolean(data.ShowTooltip ?? false);
        }
        else
        {
            WriteAttributeModifierDisplay(ref writer, data.Display, protocolVersion);
        }
    }

    private static void WriteAttributeModifierEntry(ref MinecraftPrimitiveWriter writer, AttributeModifierEntry entry, int protocolVersion)
    {
        writer.WriteVarInt(entry.TypeId);
        if (protocolVersion == 766)
        {
            writer.WriteUUID(entry.Uuid ?? Guid.Empty);
            writer.WriteString(entry.Name);
        }
        else
        {
            writer.WriteString(entry.Name);
        }
        writer.WriteDouble(entry.Value);
        writer.WriteVarInt(WriteOperation(entry.Operation));
        writer.WriteVarInt(WriteAttributeSlot(entry.Slot, protocolVersion));
    }

    private static void WriteAttributeModifierDisplay(ref MinecraftPrimitiveWriter writer, AttributeModifierDisplay? display,
        int protocolVersion)
    {
        if (display is null)
        {
            writer.WriteVarInt(WriteDisplayType("default"));
            return;
        }

        writer.WriteVarInt(WriteDisplayType(display.Type));
        if (display.Type == "override")
        {
            writer.WriteAnonymousNbtTag(display.Component ?? throw new InvalidOperationException("display component missing"), protocolVersion);
        }
    }

    private static CustomModelData ReadCustomModelData(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (protocolVersion <= 767)
        {
            return new CustomModelData(reader.ReadVarInt(), null, null, null, null);
        }

        float[] floats = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadFloat());
        bool[] flags = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadBoolean());
        string[] strings = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadString());
        int[] colors = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSignedInt());
        return new CustomModelData(null, floats, flags, strings, colors);
    }

    private static void WriteCustomModelData(ref MinecraftPrimitiveWriter writer, CustomModelData data, int protocolVersion)
    {
        if (protocolVersion <= 767)
        {
            writer.WriteVarInt(data.LegacyValue ?? 0);
            return;
        }

        WriteArray(ref writer, data.Floats ?? Array.Empty<float>(), (ref MinecraftPrimitiveWriter w, float value) => w.WriteFloat(value));
        WriteArray(ref writer, data.Flags ?? Array.Empty<bool>(), (ref MinecraftPrimitiveWriter w, bool value) => w.WriteBoolean(value));
        WriteArray(ref writer, data.Strings ?? Array.Empty<string>(), (ref MinecraftPrimitiveWriter w, string value) => w.WriteString(value));
        WriteArray(ref writer, data.Colors ?? Array.Empty<int>(), (ref MinecraftPrimitiveWriter w, int value) => w.WriteSignedInt(value));
    }

    private static NbtTag?[] ReadLore(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => protocolVersion <= 769
            ? r.ReadAnonOptionalNbtTag(protocolVersion)
            : r.ReadAnonymousNbtTag(protocolVersion));
    }

    private static void WriteLore(ref MinecraftPrimitiveWriter writer, IReadOnlyList<NbtTag?> lines, int protocolVersion)
    {
        WriteArray(ref writer, lines, (ref MinecraftPrimitiveWriter w, NbtTag? line) =>
        {
            if (protocolVersion <= 769)
            {
                w.WriteAnonOptionalNbtTag(line, protocolVersion);
            }
            else
            {
                w.WriteAnonymousNbtTag(line ?? throw new InvalidOperationException("lore entry missing"), protocolVersion);
            }
        });
    }

    private static EnchantmentEntry[] ReadEnchantments(ref MinecraftPrimitiveReader reader)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => new EnchantmentEntry(r.ReadVarInt(), r.ReadVarInt()));
    }

    private static void WriteEnchantments(ref MinecraftPrimitiveWriter writer, IReadOnlyList<EnchantmentEntry> entries)
    {
        WriteArray(ref writer, entries, (ref MinecraftPrimitiveWriter w, EnchantmentEntry entry) =>
        {
            w.WriteVarInt(entry.Id);
            w.WriteVarInt(entry.Level);
        });
    }

    private static ItemBlockPredicate[] ReadItemBlockPredicates(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemBlockPredicate(protocolVersion));
    }

    private static void WriteItemBlockPredicates(ref MinecraftPrimitiveWriter writer, IReadOnlyList<ItemBlockPredicate> predicates,
        int protocolVersion)
    {
        WriteArray(ref writer, predicates, (ref MinecraftPrimitiveWriter w, ItemBlockPredicate predicate) =>
        {
            w.WriteItemBlockPredicate(predicate, protocolVersion);
        });
    }

    private static FoodData ReadFood(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int nutrition = reader.ReadVarInt();
        float saturation = reader.ReadFloat();
        bool canAlwaysEat = reader.ReadBoolean();
        float? secondsToEat = null;
        Slot? usingConvertsTo = null;
        FoodEffect[]? effects = null;
        if (protocolVersion <= 767)
        {
            secondsToEat = reader.ReadFloat();
            usingConvertsTo = reader.ReadSlot(protocolVersion);
            effects = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => new FoodEffect(r.ReadVarInt(), r.ReadFloat()));
        }
        return new FoodData(nutrition, saturation, canAlwaysEat, secondsToEat, usingConvertsTo, effects);
    }

    private static void WriteFood(ref MinecraftPrimitiveWriter writer, FoodData data, int protocolVersion)
    {
        writer.WriteVarInt(data.Nutrition);
        writer.WriteFloat(data.SaturationModifier);
        writer.WriteBoolean(data.CanAlwaysEat);
        if (protocolVersion <= 767)
        {
            writer.WriteFloat(data.SecondsToEat ?? 0f);
            writer.WriteSlot(data.UsingConvertsTo ?? new Slot(), protocolVersion);
            WriteArray(ref writer, data.Effects ?? Array.Empty<FoodEffect>(), (ref MinecraftPrimitiveWriter w, FoodEffect effect) =>
            {
                w.WriteVarInt(effect.Effect);
                w.WriteFloat(effect.Probability);
            });
        }
    }

    private static ToolData ReadTool(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ToolRule[] rules = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            var blocks = r.ReadIDSet(protocolVersion);
            float? speed = ReadOptionalFloat(ref r);
            bool? correct = ReadOptionalBool(ref r);
            return new ToolRule(blocks, speed, correct);
        });
        float defaultSpeed = reader.ReadFloat();
        int damagePerBlock = reader.ReadVarInt();
        bool? canDestroyBlocks = null;
        if (protocolVersion >= 770)
        {
            canDestroyBlocks = reader.ReadBoolean();
        }
        return new ToolData(rules, defaultSpeed, damagePerBlock, canDestroyBlocks);
    }

    private static void WriteTool(ref MinecraftPrimitiveWriter writer, ToolData data, int protocolVersion)
    {
        WriteArray(ref writer, data.Rules, (ref MinecraftPrimitiveWriter w, ToolRule rule) =>
        {
            w.WriteIDSet(rule.Blocks, protocolVersion);
            WriteOptionalFloat(ref w, rule.Speed);
            WriteOptionalBool(ref w, rule.CorrectDropForBlocks);
        });
        writer.WriteFloat(data.DefaultMiningSpeed);
        writer.WriteVarInt(data.DamagePerBlock);
        if (protocolVersion >= 770)
        {
            writer.WriteBoolean(data.CanDestroyBlocksInCreative ?? false);
        }
    }

    private static PotionContentsData ReadPotionContents(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int? potionId = ReadOptionalVarInt(ref reader);
        int? customColor = ReadOptionalSignedInt(ref reader);
        ItemPotionEffect[] effects = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemPotionEffect(protocolVersion));
        string? customName = ReadOptionalString(ref reader);
        return new PotionContentsData(potionId, customColor, effects, customName);
    }

    private static void WritePotionContents(ref MinecraftPrimitiveWriter writer, PotionContentsData data, int protocolVersion)
    {
        WriteOptionalVarInt(ref writer, data.PotionId);
        WriteOptionalSignedInt(ref writer, data.CustomColor);
        WriteArray(ref writer, data.CustomEffects,
            (ref MinecraftPrimitiveWriter w, ItemPotionEffect effect) => w.WriteItemPotionEffect(effect, protocolVersion));
        WriteOptionalString(ref writer, data.CustomNameText);
    }

    private static SuspiciousStewEffect[] ReadSuspiciousStewEffects(ref MinecraftPrimitiveReader reader)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => new SuspiciousStewEffect(r.ReadVarInt(), r.ReadVarInt()));
    }

    private static void WriteSuspiciousStewEffects(ref MinecraftPrimitiveWriter writer, IReadOnlyList<SuspiciousStewEffect> effects)
    {
        WriteArray(ref writer, effects, (ref MinecraftPrimitiveWriter w, SuspiciousStewEffect effect) =>
        {
            w.WriteVarInt(effect.Effect);
            w.WriteVarInt(effect.Duration);
        });
    }

    private static ItemBookPage[] ReadItemBookPages(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemBookPage(protocolVersion));
    }

    private static void WriteItemBookPages(ref MinecraftPrimitiveWriter writer, IReadOnlyList<ItemBookPage> pages, int protocolVersion)
    {
        WriteArray(ref writer, pages, (ref MinecraftPrimitiveWriter w, ItemBookPage page) => w.WriteItemBookPage(page, protocolVersion));
    }

    private static WrittenBookContentData ReadWrittenBookContent(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string rawTitle = reader.ReadString();
        string? filteredTitle = ReadOptionalString(ref reader);
        string author = reader.ReadString();
        int generation = reader.ReadVarInt();
        ItemWrittenBookPage[] pages = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemWrittenBookPage(protocolVersion));
        bool resolved = reader.ReadBoolean();
        return new WrittenBookContentData(rawTitle, filteredTitle, author, generation, pages, resolved);
    }

    private static void WriteWrittenBookContent(ref MinecraftPrimitiveWriter writer, WrittenBookContentData data, int protocolVersion)
    {
        writer.WriteString(data.RawTitle);
        WriteOptionalString(ref writer, data.FilteredTitle);
        writer.WriteString(data.Author);
        writer.WriteVarInt(data.Generation);
        WriteArray(ref writer, data.Pages,
            (ref MinecraftPrimitiveWriter w, ItemWrittenBookPage page) => w.WriteItemWrittenBookPage(page, protocolVersion));
        writer.WriteBoolean(data.Resolved);
    }

    private static TrimData ReadTrim(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var material = reader.ReadRegistryEntryHolder<ArmorTrimMaterial>(protocolVersion);
        var pattern = reader.ReadRegistryEntryHolder<ArmorTrimPattern>(protocolVersion);
        bool? showInTooltip = null;
        if (protocolVersion <= 769)
        {
            showInTooltip = reader.ReadBoolean();
        }
        return new TrimData(material, pattern, showInTooltip);
    }

    private static void WriteTrim(ref MinecraftPrimitiveWriter writer, TrimData data, int protocolVersion)
    {
        writer.WriteRegistryEntryHolder(data.Material, protocolVersion);
        writer.WriteRegistryEntryHolder(data.Pattern, protocolVersion);
        if (protocolVersion <= 769)
        {
            writer.WriteBoolean(data.ShowInTooltip ?? false);
        }
    }

    private static InstrumentComponentData ReadInstrument(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (protocolVersion <= 769)
        {
            var holder = reader.ReadRegistryEntryHolder<InstrumentData>(protocolVersion);
            return new InstrumentComponentData(holder, null);
        }

        bool hasHolder = reader.ReadBoolean();
        if (hasHolder)
        {
            return new InstrumentComponentData(reader.ReadRegistryEntryHolder<InstrumentData>(protocolVersion), null);
        }

        return new InstrumentComponentData(null, reader.ReadString());
    }

    private static void WriteInstrument(ref MinecraftPrimitiveWriter writer, InstrumentComponentData data, int protocolVersion)
    {
        if (protocolVersion <= 769)
        {
            writer.WriteRegistryEntryHolder(data.Holder ?? throw new InvalidOperationException("instrument holder missing"),
                protocolVersion);
            return;
        }

        bool hasHolder = data.Holder is not null;
        writer.WriteBoolean(hasHolder);
        if (hasHolder)
        {
            writer.WriteRegistryEntryHolder<InstrumentData>(
                data.Holder ?? throw new InvalidOperationException("instrument holder missing"),
                protocolVersion);
        }
        else
        {
            writer.WriteString(data.InlineId ?? string.Empty);
        }
    }

    private static JukeboxPlayableData ReadJukeboxPlayable(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        bool hasHolder = reader.ReadBoolean();
        RegistryEntryHolder<JukeboxSongData>? holder = null;
        string? song = null;
        if (hasHolder)
        {
            holder = reader.ReadRegistryEntryHolder<JukeboxSongData>(protocolVersion);
        }
        else
        {
            song = reader.ReadString();
        }

        bool? showInTooltip = null;
        if (protocolVersion <= 769)
        {
            showInTooltip = reader.ReadBoolean();
        }
        return new JukeboxPlayableData(holder, song, showInTooltip);
    }

    private static void WriteJukeboxPlayable(ref MinecraftPrimitiveWriter writer, JukeboxPlayableData data, int protocolVersion)
    {
        bool hasHolder = data.Holder is not null;
        writer.WriteBoolean(hasHolder);
        if (hasHolder)
        {
            writer.WriteRegistryEntryHolder<JukeboxSongData>(
                data.Holder ?? throw new InvalidOperationException("jukebox song holder missing"),
                protocolVersion);
        }
        else
        {
            writer.WriteString(data.Song ?? string.Empty);
        }

        if (protocolVersion <= 769)
        {
            writer.WriteBoolean(data.ShowInTooltip ?? false);
        }
    }

    private static LodestoneTrackerData ReadLodestoneTracker(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        LodestonePosition? position = null;
        if (reader.ReadBoolean())
        {
            string dimension = reader.ReadString();
            Position pos = reader.ReadPosition(protocolVersion);
            position = new LodestonePosition(dimension, pos);
        }
        bool tracked = reader.ReadBoolean();
        return new LodestoneTrackerData(position, tracked);
    }

    private static void WriteLodestoneTracker(ref MinecraftPrimitiveWriter writer, LodestoneTrackerData data, int protocolVersion)
    {
        if (data.GlobalPosition is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteString(data.GlobalPosition.Dimension);
            writer.WritePosition(data.GlobalPosition.Position, protocolVersion);
        }
        writer.WriteBoolean(data.Tracked);
    }

    private static FireworksData ReadFireworks(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int flightDuration = reader.ReadVarInt();
        ItemFireworkExplosion[] explosions = ReadArray(ref reader,
            (ref MinecraftPrimitiveReader r) => r.ReadItemFireworkExplosion(protocolVersion));
        return new FireworksData(flightDuration, explosions);
    }

    private static void WriteFireworks(ref MinecraftPrimitiveWriter writer, FireworksData data, int protocolVersion)
    {
        writer.WriteVarInt(data.FlightDuration);
        WriteArray(ref writer, data.Explosions,
            (ref MinecraftPrimitiveWriter w, ItemFireworkExplosion explosion) => w.WriteItemFireworkExplosion(explosion, protocolVersion));
    }

    private static ProfileData ReadProfile(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string? name = ReadOptionalString(ref reader);
        Guid? uuid = ReadOptionalUuid(ref reader);
        ProfileProperty[] properties = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            string propName = r.ReadString();
            string value = r.ReadString();
            string? signature = ReadOptionalString(ref r);
            return new ProfileProperty(propName, value, signature);
        });
        return new ProfileData(name, uuid, properties);
    }

    private static void WriteProfile(ref MinecraftPrimitiveWriter writer, ProfileData data, int protocolVersion)
    {
        WriteOptionalString(ref writer, data.Name);
        WriteOptionalUuid(ref writer, data.Uuid);
        WriteArray(ref writer, data.Properties, (ref MinecraftPrimitiveWriter w, ProfileProperty property) =>
        {
            w.WriteString(property.Name);
            w.WriteString(property.Value);
            WriteOptionalString(ref w, property.Signature);
        });
    }

    private static BannerPatternLayer[] ReadBannerPatterns(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadBannerPatternLayer(protocolVersion));
    }

    private static void WriteBannerPatterns(ref MinecraftPrimitiveWriter writer, IReadOnlyList<BannerPatternLayer> layers, int protocolVersion)
    {
        WriteArray(ref writer, layers, (ref MinecraftPrimitiveWriter w, BannerPatternLayer layer) => w.WriteBannerPatternLayer(layer, protocolVersion));
    }

    private static int[] ReadVarIntArray(ref MinecraftPrimitiveReader reader)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadVarInt());
    }

    private static void WriteVarIntArray(ref MinecraftPrimitiveWriter writer, IReadOnlyList<int> values)
    {
        WriteArray(ref writer, values, (ref MinecraftPrimitiveWriter w, int value) => w.WriteVarInt(value));
    }

    private static BlockStateProperty[] ReadBlockState(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            string name = r.ReadString();
            string value = r.ReadString();
            return new BlockStateProperty(name, value);
        });
    }

    private static void WriteBlockState(ref MinecraftPrimitiveWriter writer, IReadOnlyList<BlockStateProperty> properties,
        int protocolVersion)
    {
        WriteArray(ref writer, properties, (ref MinecraftPrimitiveWriter w, BlockStateProperty property) =>
        {
            w.WriteString(property.Name);
            w.WriteString(property.Value);
        });
    }

    private static BeeData[] ReadBees(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            var nbt = r.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("bee nbt missing");
            int ticks = r.ReadVarInt();
            int minTicks = r.ReadVarInt();
            return new BeeData(nbt, ticks, minTicks);
        });
    }

    private static void WriteBees(ref MinecraftPrimitiveWriter writer, IReadOnlyList<BeeData> bees, int protocolVersion)
    {
        WriteArray(ref writer, bees, (ref MinecraftPrimitiveWriter w, BeeData bee) =>
        {
            w.WriteAnonymousNbtTag(bee.NbtData, protocolVersion);
            w.WriteVarInt(bee.TicksInHive);
            w.WriteVarInt(bee.MinTicksInHive);
        });
    }

    private static ConsumableData ReadConsumable(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        float seconds = reader.ReadFloat();
        string animation = ReadConsumableAnimation(reader.ReadVarInt(), protocolVersion);
        var sound = reader.ReadItemSoundHolder(protocolVersion);
        bool makesParticles = reader.ReadBoolean();
        ItemConsumeEffect[] effects = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemConsumeEffect(protocolVersion));
        return new ConsumableData(seconds, animation, sound, makesParticles, effects);
    }

    private static void WriteConsumable(ref MinecraftPrimitiveWriter writer, ConsumableData data, int protocolVersion)
    {
        writer.WriteFloat(data.ConsumeSeconds);
        writer.WriteVarInt(WriteConsumableAnimation(data.Animation, protocolVersion));
        writer.WriteItemSoundHolder(data.Sound, protocolVersion);
        writer.WriteBoolean(data.MakesParticles);
        WriteArray(ref writer, data.Effects,
            (ref MinecraftPrimitiveWriter w, ItemConsumeEffect effect) => w.WriteItemConsumeEffect(effect, protocolVersion));
    }

    private static UseCooldownData ReadUseCooldown(ref MinecraftPrimitiveReader reader)
    {
        float seconds = reader.ReadFloat();
        string? cooldownGroup = ReadOptionalString(ref reader);
        return new UseCooldownData(seconds, cooldownGroup);
    }

    private static void WriteUseCooldown(ref MinecraftPrimitiveWriter writer, UseCooldownData data)
    {
        writer.WriteFloat(data.Seconds);
        WriteOptionalString(ref writer, data.CooldownGroup);
    }

    private static EquippableData ReadEquippable(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string slot = ReadEquippableSlot(reader.ReadVarInt(), protocolVersion);
        ItemSoundHolder sound = reader.ReadItemSoundHolder(protocolVersion);
        string? model = ReadOptionalString(ref reader);
        string? cameraOverlay = ReadOptionalString(ref reader);
        IDSet? allowedEntities = ReadOptionalIdSet(ref reader, protocolVersion);
        bool dispensable = reader.ReadBoolean();
        bool swappable = reader.ReadBoolean();
        bool damageable = reader.ReadBoolean();
        bool? equipOnInteract = null;
        bool? shearable = null;
        ItemSoundHolder? shearingSound = null;
        if (protocolVersion >= 770)
        {
            equipOnInteract = reader.ReadBoolean();
        }
        if (protocolVersion >= 771)
        {
            shearable = reader.ReadBoolean();
            shearingSound = reader.ReadItemSoundHolder(protocolVersion);
        }
        return new EquippableData(slot, sound, model, cameraOverlay, allowedEntities, dispensable, swappable, damageable, equipOnInteract,
            shearable, shearingSound);
    }

    private static void WriteEquippable(ref MinecraftPrimitiveWriter writer, EquippableData data, int protocolVersion)
    {
        writer.WriteVarInt(WriteEquippableSlot(data.Slot, protocolVersion));
        writer.WriteItemSoundHolder(data.Sound, protocolVersion);
        WriteOptionalString(ref writer, data.Model);
        WriteOptionalString(ref writer, data.CameraOverlay);
        WriteOptionalIdSet(ref writer, data.AllowedEntities, protocolVersion);
        writer.WriteBoolean(data.Dispensable);
        writer.WriteBoolean(data.Swappable);
        writer.WriteBoolean(data.Damageable);
        if (protocolVersion >= 770)
        {
            writer.WriteBoolean(data.EquipOnInteract ?? false);
        }
        if (protocolVersion >= 771)
        {
            writer.WriteBoolean(data.Shearable ?? false);
            writer.WriteItemSoundHolder(data.ShearingSound
                ?? throw new InvalidOperationException("Equippable.shearingSound missing."), protocolVersion);
        }
    }

    private static ItemConsumeEffect[] ReadItemConsumeEffects(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadItemConsumeEffect(protocolVersion));
    }

    private static void WriteItemConsumeEffects(ref MinecraftPrimitiveWriter writer, IReadOnlyList<ItemConsumeEffect> effects,
        int protocolVersion)
    {
        WriteArray(ref writer, effects,
            (ref MinecraftPrimitiveWriter w, ItemConsumeEffect effect) => w.WriteItemConsumeEffect(effect, protocolVersion));
    }

    private static TooltipDisplayData ReadTooltipDisplay(ref MinecraftPrimitiveReader reader)
    {
        bool hideTooltip = reader.ReadBoolean();
        int[] hidden = ReadVarIntArray(ref reader);
        return new TooltipDisplayData(hideTooltip, hidden);
    }

    private static void WriteTooltipDisplay(ref MinecraftPrimitiveWriter writer, TooltipDisplayData data)
    {
        writer.WriteBoolean(data.HideTooltipFlag);
        WriteVarIntArray(ref writer, data.HiddenComponents);
    }

    private static WeaponData ReadWeapon(ref MinecraftPrimitiveReader reader)
    {
        int itemDamage = reader.ReadVarInt();
        float disableBlocking = reader.ReadFloat();
        return new WeaponData(itemDamage, disableBlocking);
    }

    private static void WriteWeapon(ref MinecraftPrimitiveWriter writer, WeaponData data)
    {
        writer.WriteVarInt(data.ItemDamagePerAttack);
        writer.WriteFloat(data.DisableBlockingForSeconds);
    }

    private static BlocksAttacksData ReadBlocksAttacks(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        float blockDelay = reader.ReadFloat();
        float disableCooldown = reader.ReadFloat();
        DamageReduction[] reductions = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            float angle = r.ReadFloat();
            IDSet? type = ReadOptionalIdSet(ref r, protocolVersion);
            float baseValue = r.ReadFloat();
            float factor = r.ReadFloat();
            return new DamageReduction(angle, type, baseValue, factor);
        });
        ItemDamageData itemDamage = new(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
        string? bypassedBy = ReadOptionalString(ref reader);
        ItemSoundHolder? blockSound = ReadOptionalItemSoundHolder(ref reader, protocolVersion);
        ItemSoundHolder? disableSound = ReadOptionalItemSoundHolder(ref reader, protocolVersion);
        return new BlocksAttacksData(blockDelay, disableCooldown, reductions, itemDamage, bypassedBy, blockSound, disableSound);
    }

    private static void WriteBlocksAttacks(ref MinecraftPrimitiveWriter writer, BlocksAttacksData data, int protocolVersion)
    {
        writer.WriteFloat(data.BlockDelaySeconds);
        writer.WriteFloat(data.DisableCooldownScale);
        WriteArray(ref writer, data.DamageReductions, (ref MinecraftPrimitiveWriter w, DamageReduction reduction) =>
        {
            w.WriteFloat(reduction.HorizontalBlockingAngle);
            WriteOptionalIdSet(ref w, reduction.Type, protocolVersion);
            w.WriteFloat(reduction.Base);
            w.WriteFloat(reduction.Factor);
        });
        writer.WriteFloat(data.ItemDamage.Threshold);
        writer.WriteFloat(data.ItemDamage.Base);
        writer.WriteFloat(data.ItemDamage.Factor);
        WriteOptionalString(ref writer, data.BypassedBy);
        WriteOptionalItemSoundHolder(ref writer, data.BlockSound, protocolVersion);
        WriteOptionalItemSoundHolder(ref writer, data.DisableSound, protocolVersion);
    }

    private static ProvidesTrimMaterialData ReadProvidesTrimMaterial(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        bool hasHolder = reader.ReadBoolean();
        if (hasHolder)
        {
            var holder = reader.ReadRegistryEntryHolder<ArmorTrimMaterial>(protocolVersion);
            return new ProvidesTrimMaterialData(holder, null);
        }

        return new ProvidesTrimMaterialData(null, reader.ReadString());
    }

    private static void WriteProvidesTrimMaterial(ref MinecraftPrimitiveWriter writer, ProvidesTrimMaterialData data, int protocolVersion)
    {
        bool hasHolder = data.Holder is not null;
        writer.WriteBoolean(hasHolder);
        if (hasHolder)
        {
            writer.WriteRegistryEntryHolder<ArmorTrimMaterial>(
                data.Holder ?? throw new InvalidOperationException("trim material holder missing"),
                protocolVersion);
        }
        else
        {
            writer.WriteString(data.MaterialId ?? string.Empty);
        }
    }

    private static Slot[] ReadSlotArray(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSlot(protocolVersion));
    }

    private static void WriteSlotArray(ref MinecraftPrimitiveWriter writer, IReadOnlyList<Slot> slots, int protocolVersion)
    {
        WriteArray(ref writer, slots, (ref MinecraftPrimitiveWriter w, Slot slot) => w.WriteSlot(slot, protocolVersion));
    }

    private static string ReadRarity(int id)
    {
        return id switch
        {
            0 => "common",
            1 => "uncommon",
            2 => "rare",
            3 => "epic",
            _ => throw new InvalidOperationException($"Unknown rarity id {id}")
        };
    }

    private static int WriteRarity(string value)
    {
        return value switch
        {
            "common" => 0,
            "uncommon" => 1,
            "rare" => 2,
            "epic" => 3,
            _ => throw new InvalidOperationException($"Unknown rarity value {value}")
        };
    }

    private static string ReadOperation(int id)
    {
        return id switch
        {
            0 => "add",
            1 => "multiply_base",
            2 => "multiply_total",
            _ => throw new InvalidOperationException($"Unknown operation id {id}")
        };
    }

    private static int WriteOperation(string value)
    {
        return value switch
        {
            "add" => 0,
            "multiply_base" => 1,
            "multiply_total" => 2,
            _ => throw new InvalidOperationException($"Unknown operation value {value}")
        };
    }

    private static string ReadAttributeSlot(int id, int protocolVersion)
    {
        return (protocolVersion >= 770 ? id : id) switch
        {
            0 => "any",
            1 => "main_hand",
            2 => "off_hand",
            3 => "hand",
            4 => "feet",
            5 => "legs",
            6 => "chest",
            7 => "head",
            8 => "armor",
            9 => "body",
            10 when protocolVersion >= 770 => "saddle",
            _ => throw new InvalidOperationException($"Unknown attribute slot id {id}")
        };
    }

    private static int WriteAttributeSlot(string value, int protocolVersion)
    {
        return value switch
        {
            "any" => 0,
            "main_hand" => 1,
            "off_hand" => 2,
            "hand" => 3,
            "feet" => 4,
            "legs" => 5,
            "chest" => 6,
            "head" => 7,
            "armor" => 8,
            "body" => 9,
            "saddle" when protocolVersion >= 770 => 10,
            _ => throw new InvalidOperationException($"Unknown attribute slot value {value}")
        };
    }

    private static string ReadDisplayType(int id)
    {
        return id switch
        {
            0 => "default",
            1 => "hidden",
            2 => "override",
            _ => throw new InvalidOperationException($"Unknown display type id {id}")
        };
    }

    private static int WriteDisplayType(string value)
    {
        return value switch
        {
            "default" => 0,
            "hidden" => 1,
            "override" => 2,
            _ => throw new InvalidOperationException($"Unknown display type value {value}")
        };
    }

    private static string ReadConsumableAnimation(int id, int protocolVersion)
    {
        return id switch
        {
            0 => "none",
            1 => "eat",
            2 => "drink",
            3 => "block",
            4 => "bow",
            5 => "spear",
            6 => "crossbow",
            7 => "spyglass",
            8 => "toot_horn",
            9 => "brush",
            10 when protocolVersion >= 770 => "bundle",
            _ => throw new InvalidOperationException($"Unknown consumable animation id {id}")
        };
    }

    private static int WriteConsumableAnimation(string value, int protocolVersion)
    {
        return value switch
        {
            "none" => 0,
            "eat" => 1,
            "drink" => 2,
            "block" => 3,
            "bow" => 4,
            "spear" => 5,
            "crossbow" => 6,
            "spyglass" => 7,
            "toot_horn" => 8,
            "brush" => 9,
            "bundle" when protocolVersion >= 770 => 10,
            _ => throw new InvalidOperationException($"Unknown consumable animation value {value}")
        };
    }

    private static string ReadEquippableSlot(int id, int protocolVersion)
    {
        return id switch
        {
            0 => "main_hand",
            1 => "off_hand",
            2 => "feet",
            3 => "legs",
            4 => "chest",
            5 => "head",
            6 => "body",
            7 when protocolVersion >= 770 => "saddle",
            _ => throw new InvalidOperationException($"Unknown equippable slot id {id}")
        };
    }

    private static int WriteEquippableSlot(string value, int protocolVersion)
    {
        return value switch
        {
            "main_hand" => 0,
            "off_hand" => 1,
            "feet" => 2,
            "legs" => 3,
            "chest" => 4,
            "head" => 5,
            "body" => 6,
            "saddle" when protocolVersion >= 770 => 7,
            _ => throw new InvalidOperationException($"Unknown equippable slot value {value}")
        };
    }

    private delegate T ReadElementDelegate<T>(ref MinecraftPrimitiveReader reader);
    private delegate void WriteElementDelegate<T>(ref MinecraftPrimitiveWriter writer, T value);

    private static T[] ReadArray<T>(ref MinecraftPrimitiveReader reader, ReadElementDelegate<T> read)
    {
        int length = reader.ReadVarInt();
        if (length == 0)
        {
            return Array.Empty<T>();
        }

        var result = new T[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = read(ref reader);
        }

        return result;
    }

    private static void WriteArray<T>(ref MinecraftPrimitiveWriter writer, IReadOnlyList<T> values,
        WriteElementDelegate<T> write)
    {
        writer.WriteVarInt(values.Count);
        for (int i = 0; i < values.Count; i++)
        {
            write(ref writer, values[i]);
        }
    }

    private static int? ReadOptionalVarInt(ref MinecraftPrimitiveReader reader)
        => reader.ReadBoolean() ? reader.ReadVarInt() : null;

    private static int? ReadOptionalSignedInt(ref MinecraftPrimitiveReader reader)
        => reader.ReadBoolean() ? reader.ReadSignedInt() : null;

    private static float? ReadOptionalFloat(ref MinecraftPrimitiveReader reader)
        => reader.ReadBoolean() ? reader.ReadFloat() : null;

    private static bool? ReadOptionalBool(ref MinecraftPrimitiveReader reader)
        => reader.ReadBoolean() ? reader.ReadBoolean() : null;

    private static string? ReadOptionalString(ref MinecraftPrimitiveReader reader)
        => reader.ReadBoolean() ? reader.ReadString() : null;

    private static Guid? ReadOptionalUuid(ref MinecraftPrimitiveReader reader)
        => reader.ReadBoolean() ? reader.ReadUUID() : null;

    private static IDSet? ReadOptionalIdSet(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => reader.ReadBoolean() ? reader.ReadIDSet(protocolVersion) : null;

    private static ItemSoundHolder? ReadOptionalItemSoundHolder(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => reader.ReadBoolean() ? reader.ReadItemSoundHolder(protocolVersion) : null;

    private static void WriteOptionalVarInt(ref MinecraftPrimitiveWriter writer, int? value)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteVarInt(value.Value);
        }
    }

    private static void WriteOptionalSignedInt(ref MinecraftPrimitiveWriter writer, int? value)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteSignedInt(value.Value);
        }
    }

    private static void WriteOptionalFloat(ref MinecraftPrimitiveWriter writer, float? value)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteFloat(value.Value);
        }
    }

    private static void WriteOptionalBool(ref MinecraftPrimitiveWriter writer, bool? value)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteBoolean(value.Value);
        }
    }

    private static void WriteOptionalString(ref MinecraftPrimitiveWriter writer, string? value)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteString(value);
        }
    }

    private static void WriteOptionalUuid(ref MinecraftPrimitiveWriter writer, Guid? value)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteUUID(value.Value);
        }
    }

    private static void WriteOptionalIdSet(ref MinecraftPrimitiveWriter writer, IDSet? value, int protocolVersion)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteIDSet(value, protocolVersion);
        }
    }

    private static void WriteOptionalItemSoundHolder(ref MinecraftPrimitiveWriter writer, ItemSoundHolder? value,
        int protocolVersion)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteItemSoundHolder(value, protocolVersion);
        }
    }
}
