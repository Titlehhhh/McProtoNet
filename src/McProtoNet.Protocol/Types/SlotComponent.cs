using Dunet;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[Union]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public partial record SlotComponent
{
    public sealed partial record AttributeModifiers(AttributeModifierEntry[] Attributes, bool? ShowTooltip, AttributeModifierDisplay? Display) : SlotComponent;
    public sealed partial record AxolotlVariant(int Value) : SlotComponent;
    public sealed partial record BannerPatterns(BannerPatternLayer[] Layers) : SlotComponent;
    public sealed partial record BaseColor(int Value) : SlotComponent;
    public sealed partial record Bees(BeeData[] BeeEntries) : SlotComponent;
    public sealed partial record BlockEntityData(NbtTag Data) : SlotComponent;
    public sealed partial record BlockState(BlockStateProperty[] Properties) : SlotComponent;
    public sealed partial record BlocksAttacks(BlocksAttacksData Data) : SlotComponent;
    public sealed partial record BreakSound(ItemSoundHolder Sound) : SlotComponent;
    public sealed partial record BucketEntityData(NbtTag Data) : SlotComponent;
    public sealed partial record BundleContents(Slot[] Contents) : SlotComponent;
    public sealed partial record CanBreak(ItemBlockPredicate[] Predicates, bool? ShowTooltip) : SlotComponent;
    public sealed partial record CanPlaceOn(ItemBlockPredicate[] Predicates, bool? ShowTooltip) : SlotComponent;
    public sealed partial record CatCollar(int Value) : SlotComponent;
    public sealed partial record CatVariant(int Value) : SlotComponent;
    public sealed partial record ChargedProjectiles(Slot[] Projectiles) : SlotComponent;
    public sealed partial record ChickenVariant(RegistryEntryHolder<string> Variant) : SlotComponent;
    public sealed partial record Consumable(ConsumableData Data) : SlotComponent;
    public sealed partial record Container(Slot[] Contents) : SlotComponent;
    public sealed partial record ContainerLoot(NbtTag Data) : SlotComponent;
    public sealed partial record CowVariant(int Value) : SlotComponent;
    public sealed partial record CreativeSlotLock() : SlotComponent;
    public sealed partial record CustomData(NbtTag Data) : SlotComponent;
    public sealed partial record CustomModelData(int? LegacyValue, float[]? Floats, bool[]? Flags, string[]? Strings, int[]? Colors) : SlotComponent;
    public sealed partial record CustomName(NbtTag Data) : SlotComponent;
    public sealed partial record Damage(int Value) : SlotComponent;
    public sealed partial record DamageResistant(string Value) : SlotComponent;
    public sealed partial record DeathProtection(ItemConsumeEffect[] Effects) : SlotComponent;
    public sealed partial record DebugStickState(NbtTag Data) : SlotComponent;
    public sealed partial record DyedColor(int Color, bool? ShowTooltip) : SlotComponent;
    public sealed partial record Enchantable(int Value) : SlotComponent;
    public sealed partial record EnchantmentGlintOverride(bool Value) : SlotComponent;
    public sealed partial record Enchantments(EnchantmentEntry[] Entries, bool? ShowTooltip) : SlotComponent;
    public sealed partial record EntityData(NbtTag Data) : SlotComponent;
    public sealed partial record Equippable(EquippableData Data) : SlotComponent;
    public sealed partial record FireResistant() : SlotComponent;
    public sealed partial record FireworkExplosion(ItemFireworkExplosion Explosion) : SlotComponent;
    public sealed partial record Fireworks(FireworksData Data) : SlotComponent;
    public sealed partial record Food(FoodData Data) : SlotComponent;
    public sealed partial record FoxVariant(int Value) : SlotComponent;
    public sealed partial record FrogVariant(int Value) : SlotComponent;
    public sealed partial record Glider() : SlotComponent;
    public sealed partial record HideAdditionalTooltip() : SlotComponent;
    public sealed partial record HideTooltip() : SlotComponent;
    public sealed partial record HorseVariant(int Value) : SlotComponent;
    public sealed partial record Instrument(InstrumentComponentData Data) : SlotComponent;
    public sealed partial record IntangibleProjectile(NbtTag? Data) : SlotComponent;
    public sealed partial record ItemModel(string Model) : SlotComponent;
    public sealed partial record ItemName(NbtTag Data) : SlotComponent;
    public sealed partial record JukeboxPlayable(JukeboxPlayableData Data) : SlotComponent;
    public sealed partial record LlamaVariant(int Value) : SlotComponent;
    public sealed partial record Lock(NbtTag Data) : SlotComponent;
    public sealed partial record LodestoneTracker(LodestoneTrackerData Data) : SlotComponent;
    public sealed partial record Lore(NbtTag?[] Lines) : SlotComponent;
    public sealed partial record MapColor(int Color) : SlotComponent;
    public sealed partial record MapDecorations(NbtTag Data) : SlotComponent;
    public sealed partial record MapId(int Value) : SlotComponent;
    public sealed partial record MapPostProcessing(int Value) : SlotComponent;
    public sealed partial record MaxDamage(int Value) : SlotComponent;
    public sealed partial record MaxStackSize(int Value) : SlotComponent;
    public sealed partial record MooshroomVariant(int Value) : SlotComponent;
    public sealed partial record NoteBlockSound(string Value) : SlotComponent;
    public sealed partial record OminousBottleAmplifier(int Value) : SlotComponent;
    public sealed partial record PaintingVariant(RegistryEntryHolder<EntityMetadataPaintingVariant> Variant) : SlotComponent;
    public sealed partial record ParrotVariant(int Value) : SlotComponent;
    public sealed partial record PigVariant(int Value) : SlotComponent;
    public sealed partial record PotDecorations(int[] Decorations) : SlotComponent;
    public sealed partial record PotionContents(PotionContentsData Data) : SlotComponent;
    public sealed partial record PotionDurationScale(float Value) : SlotComponent;
    public sealed partial record Profile(ProfileData Data) : SlotComponent;
    public sealed partial record ProvidesBannerPatterns(string Value) : SlotComponent;
    public sealed partial record ProvidesTrimMaterial(ProvidesTrimMaterialData Data) : SlotComponent;
    public sealed partial record RabbitVariant(int Value) : SlotComponent;
    public sealed partial record Rarity(string Value) : SlotComponent;
    public sealed partial record Recipes(NbtTag Data) : SlotComponent;
    public sealed partial record RepairCost(int Value) : SlotComponent;
    public sealed partial record Repairable(IDSet Items) : SlotComponent;
    public sealed partial record SalmonSize(int Value) : SlotComponent;
    public sealed partial record SheepColor(int Value) : SlotComponent;
    public sealed partial record ShulkerColor(int Value) : SlotComponent;
    public sealed partial record StoredEnchantments(EnchantmentEntry[] Entries, bool? ShowInTooltip) : SlotComponent;
    public sealed partial record SuspiciousStewEffects(SuspiciousStewEffect[] Effects) : SlotComponent;
    public sealed partial record Tool(ToolData Data) : SlotComponent;
    public sealed partial record TooltipDisplay(TooltipDisplayData Data) : SlotComponent;
    public sealed partial record TooltipStyle(string Value) : SlotComponent;
    public sealed partial record Trim(TrimData Data) : SlotComponent;
    public sealed partial record TropicalFishBaseColor(int Value) : SlotComponent;
    public sealed partial record TropicalFishPattern(int Value) : SlotComponent;
    public sealed partial record TropicalFishPatternColor(int Value) : SlotComponent;
    public sealed partial record Unbreakable(bool? Value) : SlotComponent;
    public sealed partial record UseCooldown(UseCooldownData Data) : SlotComponent;
    public sealed partial record UseRemainder(Slot Value) : SlotComponent;
    public sealed partial record VillagerVariant(int Value) : SlotComponent;
    public sealed partial record Weapon(WeaponData Data) : SlotComponent;
    public sealed partial record WolfCollar(int Value) : SlotComponent;
    public sealed partial record WolfSoundVariant(int Value) : SlotComponent;
    public sealed partial record WolfVariant(int Value) : SlotComponent;
    public sealed partial record WritableBookContent(ItemBookPage[] Pages) : SlotComponent;
    public sealed partial record WrittenBookContent(WrittenBookContentData Data) : SlotComponent;

    public sealed partial record AttributeModifierEntry(int TypeId, Guid? Uuid, string Name, double Value, string Operation, string Slot);
    public sealed partial record AttributeModifierDisplay(string Type, NbtTag? Component);
    public sealed partial record BeeData(NbtTag NbtData, int TicksInHive, int MinTicksInHive);
    public sealed partial record BlockStateProperty(string Name, string Value);
    public sealed partial record BlocksAttacksData(float BlockDelaySeconds, float DisableCooldownScale, DamageReduction[] DamageReductions,
        ItemDamageData ItemDamage, string? BypassedBy, ItemSoundHolder? BlockSound, ItemSoundHolder? DisableSound);
    public sealed partial record ConsumableData(float ConsumeSeconds, string Animation, ItemSoundHolder Sound, bool MakesParticles,
        ItemConsumeEffect[] Effects);
    public sealed partial record DamageReduction(float HorizontalBlockingAngle, IDSet? Type, float Base, float Factor);
    public sealed partial record EquippableData(string Slot, ItemSoundHolder Sound, string? Model, string? CameraOverlay, IDSet? AllowedEntities,
        bool Dispensable, bool Swappable, bool Damageable, bool? EquipOnInteract, bool? Shearable, ItemSoundHolder? ShearingSound);
    public sealed partial record EnchantmentEntry(int Id, int Level);
    public sealed partial record FireworksData(int FlightDuration, ItemFireworkExplosion[] Explosions);
    public sealed partial record FoodData(int Nutrition, float SaturationModifier, bool CanAlwaysEat, float? SecondsToEat,
        Slot? UsingConvertsTo, FoodEffect[]? Effects);
    public sealed partial record FoodEffect(int Effect, float Probability);
    public sealed partial record InstrumentComponentData(RegistryEntryHolder<InstrumentData>? Holder, string? InlineId);
    public sealed partial record ItemDamageData(float Threshold, float Base, float Factor);
    public sealed partial record JukeboxPlayableData(RegistryEntryHolder<JukeboxSongData>? Holder, string? Song, bool? ShowInTooltip);
    public sealed partial record LodestonePosition(string Dimension, Position Position);
    public sealed partial record LodestoneTrackerData(LodestonePosition? GlobalPosition, bool Tracked);
    public sealed partial record PotionContentsData(int? PotionId, int? CustomColor, ItemPotionEffect[] CustomEffects,
        string? CustomNameText);
    public sealed partial record ProfileData(string? Name, Guid? Uuid, ProfileProperty[] Properties);
    public sealed partial record ProfileProperty(string Name, string Value, string? Signature);
    public sealed partial record ProvidesTrimMaterialData(RegistryEntryHolder<ArmorTrimMaterial>? Holder, string? MaterialId);
    public sealed partial record SuspiciousStewEffect(int Effect, int Duration);
    public sealed partial record ToolData(ToolRule[] Rules, float DefaultMiningSpeed, int DamagePerBlock, bool? CanDestroyBlocksInCreative);
    public sealed partial record ToolRule(IDSet Blocks, float? Speed, bool? CorrectDropForBlocks);
    public sealed partial record TooltipDisplayData(bool HideTooltipFlag, int[] HiddenComponents);
    public sealed partial record TrimData(RegistryEntryHolder<ArmorTrimMaterial> Material, RegistryEntryHolder<ArmorTrimPattern> Pattern,
        bool? ShowInTooltip);
    public sealed partial record UseCooldownData(float Seconds, string? CooldownGroup);
    public sealed partial record WeaponData(int ItemDamagePerAttack, float DisableBlockingForSeconds);
    public sealed partial record WrittenBookContentData(string RawTitle, string? FilteredTitle, string Author, int Generation,
        ItemWrittenBookPage[] Pages, bool Resolved);
}
