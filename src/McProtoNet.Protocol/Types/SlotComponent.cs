using Dunet;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[Union]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public partial record SlotComponent
{
    public sealed record AttributeModifiers(AttributeModifierEntry[] Attributes, bool? ShowTooltip, AttributeModifierDisplay? Display) : SlotComponent;
    public sealed record AxolotlVariant(int Value) : SlotComponent;
    public sealed record BannerPatterns(BannerPatternLayer[] Layers) : SlotComponent;
    public sealed record BaseColor(int Value) : SlotComponent;
    public sealed record Bees(BeeData[] Bees) : SlotComponent;
    public sealed record BlockEntityData(NbtTag Data) : SlotComponent;
    public sealed record BlockState(BlockStateProperty[] Properties) : SlotComponent;
    public sealed record BlocksAttacks(BlocksAttacksData Data) : SlotComponent;
    public sealed record BreakSound(ItemSoundHolder Sound) : SlotComponent;
    public sealed record BucketEntityData(NbtTag Data) : SlotComponent;
    public sealed record BundleContents(Slot[] Contents) : SlotComponent;
    public sealed record CanBreak(ItemBlockPredicate[] Predicates, bool? ShowTooltip) : SlotComponent;
    public sealed record CanPlaceOn(ItemBlockPredicate[] Predicates, bool? ShowTooltip) : SlotComponent;
    public sealed record CatCollar(int Value) : SlotComponent;
    public sealed record CatVariant(int Value) : SlotComponent;
    public sealed record ChargedProjectiles(Slot[] Projectiles) : SlotComponent;
    public sealed record ChickenVariant(RegistryEntryHolder<string> Variant) : SlotComponent;
    public sealed record Consumable(ConsumableData Data) : SlotComponent;
    public sealed record Container(Slot[] Contents) : SlotComponent;
    public sealed record ContainerLoot(NbtTag Data) : SlotComponent;
    public sealed record CowVariant(int Value) : SlotComponent;
    public sealed record CreativeSlotLock() : SlotComponent;
    public sealed record CustomData(NbtTag Data) : SlotComponent;
    public sealed record CustomModelData(int? LegacyValue, float[]? Floats, bool[]? Flags, string[]? Strings, int[]? Colors) : SlotComponent;
    public sealed record CustomName(NbtTag Data) : SlotComponent;
    public sealed record Damage(int Value) : SlotComponent;
    public sealed record DamageResistant(string Value) : SlotComponent;
    public sealed record DeathProtection(ItemConsumeEffect[] Effects) : SlotComponent;
    public sealed record DebugStickState(NbtTag Data) : SlotComponent;
    public sealed record DyedColor(int Color, bool? ShowTooltip) : SlotComponent;
    public sealed record Enchantable(int Value) : SlotComponent;
    public sealed record EnchantmentGlintOverride(bool Value) : SlotComponent;
    public sealed record Enchantments(EnchantmentEntry[] Enchantments, bool? ShowTooltip) : SlotComponent;
    public sealed record EntityData(NbtTag Data) : SlotComponent;
    public sealed record Equippable(EquippableData Data) : SlotComponent;
    public sealed record FireResistant() : SlotComponent;
    public sealed record FireworkExplosion(ItemFireworkExplosion Explosion) : SlotComponent;
    public sealed record Fireworks(FireworksData Data) : SlotComponent;
    public sealed record Food(FoodData Data) : SlotComponent;
    public sealed record FoxVariant(int Value) : SlotComponent;
    public sealed record FrogVariant(int Value) : SlotComponent;
    public sealed record Glider() : SlotComponent;
    public sealed record HideAdditionalTooltip() : SlotComponent;
    public sealed record HideTooltip() : SlotComponent;
    public sealed record HorseVariant(int Value) : SlotComponent;
    public sealed record Instrument(InstrumentComponentData Data) : SlotComponent;
    public sealed record IntangibleProjectile(NbtTag? Data) : SlotComponent;
    public sealed record ItemModel(string Model) : SlotComponent;
    public sealed record ItemName(NbtTag Data) : SlotComponent;
    public sealed record JukeboxPlayable(JukeboxPlayableData Data) : SlotComponent;
    public sealed record LlamaVariant(int Value) : SlotComponent;
    public sealed record Lock(NbtTag Data) : SlotComponent;
    public sealed record LodestoneTracker(LodestoneTrackerData Data) : SlotComponent;
    public sealed record Lore(NbtTag?[] Lines) : SlotComponent;
    public sealed record MapColor(int Color) : SlotComponent;
    public sealed record MapDecorations(NbtTag Data) : SlotComponent;
    public sealed record MapId(int Value) : SlotComponent;
    public sealed record MapPostProcessing(int Value) : SlotComponent;
    public sealed record MaxDamage(int Value) : SlotComponent;
    public sealed record MaxStackSize(int Value) : SlotComponent;
    public sealed record MooshroomVariant(int Value) : SlotComponent;
    public sealed record NoteBlockSound(string Value) : SlotComponent;
    public sealed record OminousBottleAmplifier(int Value) : SlotComponent;
    public sealed record PaintingVariant(RegistryEntryHolder<EntityMetadataPaintingVariant> Variant) : SlotComponent;
    public sealed record ParrotVariant(int Value) : SlotComponent;
    public sealed record PigVariant(int Value) : SlotComponent;
    public sealed record PotDecorations(int[] Decorations) : SlotComponent;
    public sealed record PotionContents(PotionContentsData Data) : SlotComponent;
    public sealed record PotionDurationScale(float Value) : SlotComponent;
    public sealed record Profile(ProfileData Data) : SlotComponent;
    public sealed record ProvidesBannerPatterns(string Value) : SlotComponent;
    public sealed record ProvidesTrimMaterial(ProvidesTrimMaterialData Data) : SlotComponent;
    public sealed record RabbitVariant(int Value) : SlotComponent;
    public sealed record Rarity(string Value) : SlotComponent;
    public sealed record Recipes(NbtTag Data) : SlotComponent;
    public sealed record RepairCost(int Value) : SlotComponent;
    public sealed record Repairable(IDSet Items) : SlotComponent;
    public sealed record SalmonSize(int Value) : SlotComponent;
    public sealed record SheepColor(int Value) : SlotComponent;
    public sealed record ShulkerColor(int Value) : SlotComponent;
    public sealed record StoredEnchantments(EnchantmentEntry[] Enchantments, bool? ShowInTooltip) : SlotComponent;
    public sealed record SuspiciousStewEffects(SuspiciousStewEffect[] Effects) : SlotComponent;
    public sealed record Tool(ToolData Data) : SlotComponent;
    public sealed record TooltipDisplay(TooltipDisplayData Data) : SlotComponent;
    public sealed record TooltipStyle(string Value) : SlotComponent;
    public sealed record Trim(TrimData Data) : SlotComponent;
    public sealed record TropicalFishBaseColor(int Value) : SlotComponent;
    public sealed record TropicalFishPattern(int Value) : SlotComponent;
    public sealed record TropicalFishPatternColor(int Value) : SlotComponent;
    public sealed record Unbreakable(bool? Value) : SlotComponent;
    public sealed record UseCooldown(UseCooldownData Data) : SlotComponent;
    public sealed record UseRemainder(Slot Value) : SlotComponent;
    public sealed record VillagerVariant(int Value) : SlotComponent;
    public sealed record Weapon(WeaponData Data) : SlotComponent;
    public sealed record WolfCollar(int Value) : SlotComponent;
    public sealed record WolfSoundVariant(int Value) : SlotComponent;
    public sealed record WolfVariant(int Value) : SlotComponent;
    public sealed record WritableBookContent(ItemBookPage[] Pages) : SlotComponent;
    public sealed record WrittenBookContent(WrittenBookContentData Data) : SlotComponent;

    public sealed record AttributeModifierEntry(int TypeId, Guid? Uuid, string Name, double Value, string Operation, string Slot);
    public sealed record AttributeModifierDisplay(string Type, NbtTag? Component);
    public sealed record BeeData(NbtTag NbtData, int TicksInHive, int MinTicksInHive);
    public sealed record BlockStateProperty(string Name, string Value);
    public sealed record BlocksAttacksData(float BlockDelaySeconds, float DisableCooldownScale, DamageReduction[] DamageReductions,
        ItemDamageData ItemDamage, string? BypassedBy, ItemSoundHolder? BlockSound, ItemSoundHolder? DisableSound);
    public sealed record ConsumableData(float ConsumeSeconds, string Animation, ItemSoundHolder Sound, bool MakesParticles,
        ItemConsumeEffect[] Effects);
    public sealed record DamageReduction(float HorizontalBlockingAngle, IDSet? Type, float Base, float Factor);
    public sealed record EquippableData(string Slot, ItemSoundHolder Sound, string? Model, string? CameraOverlay, IDSet? AllowedEntities,
        bool Dispensable, bool Swappable, bool Damageable, bool? EquipOnInteract, bool? Shearable, ItemSoundHolder? ShearingSound);
    public sealed record EnchantmentEntry(int Id, int Level);
    public sealed record FireworksData(int FlightDuration, ItemFireworkExplosion[] Explosions);
    public sealed record FoodData(int Nutrition, float SaturationModifier, bool CanAlwaysEat, float? SecondsToEat,
        Slot? UsingConvertsTo, FoodEffect[]? Effects);
    public sealed record FoodEffect(int Effect, float Probability);
    public sealed record InstrumentComponentData(RegistryEntryHolder<InstrumentData>? Holder, string? InlineId);
    public sealed record ItemDamageData(float Threshold, float Base, float Factor);
    public sealed record JukeboxPlayableData(RegistryEntryHolder<JukeboxSongData>? Holder, string? Song, bool? ShowInTooltip);
    public sealed record LodestonePosition(string Dimension, Position Position);
    public sealed record LodestoneTrackerData(LodestonePosition? GlobalPosition, bool Tracked);
    public sealed record PotionContentsData(int? PotionId, int? CustomColor, ItemPotionEffect[] CustomEffects, string? CustomName);
    public sealed record ProfileData(string? Name, Guid? Uuid, ProfileProperty[] Properties);
    public sealed record ProfileProperty(string Name, string Value, string? Signature);
    public sealed record ProvidesTrimMaterialData(RegistryEntryHolder<ArmorTrimMaterial>? Holder, string? MaterialId);
    public sealed record SuspiciousStewEffect(int Effect, int Duration);
    public sealed record ToolData(ToolRule[] Rules, float DefaultMiningSpeed, int DamagePerBlock, bool? CanDestroyBlocksInCreative);
    public sealed record ToolRule(IDSet Blocks, float? Speed, bool? CorrectDropForBlocks);
    public sealed record TooltipDisplayData(bool HideTooltip, int[] HiddenComponents);
    public sealed record TrimData(RegistryEntryHolder<ArmorTrimMaterial> Material, RegistryEntryHolder<ArmorTrimPattern> Pattern,
        bool? ShowInTooltip);
    public sealed record UseCooldownData(float Seconds, string? CooldownGroup);
    public sealed record WeaponData(int ItemDamagePerAttack, float DisableBlockingForSeconds);
    public sealed record WrittenBookContentData(string RawTitle, string? FilteredTitle, string Author, int Generation,
        ItemWrittenBookPage[] Pages, bool Resolved);

    public static SlotComponent Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadSlotComponent(protocolVersion);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSlotComponent(this, protocolVersion);
    }
}
