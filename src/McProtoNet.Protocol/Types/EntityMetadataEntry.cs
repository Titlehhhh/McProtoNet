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
    public sealed record Byte(sbyte Value) : EntityMetadataValue;
    public sealed record Int(int Value) : EntityMetadataValue;
    public sealed record Long(long Value) : EntityMetadataValue;
    public sealed record Float(float Value) : EntityMetadataValue;
    public sealed record String(string Value) : EntityMetadataValue;
    public sealed record Component(NbtTag Value) : EntityMetadataValue;
    public sealed record OptionalComponent(NbtTag? Value) : EntityMetadataValue;
    public sealed record ItemStack(Slot Value) : EntityMetadataValue;
    public sealed record Boolean(bool Value) : EntityMetadataValue;
    public sealed record Rotations(float Pitch, float Yaw, float Roll) : EntityMetadataValue;
    public sealed record BlockPos(Position Value) : EntityMetadataValue;
    public sealed record OptionalBlockPos(Position? Value) : EntityMetadataValue;
    public sealed record Direction(int Value) : EntityMetadataValue;
    public sealed record OptionalUuid(Guid? Value) : EntityMetadataValue;
    public sealed record BlockState(int Value) : EntityMetadataValue;
    public sealed record OptionalBlockState(int Value) : EntityMetadataValue;
    public sealed record CompoundTag(NbtTag Value) : EntityMetadataValue;
    public sealed record Particle(Particle Value) : EntityMetadataValue;
    public sealed record Particles(Particle[] Values) : EntityMetadataValue;
    public sealed record VillagerData(int VillagerType, int VillagerProfession, int Level) : EntityMetadataValue;
    public sealed record OptionalUnsignedInt(int Value) : EntityMetadataValue;
    public sealed record Pose(int Value) : EntityMetadataValue;
    public sealed record CatVariant(int Value) : EntityMetadataValue;
    public sealed record CowVariant(int Value) : EntityMetadataValue;
    public sealed record WolfVariantHolder(RegistryEntryHolder<EntityMetadataWolfVariant> Value) : EntityMetadataValue;
    public sealed record WolfVariant(int Value) : EntityMetadataValue;
    public sealed record WolfSoundVariant(int Value) : EntityMetadataValue;
    public sealed record FrogVariant(int Value) : EntityMetadataValue;
    public sealed record PigVariant(int Value) : EntityMetadataValue;
    public sealed record ChickenVariant(RegistryEntryHolder<string> Value) : EntityMetadataValue;
    public sealed record OptionalGlobalPos(string? Value) : EntityMetadataValue;
    public sealed record PaintingVariant(RegistryEntryHolder<EntityMetadataPaintingVariant> Value) : EntityMetadataValue;
    public sealed record SnifferState(int Value) : EntityMetadataValue;
    public sealed record ArmadilloState(int Value) : EntityMetadataValue;
    public sealed record Vector3(Vec3f Value) : EntityMetadataValue;
    public sealed record Quaternion(Vec4f Value) : EntityMetadataValue;
}
