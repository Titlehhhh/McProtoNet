using System;
using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static EntityMetadataEntry ReadEntityMetadataEntry(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMetadataEntry>(protocolVersion);
        byte key = reader.ReadUnsignedByte();
        string type = ReadEntityMetadataType(reader.ReadVarInt(), protocolVersion);
        EntityMetadataValue value = type switch
        {
            "byte" => new EntityMetadataValue.Byte(reader.ReadSignedByte()),
            "int" => new EntityMetadataValue.Int(reader.ReadVarInt()),
            "long" => new EntityMetadataValue.Long(reader.ReadVarLong()),
            "float" => new EntityMetadataValue.Float(reader.ReadFloat()),
            "string" => new EntityMetadataValue.String(reader.ReadString()),
            "component" => new EntityMetadataValue.Component(reader.ReadAnonymousNbtTag(protocolVersion)
                ?? throw new InvalidOperationException("component missing")),
            "optional_component" => new EntityMetadataValue.OptionalComponent(
                reader.ReadBoolean() ? reader.ReadAnonymousNbtTag(protocolVersion) : null),
            "item_stack" => new EntityMetadataValue.ItemStack(reader.ReadSlot(protocolVersion)),
            "boolean" => new EntityMetadataValue.Boolean(reader.ReadBoolean()),
            "rotations" => new EntityMetadataValue.Rotations(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            "block_pos" => new EntityMetadataValue.BlockPos(reader.ReadPosition(protocolVersion)),
            "optional_block_pos" => new EntityMetadataValue.OptionalBlockPos(
                reader.ReadBoolean() ? reader.ReadPosition(protocolVersion) : null),
            "direction" => new EntityMetadataValue.Direction(reader.ReadVarInt()),
            "optional_uuid" => new EntityMetadataValue.OptionalUuid(reader.ReadBoolean() ? reader.ReadUUID() : null),
            "block_state" => new EntityMetadataValue.BlockState(reader.ReadVarInt()),
            "optional_block_state" => new EntityMetadataValue.OptionalBlockState(reader.ReadVarInt()),
            "compound_tag" => new EntityMetadataValue.CompoundTag(reader.ReadAnonymousNbtTag(protocolVersion)
                ?? throw new InvalidOperationException("compound_tag missing")),
            "particle" => new EntityMetadataValue.Particle(reader.ReadParticle(protocolVersion)),
            "particles" => new EntityMetadataValue.Particles(ReadArray(ref reader,
                (ref MinecraftPrimitiveReader r) => r.ReadParticle(protocolVersion))),
            "villager_data" => new EntityMetadataValue.VillagerData(reader.ReadVarInt(), reader.ReadVarInt(), reader.ReadVarInt()),
            "optional_unsigned_int" => new EntityMetadataValue.OptionalUnsignedInt(reader.ReadVarInt()),
            "pose" => new EntityMetadataValue.Pose(reader.ReadVarInt()),
            "cat_variant" => new EntityMetadataValue.CatVariant(reader.ReadVarInt()),
            "cow_variant" => new EntityMetadataValue.CowVariant(reader.ReadVarInt()),
            "wolf_variant" when protocolVersion <= 769 => new EntityMetadataValue.WolfVariantHolder(
                reader.ReadRegistryEntryHolder<EntityMetadataWolfVariant>(protocolVersion)),
            "wolf_variant" => new EntityMetadataValue.WolfVariant(reader.ReadVarInt()),
            "wolf_sound_variant" => new EntityMetadataValue.WolfSoundVariant(reader.ReadVarInt()),
            "frog_variant" => new EntityMetadataValue.FrogVariant(reader.ReadVarInt()),
            "pig_variant" => new EntityMetadataValue.PigVariant(reader.ReadVarInt()),
            "chicken_variant" => new EntityMetadataValue.ChickenVariant(reader.ReadRegistryEntryHolder<string>(protocolVersion)),
            "optional_global_pos" => new EntityMetadataValue.OptionalGlobalPos(reader.ReadBoolean() ? reader.ReadString() : null),
            "painting_variant" => new EntityMetadataValue.PaintingVariant(
                reader.ReadRegistryEntryHolder<EntityMetadataPaintingVariant>(protocolVersion)),
            "sniffer_state" => new EntityMetadataValue.SnifferState(reader.ReadVarInt()),
            "armadillo_state" => new EntityMetadataValue.ArmadilloState(reader.ReadVarInt()),
            "vector3" => new EntityMetadataValue.Vector3(reader.ReadVec3f(protocolVersion)),
            "quaternion" => new EntityMetadataValue.Quaternion(reader.ReadVec4f(protocolVersion)),
            _ => throw new InvalidOperationException($"Unknown EntityMetadataEntry type {type}")
        };
        return new EntityMetadataEntry(key, type, value);
    }

    public static void WriteEntityMetadataEntry(this MinecraftPrimitiveWriter writer, EntityMetadataEntry value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMetadataEntry>(protocolVersion);
        writer.WriteUnsignedByte(value.Key);
        writer.WriteVarInt(WriteEntityMetadataType(value.Type, protocolVersion));
        switch (value.Type)
        {
            case "byte" when value.Value is EntityMetadataValue.Byte data:
                writer.WriteSignedByte(data.Value);
                break;
            case "int" when value.Value is EntityMetadataValue.Int data:
                writer.WriteVarInt(data.Value);
                break;
            case "long" when value.Value is EntityMetadataValue.Long data:
                writer.WriteVarLong(data.Value);
                break;
            case "float" when value.Value is EntityMetadataValue.Float data:
                writer.WriteFloat(data.Value);
                break;
            case "string" when value.Value is EntityMetadataValue.String data:
                writer.WriteString(data.Value);
                break;
            case "component" when value.Value is EntityMetadataValue.Component data:
                writer.WriteAnonymousNbtTag(data.Value, protocolVersion);
                break;
            case "optional_component" when value.Value is EntityMetadataValue.OptionalComponent data:
                if (data.Value is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(data.Value, protocolVersion);
                }
                break;
            case "item_stack" when value.Value is EntityMetadataValue.ItemStack data:
                writer.WriteSlot(data.Value, protocolVersion);
                break;
            case "boolean" when value.Value is EntityMetadataValue.Boolean data:
                writer.WriteBoolean(data.Value);
                break;
            case "rotations" when value.Value is EntityMetadataValue.Rotations data:
                writer.WriteFloat(data.Pitch);
                writer.WriteFloat(data.Yaw);
                writer.WriteFloat(data.Roll);
                break;
            case "block_pos" when value.Value is EntityMetadataValue.BlockPos data:
                writer.WritePosition(data.Value, protocolVersion);
                break;
            case "optional_block_pos" when value.Value is EntityMetadataValue.OptionalBlockPos data:
                if (data.Value is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WritePosition(data.Value.Value, protocolVersion);
                }
                break;
            case "direction" when value.Value is EntityMetadataValue.Direction data:
                writer.WriteVarInt(data.Value);
                break;
            case "optional_uuid" when value.Value is EntityMetadataValue.OptionalUuid data:
                if (data.Value is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteUUID(data.Value.Value);
                }
                break;
            case "block_state" when value.Value is EntityMetadataValue.BlockState data:
                writer.WriteVarInt(data.Value);
                break;
            case "optional_block_state" when value.Value is EntityMetadataValue.OptionalBlockState data:
                writer.WriteVarInt(data.Value);
                break;
            case "compound_tag" when value.Value is EntityMetadataValue.CompoundTag data:
                writer.WriteAnonymousNbtTag(data.Value, protocolVersion);
                break;
            case "particle" when value.Value is EntityMetadataValue.Particle data:
                writer.WriteParticle(data.Value, protocolVersion);
                break;
            case "particles" when value.Value is EntityMetadataValue.Particles data:
                WriteArray(ref writer, data.Values, (ref MinecraftPrimitiveWriter w, Particle particle) =>
                    w.WriteParticle(particle, protocolVersion));
                break;
            case "villager_data" when value.Value is EntityMetadataValue.VillagerData data:
                writer.WriteVarInt(data.VillagerType);
                writer.WriteVarInt(data.VillagerProfession);
                writer.WriteVarInt(data.Level);
                break;
            case "optional_unsigned_int" when value.Value is EntityMetadataValue.OptionalUnsignedInt data:
                writer.WriteVarInt(data.Value);
                break;
            case "pose" when value.Value is EntityMetadataValue.Pose data:
                writer.WriteVarInt(data.Value);
                break;
            case "cat_variant" when value.Value is EntityMetadataValue.CatVariant data:
                writer.WriteVarInt(data.Value);
                break;
            case "cow_variant" when value.Value is EntityMetadataValue.CowVariant data:
                if (protocolVersion <= 769)
                {
                    throw new InvalidOperationException("cow_variant not supported before protocol 770.");
                }
                writer.WriteVarInt(data.Value);
                break;
            case "wolf_variant" when value.Value is EntityMetadataValue.WolfVariantHolder data:
                if (protocolVersion >= 770)
                {
                    throw new InvalidOperationException("wolf_variant holder not supported in protocol 770+.");
                }
                writer.WriteRegistryEntryHolder(data.Value, protocolVersion);
                break;
            case "wolf_variant" when value.Value is EntityMetadataValue.WolfVariant data:
                if (protocolVersion <= 769)
                {
                    throw new InvalidOperationException("wolf_variant id not supported before protocol 770.");
                }
                writer.WriteVarInt(data.Value);
                break;
            case "wolf_sound_variant" when value.Value is EntityMetadataValue.WolfSoundVariant data:
                writer.WriteVarInt(data.Value);
                break;
            case "frog_variant" when value.Value is EntityMetadataValue.FrogVariant data:
                writer.WriteVarInt(data.Value);
                break;
            case "pig_variant" when value.Value is EntityMetadataValue.PigVariant data:
                writer.WriteVarInt(data.Value);
                break;
            case "chicken_variant" when value.Value is EntityMetadataValue.ChickenVariant data:
                if (protocolVersion <= 769)
                {
                    throw new InvalidOperationException("chicken_variant not supported before protocol 770.");
                }
                writer.WriteRegistryEntryHolder(data.Value, protocolVersion);
                break;
            case "optional_global_pos" when value.Value is EntityMetadataValue.OptionalGlobalPos data:
                if (data.Value is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(data.Value);
                }
                break;
            case "painting_variant" when value.Value is EntityMetadataValue.PaintingVariant data:
                writer.WriteRegistryEntryHolder(data.Value, protocolVersion);
                break;
            case "sniffer_state" when value.Value is EntityMetadataValue.SnifferState data:
                writer.WriteVarInt(data.Value);
                break;
            case "armadillo_state" when value.Value is EntityMetadataValue.ArmadilloState data:
                writer.WriteVarInt(data.Value);
                break;
            case "vector3" when value.Value is EntityMetadataValue.Vector3 data:
                writer.WriteVec3f(data.Value, protocolVersion);
                break;
            case "quaternion" when value.Value is EntityMetadataValue.Quaternion data:
                writer.WriteVec4f(data.Value, protocolVersion);
                break;
            default:
                throw new InvalidOperationException($"EntityMetadataEntry value mismatch for type {value.Type}");
        }
    }

    public static EntityMetadataWolfVariant ReadEntityMetadataWolfVariant(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMetadataWolfVariant>(protocolVersion);
        string wildTexture = reader.ReadString();
        string tameTexture = reader.ReadString();
        string angryTexture = reader.ReadString();
        IDSet biome = reader.ReadIDSet(protocolVersion);
        return new EntityMetadataWolfVariant(wildTexture, tameTexture, angryTexture, biome);
    }

    public static void WriteEntityMetadataWolfVariant(this MinecraftPrimitiveWriter writer, EntityMetadataWolfVariant value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMetadataWolfVariant>(protocolVersion);
        writer.WriteString(value.WildTexture);
        writer.WriteString(value.TameTexture);
        writer.WriteString(value.AngryTexture);
        writer.WriteIDSet(value.Biome, protocolVersion);
    }

    private static string ReadEntityMetadataType(int id, int protocolVersion)
    {
        if (protocolVersion <= 769)
        {
            return id switch
            {
                0 => "byte",
                1 => "int",
                2 => "long",
                3 => "float",
                4 => "string",
                5 => "component",
                6 => "optional_component",
                7 => "item_stack",
                8 => "boolean",
                9 => "rotations",
                10 => "block_pos",
                11 => "optional_block_pos",
                12 => "direction",
                13 => "optional_uuid",
                14 => "block_state",
                15 => "optional_block_state",
                16 => "compound_tag",
                17 => "particle",
                18 => "particles",
                19 => "villager_data",
                20 => "optional_unsigned_int",
                21 => "pose",
                22 => "cat_variant",
                23 => "wolf_variant",
                24 => "frog_variant",
                25 => "optional_global_pos",
                26 => "painting_variant",
                27 => "sniffer_state",
                28 => "armadillo_state",
                29 => "vector3",
                30 => "quaternion",
                _ => throw new InvalidOperationException($"Unknown EntityMetadata type id {id}")
            };
        }

        return id switch
        {
            0 => "byte",
            1 => "int",
            2 => "long",
            3 => "float",
            4 => "string",
            5 => "component",
            6 => "optional_component",
            7 => "item_stack",
            8 => "boolean",
            9 => "rotations",
            10 => "block_pos",
            11 => "optional_block_pos",
            12 => "direction",
            13 => "optional_uuid",
            14 => "block_state",
            15 => "optional_block_state",
            16 => "compound_tag",
            17 => "particle",
            18 => "particles",
            19 => "villager_data",
            20 => "optional_unsigned_int",
            21 => "pose",
            22 => "cat_variant",
            23 => "cow_variant",
            24 => "wolf_variant",
            25 => "wolf_sound_variant",
            26 => "frog_variant",
            27 => "pig_variant",
            28 => "chicken_variant",
            29 => "optional_global_pos",
            30 => "painting_variant",
            31 => "sniffer_state",
            32 => "armadillo_state",
            33 => "vector3",
            34 => "quaternion",
            _ => throw new InvalidOperationException($"Unknown EntityMetadata type id {id}")
        };
    }

    private static int WriteEntityMetadataType(string value, int protocolVersion)
    {
        if (protocolVersion <= 769)
        {
            return value switch
            {
                "byte" => 0,
                "int" => 1,
                "long" => 2,
                "float" => 3,
                "string" => 4,
                "component" => 5,
                "optional_component" => 6,
                "item_stack" => 7,
                "boolean" => 8,
                "rotations" => 9,
                "block_pos" => 10,
                "optional_block_pos" => 11,
                "direction" => 12,
                "optional_uuid" => 13,
                "block_state" => 14,
                "optional_block_state" => 15,
                "compound_tag" => 16,
                "particle" => 17,
                "particles" => 18,
                "villager_data" => 19,
                "optional_unsigned_int" => 20,
                "pose" => 21,
                "cat_variant" => 22,
                "wolf_variant" => 23,
                "frog_variant" => 24,
                "optional_global_pos" => 25,
                "painting_variant" => 26,
                "sniffer_state" => 27,
                "armadillo_state" => 28,
                "vector3" => 29,
                "quaternion" => 30,
                _ => throw new InvalidOperationException($"Unknown EntityMetadata type {value}")
            };
        }

        return value switch
        {
            "byte" => 0,
            "int" => 1,
            "long" => 2,
            "float" => 3,
            "string" => 4,
            "component" => 5,
            "optional_component" => 6,
            "item_stack" => 7,
            "boolean" => 8,
            "rotations" => 9,
            "block_pos" => 10,
            "optional_block_pos" => 11,
            "direction" => 12,
            "optional_uuid" => 13,
            "block_state" => 14,
            "optional_block_state" => 15,
            "compound_tag" => 16,
            "particle" => 17,
            "particles" => 18,
            "villager_data" => 19,
            "optional_unsigned_int" => 20,
            "pose" => 21,
            "cat_variant" => 22,
            "cow_variant" => 23,
            "wolf_variant" => 24,
            "wolf_sound_variant" => 25,
            "frog_variant" => 26,
            "pig_variant" => 27,
            "chicken_variant" => 28,
            "optional_global_pos" => 29,
            "painting_variant" => 30,
            "sniffer_state" => 31,
            "armadillo_state" => 32,
            "vector3" => 33,
            "quaternion" => 34,
            _ => throw new InvalidOperationException($"Unknown EntityMetadata type {value}")
        };
    }
}
