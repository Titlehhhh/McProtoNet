using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityMetadata", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EntityMetadataPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol),
    };

    public VFirst_761Fields? VFirst_761 { get; set; }
    public V762_765Fields? V762_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 761:
            {
                var fields = VFirst_761 ?? throw new InvalidOperationException("EntityMetadata VFirst_761 fields missing.");
                writer.WriteVarInt(fields.EntityId);
                WriteLegacyMetadataEntries(ref writer, fields.Metadata, protocolVersion);
                writer.WriteUnsignedByte(0xFF);
                return;
            }
            case >= 762 and <= 765:
            {
                var fields = V762_765 ?? throw new InvalidOperationException("EntityMetadata V762_765 fields missing.");
                writer.WriteVarInt(fields.EntityId);
                WriteModernMetadataEntries(ref writer, fields.Metadata, protocolVersion);
                writer.WriteUnsignedByte(0xFF);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("EntityMetadata V766_Last fields missing.");
                writer.WriteVarInt(fields.EntityId);
                WriteCurrentMetadataEntries(ref writer, fields.Metadata, protocolVersion);
                writer.WriteUnsignedByte(0xFF);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityMetadata), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 761:
            {
                int entityId = reader.ReadVarInt();
                LegacyMetadataEntry[] metadata = ReadLegacyMetadataEntries(ref reader, protocolVersion);
                VFirst_761 = new VFirst_761Fields
                {
                    EntityId = entityId,
                    Metadata = metadata
                };
                return;
            }
            case >= 762 and <= 765:
            {
                int entityId = reader.ReadVarInt();
                ModernMetadataEntry[] metadata = ReadModernMetadataEntries(ref reader, protocolVersion);
                V762_765 = new V762_765Fields
                {
                    EntityId = entityId,
                    Metadata = metadata
                };
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                int entityId = reader.ReadVarInt();
                EntityMetadataEntry[] metadata = ReadCurrentMetadataEntries(ref reader, protocolVersion);
                V766_Last = new V766_LastFields
                {
                    EntityId = entityId,
                    Metadata = metadata
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityMetadata), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static LegacyMetadataEntry[] ReadLegacyMetadataEntries(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var entries = new System.Collections.Generic.List<LegacyMetadataEntry>();
        while (true)
        {
            byte key = reader.ReadUnsignedByte();
            if (key == 0xFF)
            {
                break;
            }

            int typeId = reader.ReadVarInt();
            object value = ReadLegacyMetadataValue(ref reader, typeId, protocolVersion);
            entries.Add(new LegacyMetadataEntry { Key = key, TypeId = typeId, Value = value });
        }

        return entries.Count == 0 ? Array.Empty<LegacyMetadataEntry>() : entries.ToArray();
    }

    private static ModernMetadataEntry[] ReadModernMetadataEntries(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var entries = new System.Collections.Generic.List<ModernMetadataEntry>();
        while (true)
        {
            byte key = reader.ReadUnsignedByte();
            if (key == 0xFF)
            {
                break;
            }

            int typeId = reader.ReadVarInt();
            string type = ReadModernMetadataType(typeId);
            object value = ReadModernMetadataValue(ref reader, type, protocolVersion);
            entries.Add(new ModernMetadataEntry { Key = key, Type = type, Value = value });
        }

        return entries.Count == 0 ? Array.Empty<ModernMetadataEntry>() : entries.ToArray();
    }

    private static EntityMetadataEntry[] ReadCurrentMetadataEntries(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        var entries = new System.Collections.Generic.List<EntityMetadataEntry>();
        while (true)
        {
            byte key = reader.ReadUnsignedByte();
            if (key == 0xFF)
            {
                break;
            }

            int typeId = reader.ReadVarInt();
            string type = ReadCurrentMetadataType(typeId);
            EntityMetadataValue value = ReadCurrentMetadataValue(ref reader, type, protocolVersion);
            entries.Add(new EntityMetadataEntry(key, type, value));
        }

        return entries.Count == 0 ? Array.Empty<EntityMetadataEntry>() : entries.ToArray();
    }

    private static void WriteLegacyMetadataEntries(ref MinecraftPrimitiveWriter writer, LegacyMetadataEntry[] entries,
        int protocolVersion)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            writer.WriteUnsignedByte(entries[i].Key);
            writer.WriteVarInt(entries[i].TypeId);
            WriteLegacyMetadataValue(ref writer, entries[i].TypeId, entries[i].Value, protocolVersion);
        }
    }

    private static void WriteModernMetadataEntries(ref MinecraftPrimitiveWriter writer, ModernMetadataEntry[] entries,
        int protocolVersion)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            writer.WriteUnsignedByte(entries[i].Key);
            writer.WriteVarInt(GetModernMetadataTypeId(entries[i].Type));
            WriteModernMetadataValue(ref writer, entries[i].Type, entries[i].Value, protocolVersion);
        }
    }

    private static void WriteCurrentMetadataEntries(ref MinecraftPrimitiveWriter writer, EntityMetadataEntry[] entries,
        int protocolVersion)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            writer.WriteUnsignedByte(entries[i].Key);
            writer.WriteVarInt(GetCurrentMetadataTypeId(entries[i].Type));
            WriteCurrentMetadataValue(ref writer, entries[i].Type, entries[i].Value, protocolVersion);
        }
    }

    private static object ReadLegacyMetadataValue(ref MinecraftPrimitiveReader reader, int typeId, int protocolVersion)
    {
        if (protocolVersion <= 759)
        {
            return typeId switch
            {
                0 => reader.ReadSignedByte(),
                1 => reader.ReadVarInt(),
                2 => reader.ReadFloat(),
                3 => reader.ReadString(),
                4 => reader.ReadString(),
                5 => reader.ReadOptional(ReadDelegates.String),
                6 => reader.ReadSlot(protocolVersion),
                7 => reader.ReadBoolean(),
                8 => new Rotations(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                9 => reader.ReadPosition(protocolVersion),
                10 => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadPosition(protocolVersion)),
                11 => reader.ReadVarInt(),
                12 => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID()),
                13 => reader.ReadVarInt(),
                14 => reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("nbt missing"),
                15 => reader.ReadParticle(protocolVersion),
                16 => new VillagerData(reader.ReadVarInt(), reader.ReadVarInt(), reader.ReadVarInt()),
                17 => ReadOptionalVarInt(ref reader),
                18 => reader.ReadVarInt(),
                _ => throw new InvalidOperationException($"Unknown legacy metadata type {typeId}.")
            };
        }

        if (protocolVersion == 760)
        {
            return typeId switch
            {
                0 => reader.ReadSignedByte(),
                1 => reader.ReadVarInt(),
                2 => reader.ReadFloat(),
                3 => reader.ReadString(),
                4 => reader.ReadString(),
                5 => reader.ReadOptional(ReadDelegates.String),
                6 => reader.ReadSlot(protocolVersion),
                7 => reader.ReadBoolean(),
                8 => new Rotations(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                9 => reader.ReadPosition(protocolVersion),
                10 => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadPosition(protocolVersion)),
                11 => reader.ReadVarInt(),
                12 => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID()),
                13 => reader.ReadVarInt(),
                14 => reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("nbt missing"),
                15 => reader.ReadParticle(protocolVersion),
                16 => new VillagerData(reader.ReadVarInt(), reader.ReadVarInt(), reader.ReadVarInt()),
                17 => ReadOptionalVarInt(ref reader),
                18 => reader.ReadVarInt(),
                19 => reader.ReadVarInt(),
                20 => reader.ReadVarInt(),
                21 => reader.ReadOptional(ReadDelegates.String),
                22 => reader.ReadVarInt(),
                _ => throw new InvalidOperationException($"Unknown legacy metadata type {typeId}.")
            };
        }

        return typeId switch
        {
            0 => reader.ReadSignedByte(),
            1 => reader.ReadVarInt(),
            2 => reader.ReadVarLong(),
            3 => reader.ReadFloat(),
            4 => reader.ReadString(),
            5 => reader.ReadString(),
            6 => reader.ReadOptional(ReadDelegates.String),
            7 => reader.ReadSlot(protocolVersion),
            8 => reader.ReadBoolean(),
            9 => new Rotations(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            10 => reader.ReadPosition(protocolVersion),
            11 => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadPosition(protocolVersion)),
            12 => reader.ReadVarInt(),
            13 => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID()),
            14 => reader.ReadVarInt(),
            15 => reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("nbt missing"),
            16 => reader.ReadParticle(protocolVersion),
            17 => new VillagerData(reader.ReadVarInt(), reader.ReadVarInt(), reader.ReadVarInt()),
            18 => ReadOptionalVarInt(ref reader),
            19 => reader.ReadVarInt(),
            20 => reader.ReadVarInt(),
            21 => reader.ReadVarInt(),
            22 => reader.ReadOptional(ReadDelegates.String),
            23 => reader.ReadVarInt(),
            _ => throw new InvalidOperationException($"Unknown legacy metadata type {typeId}.")
        };
    }

    private static object ReadModernMetadataValue(ref MinecraftPrimitiveReader reader, string type, int protocolVersion)
    {
        return type switch
        {
            "byte" => reader.ReadSignedByte(),
            "int" => reader.ReadVarInt(),
            "long" => reader.ReadVarLong(),
            "float" => reader.ReadFloat(),
            "string" => reader.ReadString(),
            "component" => protocolVersion >= 765
                ? reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("component missing")
                : reader.ReadString(),
            "optional_component" => protocolVersion >= 765
                ? reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion))
                : reader.ReadOptional(ReadDelegates.String),
            "item_stack" => reader.ReadSlot(protocolVersion),
            "boolean" => reader.ReadBoolean(),
            "rotations" => new Rotations(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            "block_pos" => reader.ReadPosition(protocolVersion),
            "optional_block_pos" => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadPosition(protocolVersion)),
            "direction" => reader.ReadVarInt(),
            "optional_uuid" => reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID()),
            "block_state" => reader.ReadVarInt(),
            "optional_block_state" => ReadOptionalVarInt(ref reader),
            "compound_tag" => protocolVersion >= 764
                ? reader.ReadAnonymousNbtTag(protocolVersion) ?? throw new InvalidOperationException("compound_tag missing")
                : reader.ReadNbtTag(protocolVersion) ?? throw new InvalidOperationException("compound_tag missing"),
            "particle" => reader.ReadParticle(protocolVersion),
            "villager_data" => new VillagerData(reader.ReadVarInt(), reader.ReadVarInt(), reader.ReadVarInt()),
            "optional_unsigned_int" => ReadOptionalVarInt(ref reader),
            "pose" => reader.ReadVarInt(),
            "cat_variant" => reader.ReadVarInt(),
            "frog_variant" => reader.ReadVarInt(),
            "optional_global_pos" => reader.ReadOptional(ReadDelegates.String),
            "painting_variant" => reader.ReadVarInt(),
            "sniffer_state" => reader.ReadVarInt(),
            "vector3" => reader.ReadVec3f(protocolVersion),
            "quaternion" => reader.ReadVec4f(protocolVersion),
            _ => throw new InvalidOperationException($"Unknown metadata type {type}.")
        };
    }

    private static EntityMetadataValue ReadCurrentMetadataValue(ref MinecraftPrimitiveReader reader, string type,
        int protocolVersion)
    {
        return type switch
        {
            "byte" => new EntityMetadataValue.Byte(reader.ReadSignedByte()),
            "int" => new EntityMetadataValue.Int(reader.ReadVarInt()),
            "long" => new EntityMetadataValue.Long(reader.ReadVarLong()),
            "float" => new EntityMetadataValue.Float(reader.ReadFloat()),
            "string" => new EntityMetadataValue.String(reader.ReadString()),
            "component" => new EntityMetadataValue.Component(reader.ReadAnonymousNbtTag(protocolVersion)
                ?? throw new InvalidOperationException("component missing")),
            "optional_component" => new EntityMetadataValue.OptionalComponent(reader.ReadBoolean()
                ? reader.ReadAnonymousNbtTag(protocolVersion)
                : null),
            "item_stack" => new EntityMetadataValue.ItemStack(reader.ReadSlot(protocolVersion)),
            "boolean" => new EntityMetadataValue.Boolean(reader.ReadBoolean()),
            "rotations" => new EntityMetadataValue.Rotations(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            "block_pos" => new EntityMetadataValue.BlockPos(reader.ReadPosition(protocolVersion)),
            "optional_block_pos" => new EntityMetadataValue.OptionalBlockPos(reader.ReadBoolean()
                ? reader.ReadPosition(protocolVersion)
                : null),
            "direction" => new EntityMetadataValue.Direction(reader.ReadVarInt()),
            "optional_uuid" => new EntityMetadataValue.OptionalUuid(reader.ReadBoolean() ? reader.ReadUUID() : null),
            "block_state" => new EntityMetadataValue.BlockState(reader.ReadVarInt()),
            "optional_block_state" => new EntityMetadataValue.OptionalBlockState(ReadOptionalVarInt(ref reader) ?? 0),
            "compound_tag" => new EntityMetadataValue.CompoundTag(reader.ReadAnonymousNbtTag(protocolVersion)
                ?? throw new InvalidOperationException("compound_tag missing")),
            "particle" => new EntityMetadataValue.Particle(reader.ReadParticle(protocolVersion)),
            "particles" => new EntityMetadataValue.Particles(
                reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) => r.ReadParticle(protocolVersion))),
            "villager_data" => new EntityMetadataValue.VillagerData(reader.ReadVarInt(), reader.ReadVarInt(), reader.ReadVarInt()),
            "optional_unsigned_int" => new EntityMetadataValue.OptionalUnsignedInt(ReadOptionalVarInt(ref reader) ?? 0),
            "pose" => new EntityMetadataValue.Pose(reader.ReadVarInt()),
            "cat_variant" => new EntityMetadataValue.CatVariant(reader.ReadVarInt()),
            "wolf_variant" => new EntityMetadataValue.WolfVariantHolder(
                reader.ReadRegistryEntryHolder<EntityMetadataWolfVariant>(protocolVersion)),
            "frog_variant" => new EntityMetadataValue.FrogVariant(reader.ReadVarInt()),
            "optional_global_pos" => new EntityMetadataValue.OptionalGlobalPos(reader.ReadBoolean() ? reader.ReadString() : null),
            "painting_variant" => new EntityMetadataValue.PaintingVariant(
                reader.ReadRegistryEntryHolder<EntityMetadataPaintingVariant>(protocolVersion)),
            "sniffer_state" => new EntityMetadataValue.SnifferState(reader.ReadVarInt()),
            "armadillo_state" => new EntityMetadataValue.ArmadilloState(reader.ReadVarInt()),
            "vector3" => new EntityMetadataValue.Vector3(reader.ReadVec3f(protocolVersion)),
            "quaternion" => new EntityMetadataValue.Quaternion(reader.ReadVec4f(protocolVersion)),
            _ => throw new InvalidOperationException($"Unknown metadata type {type}.")
        };
    }

    private static void WriteLegacyMetadataValue(ref MinecraftPrimitiveWriter writer, int typeId, object value,
        int protocolVersion)
    {
        if (protocolVersion <= 759)
        {
            switch (typeId)
            {
                case 0: writer.WriteSignedByte((sbyte)value); return;
                case 1: writer.WriteVarInt((int)value); return;
                case 2: writer.WriteFloat((float)value); return;
                case 3: writer.WriteString((string)value); return;
                case 4: writer.WriteString((string)value); return;
                case 5: WriteOptionalString(ref writer, (string?)value); return;
                case 6: writer.WriteSlot((Slot)value, protocolVersion); return;
                case 7: writer.WriteBoolean((bool)value); return;
                case 8: WriteRotations(ref writer, (Rotations)value); return;
                case 9: writer.WritePosition((Position)value, protocolVersion); return;
                case 10: WriteOptionalPosition(ref writer, (Position?)value, protocolVersion); return;
                case 11: writer.WriteVarInt((int)value); return;
                case 12: WriteOptionalUuid(ref writer, (Guid?)value); return;
                case 13: writer.WriteVarInt((int)value); return;
                case 14: writer.WriteNbtTag((NbtTag)value, protocolVersion); return;
                case 15: writer.WriteParticle((Particle)value, protocolVersion); return;
                case 16: WriteVillagerData(ref writer, (VillagerData)value); return;
                case 17: WriteOptionalVarInt(ref writer, (int?)value); return;
                case 18: writer.WriteVarInt((int)value); return;
                default: throw new InvalidOperationException($"Unknown legacy metadata type {typeId}.");
            }
        }

        if (protocolVersion == 760)
        {
            switch (typeId)
            {
                case 0: writer.WriteSignedByte((sbyte)value); return;
                case 1: writer.WriteVarInt((int)value); return;
                case 2: writer.WriteFloat((float)value); return;
                case 3: writer.WriteString((string)value); return;
                case 4: writer.WriteString((string)value); return;
                case 5: WriteOptionalString(ref writer, (string?)value); return;
                case 6: writer.WriteSlot((Slot)value, protocolVersion); return;
                case 7: writer.WriteBoolean((bool)value); return;
                case 8: WriteRotations(ref writer, (Rotations)value); return;
                case 9: writer.WritePosition((Position)value, protocolVersion); return;
                case 10: WriteOptionalPosition(ref writer, (Position?)value, protocolVersion); return;
                case 11: writer.WriteVarInt((int)value); return;
                case 12: WriteOptionalUuid(ref writer, (Guid?)value); return;
                case 13: writer.WriteVarInt((int)value); return;
                case 14: writer.WriteNbtTag((NbtTag)value, protocolVersion); return;
                case 15: writer.WriteParticle((Particle)value, protocolVersion); return;
                case 16: WriteVillagerData(ref writer, (VillagerData)value); return;
                case 17: WriteOptionalVarInt(ref writer, (int?)value); return;
                case 18: writer.WriteVarInt((int)value); return;
                case 19: writer.WriteVarInt((int)value); return;
                case 20: writer.WriteVarInt((int)value); return;
                case 21: WriteOptionalString(ref writer, (string?)value); return;
                case 22: writer.WriteVarInt((int)value); return;
                default: throw new InvalidOperationException($"Unknown legacy metadata type {typeId}.");
            }
        }

        switch (typeId)
        {
            case 0: writer.WriteSignedByte((sbyte)value); return;
            case 1: writer.WriteVarInt((int)value); return;
            case 2: writer.WriteVarLong((long)value); return;
            case 3: writer.WriteFloat((float)value); return;
            case 4: writer.WriteString((string)value); return;
            case 5: writer.WriteString((string)value); return;
            case 6: WriteOptionalString(ref writer, (string?)value); return;
            case 7: writer.WriteSlot((Slot)value, protocolVersion); return;
            case 8: writer.WriteBoolean((bool)value); return;
            case 9: WriteRotations(ref writer, (Rotations)value); return;
            case 10: writer.WritePosition((Position)value, protocolVersion); return;
            case 11: WriteOptionalPosition(ref writer, (Position?)value, protocolVersion); return;
            case 12: writer.WriteVarInt((int)value); return;
            case 13: WriteOptionalUuid(ref writer, (Guid?)value); return;
            case 14: writer.WriteVarInt((int)value); return;
            case 15: writer.WriteNbtTag((NbtTag)value, protocolVersion); return;
            case 16: writer.WriteParticle((Particle)value, protocolVersion); return;
            case 17: WriteVillagerData(ref writer, (VillagerData)value); return;
            case 18: WriteOptionalVarInt(ref writer, (int?)value); return;
            case 19: writer.WriteVarInt((int)value); return;
            case 20: writer.WriteVarInt((int)value); return;
            case 21: writer.WriteVarInt((int)value); return;
            case 22: WriteOptionalString(ref writer, (string?)value); return;
            case 23: writer.WriteVarInt((int)value); return;
            default: throw new InvalidOperationException($"Unknown legacy metadata type {typeId}.");
        }
    }

    private static void WriteModernMetadataValue(ref MinecraftPrimitiveWriter writer, string type, object value,
        int protocolVersion)
    {
        switch (type)
        {
            case "byte": writer.WriteSignedByte((sbyte)value); return;
            case "int": writer.WriteVarInt((int)value); return;
            case "long": writer.WriteVarLong((long)value); return;
            case "float": writer.WriteFloat((float)value); return;
            case "string": writer.WriteString((string)value); return;
            case "component":
                if (protocolVersion >= 765)
                {
                    writer.WriteAnonymousNbtTag((NbtTag)value, protocolVersion);
                }
                else
                {
                    writer.WriteString((string)value);
                }
                return;
            case "optional_component":
                if (protocolVersion >= 765)
                {
                    WriteOptionalAnonymousNbt(ref writer, (NbtTag?)value, protocolVersion);
                }
                else
                {
                    WriteOptionalString(ref writer, (string?)value);
                }
                return;
            case "item_stack": writer.WriteSlot((Slot)value, protocolVersion); return;
            case "boolean": writer.WriteBoolean((bool)value); return;
            case "rotations": WriteRotations(ref writer, (Rotations)value); return;
            case "block_pos": writer.WritePosition((Position)value, protocolVersion); return;
            case "optional_block_pos": WriteOptionalPosition(ref writer, (Position?)value, protocolVersion); return;
            case "direction": writer.WriteVarInt((int)value); return;
            case "optional_uuid": WriteOptionalUuid(ref writer, (Guid?)value); return;
            case "block_state": writer.WriteVarInt((int)value); return;
            case "optional_block_state": WriteOptionalVarInt(ref writer, (int?)value); return;
            case "compound_tag":
                if (protocolVersion >= 764)
                {
                    writer.WriteAnonymousNbtTag((NbtTag)value, protocolVersion);
                }
                else
                {
                    writer.WriteNbtTag((NbtTag)value, protocolVersion);
                }
                return;
            case "particle": writer.WriteParticle((Particle)value, protocolVersion); return;
            case "villager_data": WriteVillagerData(ref writer, (VillagerData)value); return;
            case "optional_unsigned_int": WriteOptionalVarInt(ref writer, (int?)value); return;
            case "pose": writer.WriteVarInt((int)value); return;
            case "cat_variant": writer.WriteVarInt((int)value); return;
            case "frog_variant": writer.WriteVarInt((int)value); return;
            case "optional_global_pos": WriteOptionalString(ref writer, (string?)value); return;
            case "painting_variant": writer.WriteVarInt((int)value); return;
            case "sniffer_state": writer.WriteVarInt((int)value); return;
            case "vector3": writer.WriteVec3f((Vec3f)value, protocolVersion); return;
            case "quaternion": writer.WriteVec4f((Vec4f)value, protocolVersion); return;
            default: throw new InvalidOperationException($"Unknown metadata type {type}.");
        }
    }

    private static void WriteCurrentMetadataValue(ref MinecraftPrimitiveWriter writer, string type,
        EntityMetadataValue value, int protocolVersion)
    {
        switch (type)
        {
            case "byte" when value is EntityMetadataValue.Byte data:
                writer.WriteSignedByte(data.Value);
                return;
            case "int" when value is EntityMetadataValue.Int data:
                writer.WriteVarInt(data.Value);
                return;
            case "long" when value is EntityMetadataValue.Long data:
                writer.WriteVarLong(data.Value);
                return;
            case "float" when value is EntityMetadataValue.Float data:
                writer.WriteFloat(data.Value);
                return;
            case "string" when value is EntityMetadataValue.String data:
                writer.WriteString(data.Value);
                return;
            case "component" when value is EntityMetadataValue.Component data:
                writer.WriteAnonymousNbtTag(data.Value, protocolVersion);
                return;
            case "optional_component" when value is EntityMetadataValue.OptionalComponent data:
                WriteOptionalAnonymousNbt(ref writer, data.Value, protocolVersion);
                return;
            case "item_stack" when value is EntityMetadataValue.ItemStack data:
                writer.WriteSlot(data.Value, protocolVersion);
                return;
            case "boolean" when value is EntityMetadataValue.Boolean data:
                writer.WriteBoolean(data.Value);
                return;
            case "rotations" when value is EntityMetadataValue.Rotations data:
                WriteRotations(ref writer, new Rotations(data.Pitch, data.Yaw, data.Roll));
                return;
            case "block_pos" when value is EntityMetadataValue.BlockPos data:
                writer.WritePosition(data.Value, protocolVersion);
                return;
            case "optional_block_pos" when value is EntityMetadataValue.OptionalBlockPos data:
                WriteOptionalPosition(ref writer, data.Value, protocolVersion);
                return;
            case "direction" when value is EntityMetadataValue.Direction data:
                writer.WriteVarInt(data.Value);
                return;
            case "optional_uuid" when value is EntityMetadataValue.OptionalUuid data:
                WriteOptionalUuid(ref writer, data.Value);
                return;
            case "block_state" when value is EntityMetadataValue.BlockState data:
                writer.WriteVarInt(data.Value);
                return;
            case "optional_block_state" when value is EntityMetadataValue.OptionalBlockState data:
                WriteOptionalVarInt(ref writer, data.Value);
                return;
            case "compound_tag" when value is EntityMetadataValue.CompoundTag data:
                writer.WriteAnonymousNbtTag(data.Value, protocolVersion);
                return;
            case "particle" when value is EntityMetadataValue.Particle data:
                writer.WriteParticle(data.Value, protocolVersion);
                return;
            case "particles" when value is EntityMetadataValue.Particles data:
                writer.WriteVarInt(data.Values.Length);
                for (int i = 0; i < data.Values.Length; i++)
                {
                    writer.WriteParticle(data.Values[i], protocolVersion);
                }
                return;
            case "villager_data" when value is EntityMetadataValue.VillagerData data:
                WriteVillagerData(ref writer, new VillagerData(data.VillagerType, data.VillagerProfession, data.Level));
                return;
            case "optional_unsigned_int" when value is EntityMetadataValue.OptionalUnsignedInt data:
                WriteOptionalVarInt(ref writer, data.Value);
                return;
            case "pose" when value is EntityMetadataValue.Pose data:
                writer.WriteVarInt(data.Value);
                return;
            case "cat_variant" when value is EntityMetadataValue.CatVariant data:
                writer.WriteVarInt(data.Value);
                return;
            case "wolf_variant" when value is EntityMetadataValue.WolfVariantHolder data:
                writer.WriteRegistryEntryHolder(data.Value, protocolVersion);
                return;
            case "frog_variant" when value is EntityMetadataValue.FrogVariant data:
                writer.WriteVarInt(data.Value);
                return;
            case "optional_global_pos" when value is EntityMetadataValue.OptionalGlobalPos data:
                WriteOptionalString(ref writer, data.Value);
                return;
            case "painting_variant" when value is EntityMetadataValue.PaintingVariant data:
                writer.WriteRegistryEntryHolder(data.Value, protocolVersion);
                return;
            case "sniffer_state" when value is EntityMetadataValue.SnifferState data:
                writer.WriteVarInt(data.Value);
                return;
            case "armadillo_state" when value is EntityMetadataValue.ArmadilloState data:
                writer.WriteVarInt(data.Value);
                return;
            case "vector3" when value is EntityMetadataValue.Vector3 data:
                writer.WriteVec3f(data.Value, protocolVersion);
                return;
            case "quaternion" when value is EntityMetadataValue.Quaternion data:
                writer.WriteVec4f(data.Value, protocolVersion);
                return;
            default:
                throw new InvalidOperationException($"Unknown metadata type {type}.");
        }
    }

    private static string ReadModernMetadataType(int id)
    {
        if ((uint)id >= (uint)ModernMetadataTypeMapping.Length)
        {
            throw new InvalidOperationException($"Unknown metadata type id {id}.");
        }

        return ModernMetadataTypeMapping[id];
    }

    private static int GetModernMetadataTypeId(string name)
    {
        int id = Array.IndexOf(ModernMetadataTypeMapping, name);
        if (id < 0)
        {
            throw new InvalidOperationException($"Unknown metadata type {name}.");
        }
        return id;
    }

    private static string ReadCurrentMetadataType(int id)
    {
        if ((uint)id >= (uint)CurrentMetadataTypeMapping.Length)
        {
            throw new InvalidOperationException($"Unknown metadata type id {id}.");
        }

        return CurrentMetadataTypeMapping[id];
    }

    private static int GetCurrentMetadataTypeId(string name)
    {
        int id = Array.IndexOf(CurrentMetadataTypeMapping, name);
        if (id < 0)
        {
            throw new InvalidOperationException($"Unknown metadata type {name}.");
        }
        return id;
    }

    private static int? ReadOptionalVarInt(ref MinecraftPrimitiveReader reader)
    {
        int value = reader.ReadVarInt();
        return value == 0 ? null : value - 1;
    }

    private static void WriteOptionalVarInt(ref MinecraftPrimitiveWriter writer, int? value)
    {
        writer.WriteVarInt(value is null ? 0 : value.Value + 1);
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

    private static void WriteOptionalPosition(ref MinecraftPrimitiveWriter writer, Position? value, int protocolVersion)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WritePosition(value.Value, protocolVersion);
        }
    }

    private static void WriteOptionalAnonymousNbt(ref MinecraftPrimitiveWriter writer, NbtTag? value, int protocolVersion)
    {
        if (value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteAnonymousNbtTag(value, protocolVersion);
        }
    }

    private static void WriteRotations(ref MinecraftPrimitiveWriter writer, Rotations rotations)
    {
        writer.WriteFloat(rotations.Pitch);
        writer.WriteFloat(rotations.Yaw);
        writer.WriteFloat(rotations.Roll);
    }

    private static void WriteVillagerData(ref MinecraftPrimitiveWriter writer, VillagerData data)
    {
        writer.WriteVarInt(data.VillagerType);
        writer.WriteVarInt(data.VillagerProfession);
        writer.WriteVarInt(data.Level);
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_761Fields
    {
        public int EntityId { get; set; }
        public LegacyMetadataEntry[] Metadata { get; set; }
    }

    public struct V762_765Fields
    {
        public int EntityId { get; set; }
        public ModernMetadataEntry[] Metadata { get; set; }
    }

    public struct V766_LastFields
    {
        public int EntityId { get; set; }
        public EntityMetadataEntry[] Metadata { get; set; }
    }

    public struct LegacyMetadataEntry
    {
        public byte Key { get; set; }
        public int TypeId { get; set; }
        public object Value { get; set; }
    }

    public struct ModernMetadataEntry
    {
        public byte Key { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
    }

    public readonly record struct Rotations(float Pitch, float Yaw, float Roll);
    public readonly record struct VillagerData(int VillagerType, int VillagerProfession, int Level);

    private static readonly string[] ModernMetadataTypeMapping =
    {
        "byte", "int", "long", "float", "string", "component", "optional_component", "item_stack", "boolean",
        "rotations", "block_pos", "optional_block_pos", "direction", "optional_uuid", "block_state",
        "optional_block_state", "compound_tag", "particle", "villager_data", "optional_unsigned_int", "pose",
        "cat_variant", "frog_variant", "optional_global_pos", "painting_variant", "sniffer_state", "vector3",
        "quaternion"
    };

    private static readonly string[] CurrentMetadataTypeMapping =
    {
        "byte", "int", "long", "float", "string", "component", "optional_component", "item_stack", "boolean",
        "rotations", "block_pos", "optional_block_pos", "direction", "optional_uuid", "block_state",
        "optional_block_state", "compound_tag", "particle", "particles", "villager_data", "optional_unsigned_int",
        "pose", "cat_variant", "wolf_variant", "frog_variant", "optional_global_pos", "painting_variant",
        "sniffer_state", "armadillo_state", "vector3", "quaternion"
    };
}
