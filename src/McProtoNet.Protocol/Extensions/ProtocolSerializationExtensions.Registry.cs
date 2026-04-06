using System;
using McProtoNet.Serialization;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static RegistryEntryHolder<T> ReadRegistryEntryHolder<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        bool hasInline = reader.ReadBoolean();
        if (!hasInline)
        {
            int registryId = reader.ReadVarInt();
            return new RegistryEntryHolder<T>(registryId);
        }

        T inlineValue = ReadRegistryEntryHolderInline<T>(ref reader, protocolVersion);
        return new RegistryEntryHolder<T>(inlineValue);
    }

    public static void WriteRegistryEntryHolder<T>(this MinecraftPrimitiveWriter writer, RegistryEntryHolder<T> value,
        int protocolVersion)
    {
        writer.WriteBoolean(value.HasInline);
        if (!value.HasInline)
        {
            writer.WriteVarInt(value.RegistryId ?? 0);
            return;
        }

        WriteRegistryEntryHolderInline(writer, value.Inline, protocolVersion);
    }

    private static T ReadRegistryEntryHolderInline<T>(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)reader.ReadString();
        }
        if (typeof(T) == typeof(int))
        {
            return (T)(object)reader.ReadVarInt();
        }
        if (typeof(T) == typeof(ArmorTrimMaterial))
        {
            return (T)(object)reader.ReadArmorTrimMaterial(protocolVersion);
        }
        if (typeof(T) == typeof(ArmorTrimPattern))
        {
            return (T)(object)reader.ReadArmorTrimPattern(protocolVersion);
        }
        if (typeof(T) == typeof(BannerPattern))
        {
            return (T)(object)reader.ReadBannerPattern(protocolVersion);
        }
        if (typeof(T) == typeof(EntityMetadataPaintingVariant))
        {
            return (T)(object)reader.ReadEntityMetadataPaintingVariant(protocolVersion);
        }
        if (typeof(T) == typeof(EntityMetadataWolfVariant))
        {
            return (T)(object)reader.ReadEntityMetadataWolfVariant(protocolVersion);
        }
        if (typeof(T) == typeof(InstrumentData))
        {
            return (T)(object)reader.ReadInstrumentData(protocolVersion);
        }
        if (typeof(T) == typeof(ItemSoundEvent))
        {
            return (T)(object)reader.ReadItemSoundEvent(protocolVersion);
        }
        if (typeof(T) == typeof(JukeboxSongData))
        {
            return (T)(object)reader.ReadJukeboxSongData(protocolVersion);
        }

        throw new NotImplementedException($"Inline registryEntryHolder<{typeof(T).Name}> is not implemented.");
    }

    private static void WriteRegistryEntryHolderInline<T>(MinecraftPrimitiveWriter writer, T? value, int protocolVersion)
    {
        if (value is null)
        {
            throw new InvalidOperationException("registryEntryHolder inline value missing.");
        }

        if (typeof(T) == typeof(string))
        {
            writer.WriteString((string)(object)value);
            return;
        }
        if (typeof(T) == typeof(int))
        {
            writer.WriteVarInt((int)(object)value);
            return;
        }
        if (typeof(T) == typeof(ArmorTrimMaterial))
        {
            writer.WriteArmorTrimMaterial((ArmorTrimMaterial)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(ArmorTrimPattern))
        {
            writer.WriteArmorTrimPattern((ArmorTrimPattern)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(BannerPattern))
        {
            writer.WriteBannerPattern((BannerPattern)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(EntityMetadataPaintingVariant))
        {
            writer.WriteEntityMetadataPaintingVariant((EntityMetadataPaintingVariant)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(EntityMetadataWolfVariant))
        {
            writer.WriteEntityMetadataWolfVariant((EntityMetadataWolfVariant)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(InstrumentData))
        {
            writer.WriteInstrumentData((InstrumentData)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(ItemSoundEvent))
        {
            writer.WriteItemSoundEvent((ItemSoundEvent)(object)value, protocolVersion);
            return;
        }
        if (typeof(T) == typeof(JukeboxSongData))
        {
            writer.WriteJukeboxSongData((JukeboxSongData)(object)value, protocolVersion);
            return;
        }

        throw new NotImplementedException($"Inline registryEntryHolder<{typeof(T).Name}> is not implemented.");
    }
}
