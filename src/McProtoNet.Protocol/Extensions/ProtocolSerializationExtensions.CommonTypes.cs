using System;
using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static BannerPattern ReadBannerPattern(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BannerPattern>(protocolVersion);
        string assetId = reader.ReadString();
        string translationKey = reader.ReadString();
        return new BannerPattern(assetId, translationKey);
    }

    public static void WriteBannerPattern(this MinecraftPrimitiveWriter writer, BannerPattern value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BannerPattern>(protocolVersion);
        writer.WriteString(value.AssetId);
        writer.WriteString(value.TranslationKey);
    }

    public static DataComponentMatchers ReadDataComponentMatchers(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DataComponentMatchers>(protocolVersion);
        ExactComponentMatcher exactMatchers = reader.ReadExactComponentMatcher(protocolVersion);
        int[] partialMatchers = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadVarInt());
        return new DataComponentMatchers(exactMatchers, partialMatchers);
    }

    public static void WriteDataComponentMatchers(this MinecraftPrimitiveWriter writer, DataComponentMatchers value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DataComponentMatchers>(protocolVersion);
        writer.WriteExactComponentMatcher(value.ExactMatchers, protocolVersion);
        WriteArray(ref writer, value.PartialMatchers, (ref MinecraftPrimitiveWriter w, int entry) => w.WriteVarInt(entry));
    }

    public static ExactComponentMatcher ReadExactComponentMatcher(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ExactComponentMatcher>(protocolVersion);
        SlotComponent[] components = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSlotComponent(protocolVersion));
        return new ExactComponentMatcher(components);
    }

    public static void WriteExactComponentMatcher(this MinecraftPrimitiveWriter writer, ExactComponentMatcher value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ExactComponentMatcher>(protocolVersion);
        WriteArray(ref writer, value.Components, (ref MinecraftPrimitiveWriter w, SlotComponent component) =>
            w.WriteSlotComponent(component, protocolVersion));
    }

    public static GameProfile ReadGameProfile(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameProfile>(protocolVersion);
        string name = reader.ReadString();
        GameProfile.GameProfileProperty[] properties = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            string propName = r.ReadString();
            string value = r.ReadString();
            string? signature = r.ReadBoolean() ? r.ReadString() : null;
            return new GameProfile.GameProfileProperty(propName, value, signature);
        });
        return new GameProfile(name, properties);
    }

    public static void WriteGameProfile(this MinecraftPrimitiveWriter writer, GameProfile value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameProfile>(protocolVersion);
        writer.WriteString(value.Name);
        WriteArray(ref writer, value.Properties, (ref MinecraftPrimitiveWriter w, GameProfile.GameProfileProperty property) =>
        {
            w.WriteString(property.Name);
            w.WriteString(property.Value);
            if (property.Signature is null)
            {
                w.WriteBoolean(false);
            }
            else
            {
                w.WriteBoolean(true);
                w.WriteString(property.Signature);
            }
        });
    }

    public static ItemBlockProperty ReadItemBlockProperty(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemBlockProperty>(protocolVersion);
        string name = reader.ReadString();
        bool isExactMatch = reader.ReadBoolean();
        string? exactValue = null;
        string? minValue = null;
        string? maxValue = null;
        if (isExactMatch)
        {
            exactValue = reader.ReadString();
        }
        else
        {
            minValue = reader.ReadString();
            maxValue = reader.ReadString();
        }
        return new ItemBlockProperty(name, isExactMatch, exactValue, minValue, maxValue);
    }

    public static void WriteItemBlockProperty(this MinecraftPrimitiveWriter writer, ItemBlockProperty value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemBlockProperty>(protocolVersion);
        writer.WriteString(value.Name);
        writer.WriteBoolean(value.IsExactMatch);
        if (value.IsExactMatch)
        {
            writer.WriteString(value.ExactValue ?? string.Empty);
        }
        else
        {
            writer.WriteString(value.MinValue ?? string.Empty);
            writer.WriteString(value.MaxValue ?? string.Empty);
        }
    }

    public static ItemEffectDetail ReadItemEffectDetail(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemEffectDetail>(protocolVersion);
        int amplifier = reader.ReadVarInt();
        int duration = reader.ReadVarInt();
        bool ambient = reader.ReadBoolean();
        bool showParticles = reader.ReadBoolean();
        bool showIcon = reader.ReadBoolean();
        ItemEffectDetail? hiddenEffect = null;
        if (reader.ReadBoolean())
        {
            hiddenEffect = reader.ReadItemEffectDetail(protocolVersion);
        }
        return new ItemEffectDetail(amplifier, duration, ambient, showParticles, showIcon, hiddenEffect);
    }

    public static void WriteItemEffectDetail(this MinecraftPrimitiveWriter writer, ItemEffectDetail value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemEffectDetail>(protocolVersion);
        writer.WriteVarInt(value.Amplifier);
        writer.WriteVarInt(value.Duration);
        writer.WriteBoolean(value.Ambient);
        writer.WriteBoolean(value.ShowParticles);
        writer.WriteBoolean(value.ShowIcon);
        if (value.HiddenEffect is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteItemEffectDetail(value.HiddenEffect, protocolVersion);
        }
    }

    public static ItemSoundEvent ReadItemSoundEvent(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemSoundEvent>(protocolVersion);
        string soundName = reader.ReadString();
        float? fixedRange = reader.ReadBoolean() ? reader.ReadFloat() : null;
        return new ItemSoundEvent(soundName, fixedRange);
    }

    public static void WriteItemSoundEvent(this MinecraftPrimitiveWriter writer, ItemSoundEvent value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemSoundEvent>(protocolVersion);
        writer.WriteString(value.SoundName);
        if (value.FixedRange is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteFloat(value.FixedRange.Value);
        }
    }

    public static ChatSession? ReadChatSession(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatSession>(protocolVersion);
        if (!reader.ReadBoolean())
        {
            return null;
        }

        Guid uuid = reader.ReadUUID();
        long expireTime = reader.ReadSignedLong();
        byte[] keyBytes = reader.ReadBuffer(reader.ReadVarInt());
        byte[] keySignature = reader.ReadBuffer(reader.ReadVarInt());
        var publicKey = new ChatSession.ChatSessionPublicKey(expireTime, keyBytes, keySignature);
        return new ChatSession(uuid, publicKey);
    }

    public static void WriteChatSession(this MinecraftPrimitiveWriter writer, ChatSession? value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatSession>(protocolVersion);
        if (value is null)
        {
            writer.WriteBoolean(false);
            return;
        }

        writer.WriteBoolean(true);
        writer.WriteUUID(value.Uuid);
        writer.WriteSignedLong(value.PublicKey.ExpireTime);
        writer.WriteVarInt(value.PublicKey.KeyBytes.Length);
        writer.WriteBuffer(value.PublicKey.KeyBytes);
        writer.WriteVarInt(value.PublicKey.KeySignature.Length);
        writer.WriteBuffer(value.PublicKey.KeySignature);
    }

    public static MinecraftSimpleRecipeFormat ReadMinecraftSimpleRecipeFormat(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MinecraftSimpleRecipeFormat>(protocolVersion);
        int category = reader.ReadVarInt();
        return new MinecraftSimpleRecipeFormat(category);
    }

    public static void WriteMinecraftSimpleRecipeFormat(this MinecraftPrimitiveWriter writer,
        MinecraftSimpleRecipeFormat value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MinecraftSimpleRecipeFormat>(protocolVersion);
        writer.WriteVarInt(value.Category);
    }

    public static PackedChunkPos ReadPackedChunkPos(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PackedChunkPos>(protocolVersion);
        int z = reader.ReadSignedInt();
        int x = reader.ReadSignedInt();
        return new PackedChunkPos(z, x);
    }

    public static void WritePackedChunkPos(this MinecraftPrimitiveWriter writer, PackedChunkPos value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PackedChunkPos>(protocolVersion);
        writer.WriteSignedInt(value.Z);
        writer.WriteSignedInt(value.X);
    }

    public static PreviousMessages ReadPreviousMessages(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PreviousMessages>(protocolVersion);
        PreviousMessages.PreviousMessage[] messages = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            if (protocolVersion == 760)
            {
                Guid sender = r.ReadUUID();
                byte[] signatureBytes = r.ReadBuffer(r.ReadVarInt());
                return new PreviousMessages.PreviousMessage(sender, signatureBytes, null, null);
            }

            int id = r.ReadVarInt();
            byte[]? signature = null;
            if (id == 0)
            {
                signature = r.ReadBuffer(256);
            }
            return new PreviousMessages.PreviousMessage(null, null, id, signature);
        });
        return new PreviousMessages(messages);
    }

    public static void WritePreviousMessages(this MinecraftPrimitiveWriter writer, PreviousMessages value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PreviousMessages>(protocolVersion);
        WriteArray(ref writer, value.Messages, (ref MinecraftPrimitiveWriter w, PreviousMessages.PreviousMessage message) =>
        {
            if (protocolVersion == 760)
            {
                if (message.MessageSender is null || message.MessageSignature is null)
                {
                    throw new InvalidOperationException("PreviousMessages entry missing sender/signature.");
                }
                w.WriteUUID(message.MessageSender.Value);
                w.WriteVarInt(message.MessageSignature.Length);
                w.WriteBuffer(message.MessageSignature);
                return;
            }

            if (message.Id is null)
            {
                throw new InvalidOperationException("PreviousMessages entry missing id.");
            }
            int id = message.Id.Value;
            w.WriteVarInt(id);
            if (id == 0)
            {
                if (message.Signature is null || message.Signature.Length != 256)
                {
                    throw new InvalidOperationException("PreviousMessages signature must be 256 bytes.");
                }
                w.WriteBuffer(message.Signature);
            }
        });
    }

    public static ServerLinkType ReadServerLinkType(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinkType>(protocolVersion);
        return new ServerLinkType(ReadServerLinkTypeValue(reader.ReadVarInt()));
    }

    public static void WriteServerLinkType(this MinecraftPrimitiveWriter writer, ServerLinkType value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinkType>(protocolVersion);
        writer.WriteVarInt(WriteServerLinkTypeValue(value.Value));
    }

    public static SoundSource ReadSoundSource(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SoundSource>(protocolVersion);
        int id = reader.ReadVarInt();
        return new SoundSource(ReadSoundSourceValue(id, protocolVersion));
    }

    public static void WriteSoundSource(this MinecraftPrimitiveWriter writer, SoundSource value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SoundSource>(protocolVersion);
        writer.WriteVarInt(WriteSoundSourceValue(value.Value, protocolVersion));
    }

    public static Tags ReadTags(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Tags>(protocolVersion);
        Tag[] values = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            string tagName = r.ReadString();
            int[] entries = ReadArray(ref r, (ref MinecraftPrimitiveReader r2) => r2.ReadVarInt());
            return new Tag(tagName, entries);
        });
        return new Tags(values);
    }

    public static void WriteTags(this MinecraftPrimitiveWriter writer, Tags value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Tags>(protocolVersion);
        WriteArray(ref writer, value.Values, (ref MinecraftPrimitiveWriter w, Tag tag) =>
        {
            w.WriteString(tag.TagName);
            WriteArray(ref w, tag.Entries, (ref MinecraftPrimitiveWriter w2, int entry) => w2.WriteVarInt(entry));
        });
    }

    private static string ReadServerLinkTypeValue(int id)
    {
        return id switch
        {
            0 => "bug_report",
            1 => "community_guidelines",
            2 => "support",
            3 => "status",
            4 => "feedback",
            5 => "community",
            6 => "website",
            7 => "forums",
            8 => "news",
            9 => "announcements",
            _ => throw new InvalidOperationException($"Unknown ServerLinkType id {id}")
        };
    }

    private static int WriteServerLinkTypeValue(string value)
    {
        return value switch
        {
            "bug_report" => 0,
            "community_guidelines" => 1,
            "support" => 2,
            "status" => 3,
            "feedback" => 4,
            "community" => 5,
            "website" => 6,
            "forums" => 7,
            "news" => 8,
            "announcements" => 9,
            _ => throw new InvalidOperationException($"Unknown ServerLinkType value {value}")
        };
    }

    private static string ReadSoundSourceValue(int id, int protocolVersion)
    {
        return id switch
        {
            0 => "master",
            1 => "music",
            2 => "record",
            3 => "weather",
            4 => "block",
            5 => "hostile",
            6 => "neutral",
            7 => "player",
            8 => "ambient",
            9 => "voice",
            10 when protocolVersion >= 771 => "ui",
            _ => throw new InvalidOperationException($"Unknown SoundSource id {id}")
        };
    }

    private static int WriteSoundSourceValue(string value, int protocolVersion)
    {
        return value switch
        {
            "master" => 0,
            "music" => 1,
            "record" => 2,
            "weather" => 3,
            "block" => 4,
            "hostile" => 5,
            "neutral" => 6,
            "player" => 7,
            "ambient" => 8,
            "voice" => 9,
            "ui" when protocolVersion >= 771 => 10,
            _ => throw new InvalidOperationException($"Unknown SoundSource value {value}")
        };
    }
}
