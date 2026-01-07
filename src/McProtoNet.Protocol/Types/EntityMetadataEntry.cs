using System;
using Dunet;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class EntityMetadataEntry
{
    public byte Key { get; }
    public string Type { get; }
    public EntityMetadataValue Value { get; }

    public EntityMetadataEntry(byte key, string type, EntityMetadataValue value)
    {
        Key = key;
        Type = type;
        Value = value;
    }
}

[Union]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public partial record EntityMetadataValue
{
    public sealed partial record Byte(sbyte Value) : EntityMetadataValue;
    public sealed partial record Int(int Value) : EntityMetadataValue;
    public sealed partial record Long(long Value) : EntityMetadataValue;
    public sealed partial record Float(float Value) : EntityMetadataValue;
    public sealed partial record String(string Value) : EntityMetadataValue;
    public sealed partial record Component(NbtTag Value) : EntityMetadataValue;
    public sealed partial record OptionalComponent(NbtTag? Value) : EntityMetadataValue;
    public sealed partial record ItemStack(Slot Value) : EntityMetadataValue;
    public sealed partial record Boolean(bool Value) : EntityMetadataValue;
    public sealed partial record Rotations(float Pitch, float Yaw, float Roll) : EntityMetadataValue;
    public sealed partial record BlockPos(Position Value) : EntityMetadataValue;
    public sealed partial record OptionalBlockPos(Position? Value) : EntityMetadataValue;
    public sealed partial record Direction(int Value) : EntityMetadataValue;
    public sealed partial record OptionalUuid(Guid? Value) : EntityMetadataValue;
    public sealed partial record BlockState(int Value) : EntityMetadataValue;
    public sealed partial record OptionalBlockState(int Value) : EntityMetadataValue;
    public sealed partial record CompoundTag(NbtTag Value) : EntityMetadataValue;
    public sealed partial record Particle(global::McProtoNet.Protocol.Particle Value) : EntityMetadataValue;
    public sealed partial record Particles(global::McProtoNet.Protocol.Particle[] Values) : EntityMetadataValue;
    public sealed partial record VillagerData(int VillagerType, int VillagerProfession, int Level) : EntityMetadataValue;
    public sealed partial record OptionalUnsignedInt(int Value) : EntityMetadataValue;
    public sealed partial record Pose(int Value) : EntityMetadataValue;
    public sealed partial record CatVariant(int Value) : EntityMetadataValue;
    public sealed partial record CowVariant(int Value) : EntityMetadataValue;
    public sealed partial record WolfVariantHolder(RegistryEntryHolder<EntityMetadataWolfVariant> Value) : EntityMetadataValue;
    public sealed partial record WolfVariant(int Value) : EntityMetadataValue;
    public sealed partial record WolfSoundVariant(int Value) : EntityMetadataValue;
    public sealed partial record FrogVariant(int Value) : EntityMetadataValue;
    public sealed partial record PigVariant(int Value) : EntityMetadataValue;
    public sealed partial record ChickenVariant(RegistryEntryHolder<string> Value) : EntityMetadataValue;
    public sealed partial record OptionalGlobalPos(string? Value) : EntityMetadataValue;
    public sealed partial record PaintingVariant(RegistryEntryHolder<EntityMetadataPaintingVariant> Value) : EntityMetadataValue;
    public sealed partial record SnifferState(int Value) : EntityMetadataValue;
    public sealed partial record ArmadilloState(int Value) : EntityMetadataValue;
    public sealed partial record Vector3(Vec3f Value) : EntityMetadataValue;
    public sealed partial record Quaternion(Vec4f Value) : EntityMetadataValue;
}
