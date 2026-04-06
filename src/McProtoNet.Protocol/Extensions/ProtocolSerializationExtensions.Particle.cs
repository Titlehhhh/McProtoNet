using System;
using System.Collections.Generic;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    private static readonly string[] ParticleType_765 =
    {
        "angry_villager",
        "block",
        "block_marker",
        "bubble",
        "cloud",
        "crit",
        "damage_indicator",
        "dragon_breath",
        "dripping_lava",
        "falling_lava",
        "landing_lava",
        "dripping_water",
        "falling_water",
        "dust",
        "dust_color_transition",
        "effect",
        "elder_guardian",
        "enchanted_hit",
        "enchant",
        "end_rod",
        "entity_effect",
        "explosion_emitter",
        "explosion",
        "gust",
        "small_gust",
        "gust_emitter_large",
        "gust_emitter_small",
        "sonic_boom",
        "falling_dust",
        "firework",
        "fishing",
        "flame",
        "infested",
        "cherry_leaves",
        "sculk_soul",
        "sculk_charge",
        "sculk_charge_pop",
        "soul_fire_flame",
        "soul",
        "flash",
        "happy_villager",
        "composter",
        "heart",
        "instant_effect",
        "item",
        "vibration",
        "item_slime",
        "item_cobweb",
        "item_snowball",
        "large_smoke",
        "lava",
        "mycelium",
        "note",
        "poof",
        "portal",
        "rain",
        "smoke",
        "white_smoke",
        "sneeze",
        "spit",
        "squid_ink",
        "sweep_attack",
        "totem_of_undying",
        "underwater",
        "splash",
        "witch",
        "bubble_pop",
        "current_down",
        "bubble_column_up",
        "nautilus",
        "dolphin",
        "campfire_cosy_smoke",
        "campfire_signal_smoke",
        "dripping_honey",
        "falling_honey",
        "landing_honey",
        "falling_nectar",
        "falling_spore_blossom",
        "ash",
        "crimson_spore",
        "warped_spore",
        "spore_blossom_air",
        "dripping_obsidian_tear",
        "falling_obsidian_tear",
        "landing_obsidian_tear",
        "reverse_portal",
        "white_ash",
        "small_flame",
        "snowflake",
        "dripping_dripstone_lava",
        "falling_dripstone_lava",
        "dripping_dripstone_water",
        "falling_dripstone_water",
        "glow_squid_ink",
        "glow",
        "wax_on",
        "wax_off",
        "electric_spark",
        "scrape",
        "shriek",
        "egg_crack",
        "dust_plume",
        "trial_spawner_detected_player",
        "trial_spawner_detected_player_ominous",
        "vault_connection",
        "dust_pillar",
        "ominous_spawning",
        "raid_omen",
        "trial_omen"
    };

    private static readonly string[] ParticleType_766_767 =
    {
        "angry_villager",
        "block",
        "block_marker",
        "bubble",
        "cloud",
        "crit",
        "damage_indicator",
        "dragon_breath",
        "dripping_lava",
        "falling_lava",
        "landing_lava",
        "dripping_water",
        "falling_water",
        "dust",
        "dust_color_transition",
        "effect",
        "elder_guardian",
        "enchanted_hit",
        "enchant",
        "end_rod",
        "entity_effect",
        "explosion_emitter",
        "explosion",
        "gust",
        "small_gust",
        "gust_emitter_large",
        "gust_emitter_small",
        "sonic_boom",
        "falling_dust",
        "firework",
        "fishing",
        "flame",
        "infested",
        "cherry_leaves",
        "sculk_soul",
        "sculk_charge",
        "sculk_charge_pop",
        "soul_fire_flame",
        "soul",
        "flash",
        "happy_villager",
        "composter",
        "heart",
        "instant_effect",
        "item",
        "vibration",
        "item_slime",
        "item_cobweb",
        "item_snowball",
        "large_smoke",
        "lava",
        "mycelium",
        "note",
        "poof",
        "portal",
        "rain",
        "smoke",
        "white_smoke",
        "sneeze",
        "spit",
        "squid_ink",
        "sweep_attack",
        "totem_of_undying",
        "underwater",
        "splash",
        "witch",
        "bubble_pop",
        "current_down",
        "bubble_column_up",
        "nautilus",
        "dolphin",
        "campfire_cosy_smoke",
        "campfire_signal_smoke",
        "dripping_honey",
        "falling_honey",
        "landing_honey",
        "falling_nectar",
        "falling_spore_blossom",
        "ash",
        "crimson_spore",
        "warped_spore",
        "spore_blossom_air",
        "dripping_obsidian_tear",
        "falling_obsidian_tear",
        "landing_obsidian_tear",
        "reverse_portal",
        "white_ash",
        "small_flame",
        "snowflake",
        "dripping_dripstone_lava",
        "falling_dripstone_lava",
        "dripping_dripstone_water",
        "falling_dripstone_water",
        "glow_squid_ink",
        "glow",
        "wax_on",
        "wax_off",
        "electric_spark",
        "scrape",
        "shriek",
        "egg_crack",
        "dust_plume",
        "trial_spawner_detected_player",
        "trial_spawner_detected_player_ominous",
        "vault_connection",
        "dust_pillar",
        "ominous_spawning",
        "raid_omen",
        "trial_omen"
    };

    private static readonly string[] ParticleType_768 =
    {
        "angry_villager",
        "block",
        "block_marker",
        "bubble",
        "cloud",
        "crit",
        "damage_indicator",
        "dragon_breath",
        "dripping_lava",
        "falling_lava",
        "landing_lava",
        "dripping_water",
        "falling_water",
        "dust",
        "dust_color_transition",
        "effect",
        "elder_guardian",
        "enchanted_hit",
        "enchant",
        "end_rod",
        "entity_effect",
        "explosion_emitter",
        "explosion",
        "gust",
        "small_gust",
        "gust_emitter_large",
        "gust_emitter_small",
        "sonic_boom",
        "falling_dust",
        "firework",
        "fishing",
        "flame",
        "infested",
        "cherry_leaves",
        "sculk_soul",
        "sculk_charge",
        "sculk_charge_pop",
        "soul_fire_flame",
        "soul",
        "flash",
        "happy_villager",
        "composter",
        "heart",
        "instant_effect",
        "item",
        "vibration",
        "trail",
        "item_slime",
        "item_cobweb",
        "item_snowball",
        "large_smoke",
        "lava",
        "mycelium",
        "note",
        "poof",
        "portal",
        "rain",
        "smoke",
        "white_smoke",
        "sneeze",
        "spit",
        "squid_ink",
        "sweep_attack",
        "totem_of_undying",
        "underwater",
        "splash",
        "witch",
        "bubble_pop",
        "current_down",
        "bubble_column_up",
        "nautilus",
        "dolphin",
        "campfire_cosy_smoke",
        "campfire_signal_smoke",
        "dripping_honey",
        "falling_honey",
        "landing_honey",
        "falling_nectar",
        "falling_spore_blossom",
        "ash",
        "crimson_spore",
        "warped_spore",
        "spore_blossom_air",
        "dripping_obsidian_tear",
        "falling_obsidian_tear",
        "landing_obsidian_tear",
        "reverse_portal",
        "white_ash",
        "small_flame",
        "snowflake",
        "dripping_dripstone_lava",
        "falling_dripstone_lava",
        "dripping_dripstone_water",
        "falling_dripstone_water",
        "glow_squid_ink",
        "glow",
        "wax_on",
        "wax_off",
        "electric_spark",
        "scrape",
        "shriek",
        "egg_crack",
        "dust_plume",
        "trial_spawner_detected_player",
        "trial_spawner_detected_player_ominous",
        "vault_connection",
        "dust_pillar",
        "ominous_spawning",
        "raid_omen",
        "trial_omen",
        "block_crumble"
    };

    private static readonly string[] ParticleType_769 =
    {
        "angry_villager",
        "block",
        "block_marker",
        "bubble",
        "cloud",
        "crit",
        "damage_indicator",
        "dragon_breath",
        "dripping_lava",
        "falling_lava",
        "landing_lava",
        "dripping_water",
        "falling_water",
        "dust",
        "dust_color_transition",
        "effect",
        "elder_guardian",
        "enchanted_hit",
        "enchant",
        "end_rod",
        "entity_effect",
        "explosion_emitter",
        "explosion",
        "gust",
        "small_gust",
        "gust_emitter_large",
        "gust_emitter_small",
        "sonic_boom",
        "falling_dust",
        "firework",
        "fishing",
        "flame",
        "infested",
        "cherry_leaves",
        "pale_oak_leaves",
        "sculk_soul",
        "sculk_charge",
        "sculk_charge_pop",
        "soul_fire_flame",
        "soul",
        "flash",
        "happy_villager",
        "composter",
        "heart",
        "instant_effect",
        "item",
        "vibration",
        "trail",
        "item_slime",
        "item_cobweb",
        "item_snowball",
        "large_smoke",
        "lava",
        "mycelium",
        "note",
        "poof",
        "portal",
        "rain",
        "smoke",
        "white_smoke",
        "sneeze",
        "spit",
        "squid_ink",
        "sweep_attack",
        "totem_of_undying",
        "underwater",
        "splash",
        "witch",
        "bubble_pop",
        "current_down",
        "bubble_column_up",
        "nautilus",
        "dolphin",
        "campfire_cosy_smoke",
        "campfire_signal_smoke",
        "dripping_honey",
        "falling_honey",
        "landing_honey",
        "falling_nectar",
        "falling_spore_blossom",
        "ash",
        "crimson_spore",
        "warped_spore",
        "spore_blossom_air",
        "dripping_obsidian_tear",
        "falling_obsidian_tear",
        "landing_obsidian_tear",
        "reverse_portal",
        "white_ash",
        "small_flame",
        "snowflake",
        "dripping_dripstone_lava",
        "falling_dripstone_lava",
        "dripping_dripstone_water",
        "falling_dripstone_water",
        "glow_squid_ink",
        "glow",
        "wax_on",
        "wax_off",
        "electric_spark",
        "scrape",
        "shriek",
        "egg_crack",
        "dust_plume",
        "trial_spawner_detected_player",
        "trial_spawner_detected_player_ominous",
        "vault_connection",
        "dust_pillar",
        "ominous_spawning",
        "raid_omen",
        "trial_omen",
        "block_crumble"
    };

    private static readonly string[] ParticleType_770_772 =
    {
        "angry_villager",
        "block",
        "block_marker",
        "bubble",
        "cloud",
        "crit",
        "damage_indicator",
        "dragon_breath",
        "dripping_lava",
        "falling_lava",
        "landing_lava",
        "dripping_water",
        "falling_water",
        "dust",
        "dust_color_transition",
        "effect",
        "elder_guardian",
        "enchanted_hit",
        "enchant",
        "end_rod",
        "entity_effect",
        "explosion_emitter",
        "explosion",
        "gust",
        "small_gust",
        "gust_emitter_large",
        "gust_emitter_small",
        "sonic_boom",
        "falling_dust",
        "firework",
        "fishing",
        "flame",
        "infested",
        "cherry_leaves",
        "pale_oak_leaves",
        "tinted_leaves",
        "sculk_soul",
        "sculk_charge",
        "sculk_charge_pop",
        "soul_fire_flame",
        "soul",
        "flash",
        "happy_villager",
        "composter",
        "heart",
        "instant_effect",
        "item",
        "vibration",
        "trail",
        "item_slime",
        "item_cobweb",
        "item_snowball",
        "large_smoke",
        "lava",
        "mycelium",
        "note",
        "poof",
        "portal",
        "rain",
        "smoke",
        "white_smoke",
        "sneeze",
        "spit",
        "squid_ink",
        "sweep_attack",
        "totem_of_undying",
        "underwater",
        "splash",
        "witch",
        "bubble_pop",
        "current_down",
        "bubble_column_up",
        "nautilus",
        "dolphin",
        "campfire_cosy_smoke",
        "campfire_signal_smoke",
        "dripping_honey",
        "falling_honey",
        "landing_honey",
        "falling_nectar",
        "falling_spore_blossom",
        "ash",
        "crimson_spore",
        "warped_spore",
        "spore_blossom_air",
        "dripping_obsidian_tear",
        "falling_obsidian_tear",
        "landing_obsidian_tear",
        "reverse_portal",
        "white_ash",
        "small_flame",
        "snowflake",
        "dripping_dripstone_lava",
        "falling_dripstone_lava",
        "dripping_dripstone_water",
        "falling_dripstone_water",
        "glow_squid_ink",
        "glow",
        "wax_on",
        "wax_off",
        "electric_spark",
        "scrape",
        "shriek",
        "egg_crack",
        "dust_plume",
        "trial_spawner_detected_player",
        "trial_spawner_detected_player_ominous",
        "vault_connection",
        "dust_pillar",
        "ominous_spawning",
        "raid_omen",
        "trial_omen",
        "block_crumble",
        "firefly"
    };

    private static readonly Dictionary<string, int> ParticleTypeId_765 = BuildParticleTypeLookup(ParticleType_765);
    private static readonly Dictionary<string, int> ParticleTypeId_766_767 = BuildParticleTypeLookup(ParticleType_766_767);
    private static readonly Dictionary<string, int> ParticleTypeId_768 = BuildParticleTypeLookup(ParticleType_768);
    private static readonly Dictionary<string, int> ParticleTypeId_769 = BuildParticleTypeLookup(ParticleType_769);
    private static readonly Dictionary<string, int> ParticleTypeId_770_772 = BuildParticleTypeLookup(ParticleType_770_772);

    public static Particle ReadParticle(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Particle>(protocolVersion);
        if (protocolVersion <= 764)
        {
            int particleId = reader.ReadVarInt();
            ParticleData? data = reader.ReadParticleData(protocolVersion, particleId);
            return new Particle(particleId, data, null, null);
        }

        string type = ReadParticleType(reader.ReadVarInt(), protocolVersion);
        if (protocolVersion == 765)
        {
            return new Particle(null, null, type, null);
        }

        ParticlePayload? payload = ReadParticlePayload(ref reader, protocolVersion, type);
        return new Particle(null, null, type, payload);
    }

    public static void WriteParticle(this MinecraftPrimitiveWriter writer, Particle value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Particle>(protocolVersion);
        if (protocolVersion <= 764)
        {
            int particleId = value.ParticleId ?? throw new InvalidOperationException("ParticleId missing.");
            writer.WriteVarInt(particleId);
            writer.WriteParticleData(protocolVersion, particleId, value.LegacyData);
            return;
        }

        string type = value.Type ?? throw new InvalidOperationException("Particle type missing.");
        writer.WriteVarInt(WriteParticleType(type, protocolVersion));
        if (protocolVersion == 765)
        {
            return;
        }
        WriteParticlePayload(writer, protocolVersion, type, value.Data);
    }

    public static ParticleData? ReadParticleData(this ref MinecraftPrimitiveReader reader, int protocolVersion, int particleId)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ParticleData>(protocolVersion);
        if (protocolVersion <= 754)
        {
            return particleId switch
            {
                3 or 23 => new ParticleData.BlockState(reader.ReadVarInt()),
                14 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                34 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
                _ => null
            };
        }
        if (protocolVersion <= 756)
        {
            return particleId switch
            {
                4 or 25 => new ParticleData.BlockState(reader.ReadVarInt()),
                15 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                16 => new ParticleData.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                    reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                36 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
                37 => new ParticleData.LegacyVibration(ReadLegacyVibration(ref reader, protocolVersion)),
                _ => null
            };
        }
        if (protocolVersion <= 758)
        {
            return particleId switch
            {
                2 or 3 or 24 => new ParticleData.BlockState(reader.ReadVarInt()),
                14 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                15 => new ParticleData.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                    reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                35 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
                36 => new ParticleData.LegacyVibration(ReadLegacyVibration(ref reader, protocolVersion)),
                _ => null
            };
        }
        if (protocolVersion <= 761)
        {
            return particleId switch
            {
                2 or 3 or 25 => new ParticleData.BlockState(reader.ReadVarInt()),
                14 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                15 => new ParticleData.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                    reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                30 => new ParticleData.Rotation(reader.ReadFloat()),
                39 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
                40 => new ParticleData.Vibration(ReadVibration(ref reader, protocolVersion)),
                92 => new ParticleData.Delay(reader.ReadVarInt()),
                _ => null
            };
        }
        if (protocolVersion == 762)
        {
            return particleId switch
            {
                2 or 3 or 25 => new ParticleData.BlockState(reader.ReadVarInt()),
                14 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                15 => new ParticleData.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                    reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                33 => new ParticleData.Rotation(reader.ReadFloat()),
                42 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
                43 => new ParticleData.Vibration(ReadVibration(ref reader, protocolVersion)),
                95 => new ParticleData.Delay(reader.ReadVarInt()),
                _ => null
            };
        }
        if (protocolVersion <= 764)
        {
            return particleId switch
            {
                2 or 3 or 25 => new ParticleData.BlockState(reader.ReadVarInt()),
                14 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                15 => new ParticleData.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                    reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
                31 => new ParticleData.Rotation(reader.ReadFloat()),
                40 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
                41 => new ParticleData.Vibration(ReadVibration(ref reader, protocolVersion)),
                93 => new ParticleData.Delay(reader.ReadVarInt()),
                _ => null
            };
        }

        return particleId switch
        {
            2 or 3 or 27 => new ParticleData.BlockState(reader.ReadVarInt()),
            14 => new ParticleData.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            15 => new ParticleData.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(),
                reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            33 => new ParticleData.Rotation(reader.ReadFloat()),
            42 => new ParticleData.Item(reader.ReadSlot(protocolVersion)),
            43 => new ParticleData.Vibration(ReadVibration(ref reader, protocolVersion)),
            96 => new ParticleData.Delay(reader.ReadVarInt()),
            _ => null
        };
    }

    public static void WriteParticleData(this MinecraftPrimitiveWriter writer, int protocolVersion, int particleId,
        ParticleData? data)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ParticleData>(protocolVersion);
        switch (data)
        {
            case null:
                return;
            case ParticleData.BlockState blockState:
                writer.WriteVarInt(blockState.Value);
                return;
            case ParticleData.Dust dust:
                writer.WriteFloat(dust.Red);
                writer.WriteFloat(dust.Green);
                writer.WriteFloat(dust.Blue);
                writer.WriteFloat(dust.Scale);
                return;
            case ParticleData.DustColorTransition transition:
                writer.WriteFloat(transition.FromRed);
                writer.WriteFloat(transition.FromGreen);
                writer.WriteFloat(transition.FromBlue);
                writer.WriteFloat(transition.Scale);
                writer.WriteFloat(transition.ToRed);
                writer.WriteFloat(transition.ToGreen);
                writer.WriteFloat(transition.ToBlue);
                return;
            case ParticleData.Item item:
                writer.WriteSlot(item.ItemStack, protocolVersion);
                return;
            case ParticleData.LegacyVibration legacyVibration:
                WriteLegacyVibration(writer, legacyVibration.Data, protocolVersion);
                return;
            case ParticleData.Vibration vibration:
                WriteVibration(writer, vibration.Data, protocolVersion);
                return;
            case ParticleData.Rotation rotation:
                writer.WriteFloat(rotation.Value);
                return;
            case ParticleData.Delay delay:
                writer.WriteVarInt(delay.DelayInTicksBeforeShown);
                return;
            default:
                throw new InvalidOperationException($"Unhandled ParticleData for particle id {particleId}");
        }
    }

    private static ParticlePayload? ReadParticlePayload(ref MinecraftPrimitiveReader reader, int protocolVersion, string type)
    {
        return type switch
        {
            "block" => new ParticlePayload.Block(reader.ReadVarInt()),
            "block_marker" => new ParticlePayload.BlockMarker(reader.ReadVarInt()),
            "falling_dust" => new ParticlePayload.FallingDust(reader.ReadVarInt()),
            "dust_pillar" => new ParticlePayload.DustPillar(reader.ReadVarInt()),
            "block_crumble" when protocolVersion >= 768 => new ParticlePayload.BlockCrumble(reader.ReadVarInt()),
            "dust" => new ParticlePayload.Dust(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            "dust_color_transition" => new ParticlePayload.DustColorTransition(reader.ReadFloat(), reader.ReadFloat(),
                reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()),
            "entity_effect" => new ParticlePayload.EntityEffect(reader.ReadSignedInt()),
            "item" => new ParticlePayload.Item(reader.ReadSlot(protocolVersion)),
            "sculk_charge" => new ParticlePayload.SculkCharge(reader.ReadFloat()),
            "shriek" => new ParticlePayload.Shriek(reader.ReadVarInt()),
            "vibration" => new ParticlePayload.Vibration(ReadParticleVibration(ref reader, protocolVersion)),
            "trail" when protocolVersion >= 768 => new ParticlePayload.Trail(ReadParticleTrail(ref reader, protocolVersion)),
            "tinted_leaves" when protocolVersion >= 770 => new ParticlePayload.TintedLeaves(reader.ReadSignedInt()),
            "firefly" when protocolVersion >= 770 => new ParticlePayload.Firefly(),
            _ => null
        };
    }

    private static void WriteParticlePayload(MinecraftPrimitiveWriter writer, int protocolVersion, string type,
        ParticlePayload? payload)
    {
        switch (type)
        {
            case "block" when payload is ParticlePayload.Block data:
                writer.WriteVarInt(data.BlockState);
                break;
            case "block_marker" when payload is ParticlePayload.BlockMarker data:
                writer.WriteVarInt(data.BlockState);
                break;
            case "falling_dust" when payload is ParticlePayload.FallingDust data:
                writer.WriteVarInt(data.BlockState);
                break;
            case "dust_pillar" when payload is ParticlePayload.DustPillar data:
                writer.WriteVarInt(data.BlockState);
                break;
            case "block_crumble" when payload is ParticlePayload.BlockCrumble data:
                writer.WriteVarInt(data.BlockState);
                break;
            case "dust" when payload is ParticlePayload.Dust data:
                writer.WriteFloat(data.Red);
                writer.WriteFloat(data.Green);
                writer.WriteFloat(data.Blue);
                writer.WriteFloat(data.Scale);
                break;
            case "dust_color_transition" when payload is ParticlePayload.DustColorTransition data:
                writer.WriteFloat(data.FromRed);
                writer.WriteFloat(data.FromGreen);
                writer.WriteFloat(data.FromBlue);
                writer.WriteFloat(data.Scale);
                writer.WriteFloat(data.ToRed);
                writer.WriteFloat(data.ToGreen);
                writer.WriteFloat(data.ToBlue);
                break;
            case "entity_effect" when payload is ParticlePayload.EntityEffect data:
                writer.WriteSignedInt(data.Color);
                break;
            case "item" when payload is ParticlePayload.Item data:
                writer.WriteSlot(data.ItemStack, protocolVersion);
                break;
            case "sculk_charge" when payload is ParticlePayload.SculkCharge data:
                writer.WriteFloat(data.Value);
                break;
            case "shriek" when payload is ParticlePayload.Shriek data:
                writer.WriteVarInt(data.Delay);
                break;
            case "vibration" when payload is ParticlePayload.Vibration data:
                WriteParticleVibration(writer, data.Data, protocolVersion);
                break;
            case "trail" when payload is ParticlePayload.Trail data:
                WriteParticleTrail(writer, data.Data, protocolVersion);
                break;
            case "tinted_leaves" when payload is ParticlePayload.TintedLeaves data:
                writer.WriteSignedInt(data.Color);
                break;
            case "firefly":
                break;
            default:
                if (payload is not null)
                {
                    throw new InvalidOperationException($"Unexpected particle payload for {type}");
                }
                break;
        }
    }

    private static ParticlePayload.ParticleVibrationData ReadParticleVibration(ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        string positionType = ReadParticleVibrationPositionType(reader.ReadVarInt());
        Position? blockPosition = null;
        int? entityId = null;
        float? entityEyeHeight = null;
        if (positionType == "block")
        {
            blockPosition = reader.ReadPosition(protocolVersion);
        }
        else
        {
            entityId = reader.ReadVarInt();
            entityEyeHeight = reader.ReadFloat();
        }
        int ticks = reader.ReadVarInt();
        return new ParticlePayload.ParticleVibrationData(positionType, blockPosition, entityId, entityEyeHeight, ticks);
    }

    private static void WriteParticleVibration(MinecraftPrimitiveWriter writer, ParticlePayload.ParticleVibrationData data,
        int protocolVersion)
    {
        writer.WriteVarInt(WriteParticleVibrationPositionType(data.PositionType));
        if (data.PositionType == "block")
        {
            writer.WritePosition(data.BlockPosition ?? throw new InvalidOperationException("block position missing"),
                protocolVersion);
        }
        else
        {
            writer.WriteVarInt(data.EntityId ?? 0);
            writer.WriteFloat(data.EntityEyeHeight ?? 0f);
        }
        writer.WriteVarInt(data.Ticks);
    }

    private static ParticlePayload.ParticleTrailData ReadParticleTrail(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Vec3f64 target = reader.ReadVec3f64(protocolVersion);
        byte color = reader.ReadUnsignedByte();
        return new ParticlePayload.ParticleTrailData(target, color);
    }

    private static void WriteParticleTrail(MinecraftPrimitiveWriter writer, ParticlePayload.ParticleTrailData data,
        int protocolVersion)
    {
        writer.WriteVec3f64(data.Target, protocolVersion);
        writer.WriteUnsignedByte(data.Color);
    }

    private static ParticleData.LegacyVibrationData ReadLegacyVibration(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Position origin = reader.ReadPosition(protocolVersion);
        string positionType = reader.ReadString();
        Position? destinationBlock = null;
        int? destinationEntityId = null;
        if (positionType == "minecraft:block")
        {
            destinationBlock = reader.ReadPosition(protocolVersion);
        }
        else if (positionType == "minecraft:entity")
        {
            destinationEntityId = reader.ReadVarInt();
        }
        int ticks = reader.ReadVarInt();
        return new ParticleData.LegacyVibrationData(origin, positionType, destinationBlock, destinationEntityId, ticks);
    }

    private static void WriteLegacyVibration(MinecraftPrimitiveWriter writer, ParticleData.LegacyVibrationData data,
        int protocolVersion)
    {
        writer.WritePosition(data.Origin, protocolVersion);
        writer.WriteString(data.PositionType);
        if (data.PositionType == "minecraft:block")
        {
            writer.WritePosition(data.DestinationBlock ?? throw new InvalidOperationException("destination block missing"),
                protocolVersion);
        }
        else
        {
            writer.WriteVarInt(data.DestinationEntityId ?? 0);
        }
        writer.WriteVarInt(data.Ticks);
    }

    private static ParticleData.VibrationData ReadVibration(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string positionType = reader.ReadString();
        int? entityId = null;
        int? entityEyeHeight = null;
        if (positionType == "minecraft:entity")
        {
            entityId = reader.ReadVarInt();
            entityEyeHeight = reader.ReadVarInt();
        }
        Position? destinationBlock = null;
        int? destinationEntityId = null;
        if (positionType == "minecraft:block")
        {
            destinationBlock = reader.ReadPosition(protocolVersion);
        }
        else if (positionType == "minecraft:entity")
        {
            destinationEntityId = reader.ReadVarInt();
        }
        int ticks = reader.ReadVarInt();
        return new ParticleData.VibrationData(positionType, entityId, entityEyeHeight, destinationBlock, destinationEntityId, ticks);
    }

    private static void WriteVibration(MinecraftPrimitiveWriter writer, ParticleData.VibrationData data,
        int protocolVersion)
    {
        writer.WriteString(data.PositionType);
        if (data.PositionType == "minecraft:entity")
        {
            writer.WriteVarInt(data.EntityId ?? 0);
            writer.WriteVarInt(data.EntityEyeHeight ?? 0);
        }
        if (data.PositionType == "minecraft:block")
        {
            writer.WritePosition(data.DestinationBlock ?? throw new InvalidOperationException("destination block missing"),
                protocolVersion);
        }
        else if (data.PositionType == "minecraft:entity")
        {
            writer.WriteVarInt(data.DestinationEntityId ?? 0);
        }
        writer.WriteVarInt(data.Ticks);
    }

    private static string ReadParticleType(int id, int protocolVersion)
    {
        string[] mapping = protocolVersion switch
        {
            765 => ParticleType_765,
            766 or 767 => ParticleType_766_767,
            768 => ParticleType_768,
            769 => ParticleType_769,
            _ => ParticleType_770_772
        };
        if (id < 0 || id >= mapping.Length)
        {
            throw new InvalidOperationException($"Unknown particle type id {id}");
        }
        return mapping[id];
    }

    private static int WriteParticleType(string value, int protocolVersion)
    {
        Dictionary<string, int> mapping = protocolVersion switch
        {
            765 => ParticleTypeId_765,
            766 or 767 => ParticleTypeId_766_767,
            768 => ParticleTypeId_768,
            769 => ParticleTypeId_769,
            _ => ParticleTypeId_770_772
        };
        if (!mapping.TryGetValue(value, out int id))
        {
            throw new InvalidOperationException($"Unknown particle type {value}");
        }
        return id;
    }

    private static string ReadParticleVibrationPositionType(int id)
    {
        return id switch
        {
            0 => "block",
            1 => "entity",
            _ => throw new InvalidOperationException($"Unknown vibration position type id {id}")
        };
    }

    private static int WriteParticleVibrationPositionType(string value)
    {
        return value switch
        {
            "block" => 0,
            "entity" => 1,
            _ => throw new InvalidOperationException($"Unknown vibration position type {value}")
        };
    }

    private static Dictionary<string, int> BuildParticleTypeLookup(string[] values)
    {
        var map = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < values.Length; i++)
        {
            map[values[i]] = i;
        }
        return map;
    }
}
