using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityUpdateAttributes", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EntityUpdateAttributesPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, 765),
        new(766, MinecraftVersion.LatestProtocol)
    };

    public int EntityId { get; set; }

    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_765Fields? V755_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("EntityUpdateAttributes VFirst_754 missing.");
                writer.WriteVarInt(EntityId);
                WritePropertiesInt(writer, fields.Properties);
                return;
            }
            case >= 755 and <= 765:
            {
                var fields = V755_765 ?? throw new InvalidOperationException("EntityUpdateAttributes V755_765 missing.");
                writer.WriteVarInt(EntityId);
                WritePropertiesVarInt(writer, fields.Properties);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("EntityUpdateAttributes V766_Last missing.");
                writer.WriteVarInt(EntityId);
                WriteMappedProperties(writer, fields.Properties);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityUpdateAttributes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                EntityId = reader.ReadVarInt();
                VFirst_754 = new VFirst_754Fields
                {
                    Properties = ReadPropertiesInt(ref reader)
                };
                return;
            }
            case >= 755 and <= 765:
            {
                EntityId = reader.ReadVarInt();
                V755_765 = new V755_765Fields
                {
                    Properties = ReadPropertiesVarInt(ref reader)
                };
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                EntityId = reader.ReadVarInt();
                V766_Last = new V766_LastFields
                {
                    Properties = ReadMappedProperties(ref reader)
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EntityUpdateAttributes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    private static PropertyEntryVFirst_754[] ReadPropertiesInt(ref MinecraftPrimitiveReader reader)
    {
        int length = reader.ReadSignedInt();
        if (length == 0)
        {
            return Array.Empty<PropertyEntryVFirst_754>();
        }

        var entries = new PropertyEntryVFirst_754[length];
        for (int i = 0; i < entries.Length; i++)
        {
            string key = reader.ReadString();
            double value = reader.ReadDouble();
            int modifierCount = reader.ReadVarInt();
            var modifiers = modifierCount == 0 ? Array.Empty<ModifierEntry>() : new ModifierEntry[modifierCount];
            for (int m = 0; m < modifiers.Length; m++)
            {
                modifiers[m] = new ModifierEntry
                {
                    Uuid = reader.ReadUUID(),
                    Amount = reader.ReadDouble(),
                    Operation = reader.ReadSignedByte()
                };
            }

            entries[i] = new PropertyEntryVFirst_754
            {
                Key = key,
                Value = value,
                Modifiers = modifiers
            };
        }

        return entries;
    }

    private static PropertyEntryV755_765[] ReadPropertiesVarInt(ref MinecraftPrimitiveReader reader)
    {
        int length = reader.ReadVarInt();
        if (length == 0)
        {
            return Array.Empty<PropertyEntryV755_765>();
        }

        var entries = new PropertyEntryV755_765[length];
        for (int i = 0; i < entries.Length; i++)
        {
            string name = reader.ReadString();
            double value = reader.ReadDouble();
            int modifierCount = reader.ReadVarInt();
            var modifiers = modifierCount == 0 ? Array.Empty<ModifierEntry>() : new ModifierEntry[modifierCount];
            for (int m = 0; m < modifiers.Length; m++)
            {
                modifiers[m] = new ModifierEntry
                {
                    Uuid = reader.ReadUUID(),
                    Amount = reader.ReadDouble(),
                    Operation = reader.ReadSignedByte()
                };
            }

            entries[i] = new PropertyEntryV755_765
            {
                Name = name,
                Value = value,
                Modifiers = modifiers
            };
        }

        return entries;
    }

    private static PropertyEntryV766_Last[] ReadMappedProperties(ref MinecraftPrimitiveReader reader)
    {
        int length = reader.ReadVarInt();
        if (length == 0)
        {
            return Array.Empty<PropertyEntryV766_Last>();
        }

        var entries = new PropertyEntryV766_Last[length];
        for (int i = 0; i < entries.Length; i++)
        {
            string key = ReadAttributeName(reader.ReadVarInt());
            double value = reader.ReadDouble();
            int modifierCount = reader.ReadVarInt();
            var modifiers = modifierCount == 0 ? Array.Empty<ModifierEntry>() : new ModifierEntry[modifierCount];
            for (int m = 0; m < modifiers.Length; m++)
            {
                modifiers[m] = new ModifierEntry
                {
                    Uuid = reader.ReadUUID(),
                    Amount = reader.ReadDouble(),
                    Operation = reader.ReadSignedByte()
                };
            }

            entries[i] = new PropertyEntryV766_Last
            {
                Key = key,
                Value = value,
                Modifiers = modifiers
            };
        }

        return entries;
    }

    private static void WritePropertiesInt(MinecraftPrimitiveWriter writer, PropertyEntryVFirst_754[] properties)
    {
        writer.WriteSignedInt(properties.Length);
        for (int i = 0; i < properties.Length; i++)
        {
            writer.WriteString(properties[i].Key);
            writer.WriteDouble(properties[i].Value);
            WriteModifiers(writer, properties[i].Modifiers);
        }
    }

    private static void WritePropertiesVarInt(MinecraftPrimitiveWriter writer, PropertyEntryV755_765[] properties)
    {
        writer.WriteVarInt(properties.Length);
        for (int i = 0; i < properties.Length; i++)
        {
            writer.WriteString(properties[i].Name);
            writer.WriteDouble(properties[i].Value);
            WriteModifiers(writer, properties[i].Modifiers);
        }
    }

    private static void WriteMappedProperties(MinecraftPrimitiveWriter writer, PropertyEntryV766_Last[] properties)
    {
        writer.WriteVarInt(properties.Length);
        for (int i = 0; i < properties.Length; i++)
        {
            writer.WriteVarInt(WriteAttributeName(properties[i].Key));
            writer.WriteDouble(properties[i].Value);
            WriteModifiers(writer, properties[i].Modifiers);
        }
    }

    private static void WriteModifiers(MinecraftPrimitiveWriter writer, ModifierEntry[] modifiers)
    {
        writer.WriteVarInt(modifiers.Length);
        for (int i = 0; i < modifiers.Length; i++)
        {
            writer.WriteUUID(modifiers[i].Uuid);
            writer.WriteDouble(modifiers[i].Amount);
            writer.WriteSignedByte(modifiers[i].Operation);
        }
    }

    private static string ReadAttributeName(int id)
    {
        if ((uint)id >= (uint)AttributeIdToName.Length)
        {
            throw new InvalidOperationException($"Unknown attribute id {id}.");
        }

        return AttributeIdToName[id];
    }

    private static int WriteAttributeName(string name)
    {
        return name switch
        {
            "generic.armor" => 0,
            "generic.armor_toughness" => 1,
            "generic.attack_damage" => 2,
            "generic.attack_knockback" => 3,
            "generic.attack_speed" => 4,
            "player.block_break_speed" => 5,
            "player.block_interaction_range" => 6,
            "player.entity_interaction_range" => 7,
            "generic.fall_damage_multiplier" => 8,
            "generic.flying_speed" => 9,
            "generic.follow_range" => 10,
            "generic.gravity" => 11,
            "generic.jump_strength" => 12,
            "generic.knockback_resistance" => 13,
            "generic.luck" => 14,
            "generic.max_absorption" => 15,
            "generic.max_health" => 16,
            "generic.movement_speed" => 17,
            "generic.safe_fall_distance" => 18,
            "generic.scale" => 19,
            "zombie.spawn_reinforcements" => 20,
            "generic.step_height" => 21,
            _ => throw new InvalidOperationException($"Unknown attribute name {name}.")
        };
    }

    private static readonly string[] AttributeIdToName =
    {
        "generic.armor",
        "generic.armor_toughness",
        "generic.attack_damage",
        "generic.attack_knockback",
        "generic.attack_speed",
        "player.block_break_speed",
        "player.block_interaction_range",
        "player.entity_interaction_range",
        "generic.fall_damage_multiplier",
        "generic.flying_speed",
        "generic.follow_range",
        "generic.gravity",
        "generic.jump_strength",
        "generic.knockback_resistance",
        "generic.luck",
        "generic.max_absorption",
        "generic.max_health",
        "generic.movement_speed",
        "generic.safe_fall_distance",
        "generic.scale",
        "zombie.spawn_reinforcements",
        "generic.step_height"
    };

    public struct VFirst_754Fields
    {
        public PropertyEntryVFirst_754[] Properties { get; set; }
    }

    public struct V755_765Fields
    {
        public PropertyEntryV755_765[] Properties { get; set; }
    }

    public struct V766_LastFields
    {
        public PropertyEntryV766_Last[] Properties { get; set; }
    }

    public struct PropertyEntryVFirst_754
    {
        public string Key { get; set; }
        public double Value { get; set; }
        public ModifierEntry[] Modifiers { get; set; }
    }

    public struct PropertyEntryV755_765
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public ModifierEntry[] Modifiers { get; set; }
    }

    public struct PropertyEntryV766_Last
    {
        public string Key { get; set; }
        public double Value { get; set; }
        public ModifierEntry[] Modifiers { get; set; }
    }

    public struct ModifierEntry
    {
        public Guid Uuid { get; set; }
        public double Amount { get; set; }
        public sbyte Operation { get; set; }
    }
}
