using System;
using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static ChatTypeParameterType ReadChatTypeParameterType(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypeParameterType>(protocolVersion);
        int id = reader.ReadVarInt();
        return new ChatTypeParameterType(ReadChatTypeParameterTypeValue(id));
    }

    public static void WriteChatTypeParameterType(this MinecraftPrimitiveWriter writer, ChatTypeParameterType value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypeParameterType>(protocolVersion);
        writer.WriteVarInt(WriteChatTypeParameterTypeValue(value.Value));
    }

    public static ChatType ReadChatType(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatType>(protocolVersion);
        string translationKey = reader.ReadString();
        ChatTypeParameterType[] parameters = ReadArray(ref reader,
            (ref MinecraftPrimitiveReader r) => r.ReadChatTypeParameterType(protocolVersion));
        NbtTag style = reader.ReadAnonymousNbtTag(protocolVersion)
            ?? throw new InvalidOperationException("ChatType.style missing.");
        return new ChatType(translationKey, parameters, style);
    }

    public static void WriteChatType(this MinecraftPrimitiveWriter writer, ChatType value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatType>(protocolVersion);
        writer.WriteString(value.TranslationKey);
        WriteArray(writer, value.Parameters,
            (MinecraftPrimitiveWriter w, ChatTypeParameterType entry) => w.WriteChatTypeParameterType(entry, protocolVersion));
        writer.WriteAnonymousNbtTag(value.Style, protocolVersion);
    }

    public static ChatTypes ReadChatTypes(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypes>(protocolVersion);
        ChatType chat = reader.ReadChatType(protocolVersion);
        ChatType narration = reader.ReadChatType(protocolVersion);
        return new ChatTypes(chat, narration);
    }

    public static void WriteChatTypes(this MinecraftPrimitiveWriter writer, ChatTypes value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypes>(protocolVersion);
        writer.WriteChatType(value.Chat, protocolVersion);
        writer.WriteChatType(value.Narration, protocolVersion);
    }

    public static ChatTypesHolder ReadChatTypesHolder(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypesHolder>(protocolVersion);
        bool hasInline = reader.ReadBoolean();
        if (!hasInline)
        {
            int registryId = reader.ReadVarInt();
            return new ChatTypesHolder(registryId);
        }

        ChatTypes data = reader.ReadChatTypes(protocolVersion);
        return new ChatTypesHolder(data);
    }

    public static void WriteChatTypesHolder(this MinecraftPrimitiveWriter writer, ChatTypesHolder value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypesHolder>(protocolVersion);
        writer.WriteBoolean(value.HasInline);
        if (!value.HasInline)
        {
            writer.WriteVarInt(value.RegistryId ?? throw new InvalidOperationException("ChatTypesHolder.registryId missing."));
            return;
        }

        writer.WriteChatTypes(value.Data ?? throw new InvalidOperationException("ChatTypesHolder.data missing."), protocolVersion);
    }

    public static PositionUpdateRelatives ReadPositionUpdateRelatives(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionUpdateRelatives>(protocolVersion);
        if (protocolVersion <= 767)
        {
            byte flags = reader.ReadUnsignedByte();
            return new PositionUpdateRelatives(
                (flags & 0x01) != 0,
                (flags & 0x02) != 0,
                (flags & 0x04) != 0,
                (flags & 0x08) != 0,
                (flags & 0x10) != 0,
                false,
                false,
                false,
                false);
        }

        uint value = reader.ReadUnsignedInt();
        return new PositionUpdateRelatives(
            (value & 0x01) != 0,
            (value & 0x02) != 0,
            (value & 0x04) != 0,
            (value & 0x08) != 0,
            (value & 0x10) != 0,
            (value & 0x20) != 0,
            (value & 0x40) != 0,
            (value & 0x80) != 0,
            (value & 0x100) != 0);
    }

    public static void WritePositionUpdateRelatives(this MinecraftPrimitiveWriter writer, PositionUpdateRelatives value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionUpdateRelatives>(protocolVersion);
        if (protocolVersion <= 767)
        {
            byte flags = 0;
            if (value.X) flags |= 0x01;
            if (value.Y) flags |= 0x02;
            if (value.Z) flags |= 0x04;
            if (value.Yaw) flags |= 0x08;
            if (value.Pitch) flags |= 0x10;
            writer.WriteUnsignedByte(flags);
            return;
        }

        uint flags32 = 0;
        if (value.X) flags32 |= 0x01;
        if (value.Y) flags32 |= 0x02;
        if (value.Z) flags32 |= 0x04;
        if (value.Yaw) flags32 |= 0x08;
        if (value.Pitch) flags32 |= 0x10;
        if (value.Dx) flags32 |= 0x20;
        if (value.Dy) flags32 |= 0x40;
        if (value.Dz) flags32 |= 0x80;
        if (value.YawDelta) flags32 |= 0x100;
        writer.WriteUnsignedInt(flags32);
    }

    public static RecipeBookSetting ReadRecipeBookSetting(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookSetting>(protocolVersion);
        bool open = reader.ReadBoolean();
        bool filtering = reader.ReadBoolean();
        return new RecipeBookSetting(open, filtering);
    }

    public static void WriteRecipeBookSetting(this MinecraftPrimitiveWriter writer, RecipeBookSetting value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookSetting>(protocolVersion);
        writer.WriteBoolean(value.Open);
        writer.WriteBoolean(value.Filtering);
    }

    public static RecipeDisplay ReadRecipeDisplay(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeDisplay>(protocolVersion);
        string type = ReadRecipeDisplayType(reader.ReadVarInt());
        RecipeDisplayData data = type switch
        {
            "crafting_shapeless" => new RecipeDisplayData.CraftingShapeless(
                ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSlotDisplay(protocolVersion)),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion)),
            "crafting_shaped" => new RecipeDisplayData.CraftingShaped(
                reader.ReadVarInt(),
                reader.ReadVarInt(),
                ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSlotDisplay(protocolVersion)),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion)),
            "furnace" => new RecipeDisplayData.Furnace(
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadVarInt(),
                reader.ReadFloat()),
            "stonecutter" => new RecipeDisplayData.Stonecutter(
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion)),
            "smithing" => new RecipeDisplayData.Smithing(
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion)),
            _ => throw new InvalidOperationException($"Unknown RecipeDisplay type {type}.")
        };
        return new RecipeDisplay(type, data);
    }

    public static void WriteRecipeDisplay(this MinecraftPrimitiveWriter writer, RecipeDisplay value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeDisplay>(protocolVersion);
        writer.WriteVarInt(WriteRecipeDisplayType(value.Type));
        switch (value.Type)
        {
            case "crafting_shapeless" when value.Data is RecipeDisplayData.CraftingShapeless shapeless:
                WriteArray(writer, shapeless.Ingredients,
                    (MinecraftPrimitiveWriter w, SlotDisplay entry) => w.WriteSlotDisplay(entry, protocolVersion));
                writer.WriteSlotDisplay(shapeless.Result, protocolVersion);
                writer.WriteSlotDisplay(shapeless.CraftingStation, protocolVersion);
                return;
            case "crafting_shaped" when value.Data is RecipeDisplayData.CraftingShaped shaped:
                writer.WriteVarInt(shaped.Width);
                writer.WriteVarInt(shaped.Height);
                WriteArray(writer, shaped.Ingredients,
                    (MinecraftPrimitiveWriter w, SlotDisplay entry) => w.WriteSlotDisplay(entry, protocolVersion));
                writer.WriteSlotDisplay(shaped.Result, protocolVersion);
                writer.WriteSlotDisplay(shaped.CraftingStation, protocolVersion);
                return;
            case "furnace" when value.Data is RecipeDisplayData.Furnace furnace:
                writer.WriteSlotDisplay(furnace.Ingredient, protocolVersion);
                writer.WriteSlotDisplay(furnace.Fuel, protocolVersion);
                writer.WriteSlotDisplay(furnace.Result, protocolVersion);
                writer.WriteSlotDisplay(furnace.CraftingStation, protocolVersion);
                writer.WriteVarInt(furnace.Duration);
                writer.WriteFloat(furnace.Experience);
                return;
            case "stonecutter" when value.Data is RecipeDisplayData.Stonecutter stonecutter:
                writer.WriteSlotDisplay(stonecutter.Ingredient, protocolVersion);
                writer.WriteSlotDisplay(stonecutter.Result, protocolVersion);
                writer.WriteSlotDisplay(stonecutter.CraftingStation, protocolVersion);
                return;
            case "smithing" when value.Data is RecipeDisplayData.Smithing smithing:
                writer.WriteSlotDisplay(smithing.Template, protocolVersion);
                writer.WriteSlotDisplay(smithing.Base, protocolVersion);
                writer.WriteSlotDisplay(smithing.Addition, protocolVersion);
                writer.WriteSlotDisplay(smithing.Result, protocolVersion);
                writer.WriteSlotDisplay(smithing.CraftingStation, protocolVersion);
                return;
            default:
                throw new InvalidOperationException($"RecipeDisplay type/data mismatch for {value.Type}.");
        }
    }

    public static SlotDisplay ReadSlotDisplay(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotDisplay>(protocolVersion);
        string type = ReadSlotDisplayType(reader.ReadVarInt());
        SlotDisplayData data = type switch
        {
            "empty" => new SlotDisplayData.Empty(),
            "any_fuel" => new SlotDisplayData.AnyFuel(),
            "item" => new SlotDisplayData.Item(reader.ReadVarInt()),
            "item_stack" => new SlotDisplayData.ItemStack(reader.ReadSlot(protocolVersion)),
            "tag" => new SlotDisplayData.Tag(reader.ReadString()),
            "smithing_trim" => ReadSlotDisplaySmithingTrim(ref reader, protocolVersion),
            "with_remainder" => new SlotDisplayData.WithRemainder(
                reader.ReadSlotDisplay(protocolVersion),
                reader.ReadSlotDisplay(protocolVersion)),
            "composite" => new SlotDisplayData.Composite(
                ReadArray(ref reader, (ref MinecraftPrimitiveReader r) => r.ReadSlotDisplay(protocolVersion))),
            _ => throw new InvalidOperationException($"Unknown SlotDisplay type {type}.")
        };
        return new SlotDisplay(type, data);
    }

    public static void WriteSlotDisplay(this MinecraftPrimitiveWriter writer, SlotDisplay value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SlotDisplay>(protocolVersion);
        writer.WriteVarInt(WriteSlotDisplayType(value.Type));
        switch (value.Type)
        {
            case "empty" when value.Data is SlotDisplayData.Empty:
            case "any_fuel" when value.Data is SlotDisplayData.AnyFuel:
                return;
            case "item" when value.Data is SlotDisplayData.Item item:
                writer.WriteVarInt(item.ItemId);
                return;
            case "item_stack" when value.Data is SlotDisplayData.ItemStack stack:
                writer.WriteSlot(stack.Stack, protocolVersion);
                return;
            case "tag" when value.Data is SlotDisplayData.Tag tag:
                writer.WriteString(tag.TagId);
                return;
            case "smithing_trim" when value.Data is SlotDisplayData.SmithingTrim smithing:
                writer.WriteSlotDisplay(smithing.Base, protocolVersion);
                writer.WriteSlotDisplay(smithing.Material, protocolVersion);
                if (protocolVersion <= 769)
                {
                    writer.WriteSlotDisplay(smithing.PatternDisplay
                        ?? throw new InvalidOperationException("SlotDisplay.smithing_trim.patternDisplay missing."),
                        protocolVersion);
                }
                else
                {
                    writer.WriteRegistryEntryHolder(smithing.Pattern
                        ?? throw new InvalidOperationException("SlotDisplay.smithing_trim.pattern missing."),
                        protocolVersion);
                }
                return;
            case "with_remainder" when value.Data is SlotDisplayData.WithRemainder remainder:
                writer.WriteSlotDisplay(remainder.Input, protocolVersion);
                writer.WriteSlotDisplay(remainder.Remainder, protocolVersion);
                return;
            case "composite" when value.Data is SlotDisplayData.Composite composite:
                WriteArray(writer, composite.Entries,
                    (MinecraftPrimitiveWriter w, SlotDisplay entry) => w.WriteSlotDisplay(entry, protocolVersion));
                return;
            default:
                throw new InvalidOperationException($"SlotDisplay type/data mismatch for {value.Type}.");
        }
    }

    public static MovementFlags ReadMovementFlags(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MovementFlags>(protocolVersion);
        byte value = reader.ReadUnsignedByte();
        return new MovementFlags(
            (value & 0x01) != 0,
            (value & 0x02) != 0);
    }

    public static void WriteMovementFlags(this MinecraftPrimitiveWriter writer, MovementFlags value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MovementFlags>(protocolVersion);
        byte flags = 0;
        if (value.OnGround) flags |= 0x01;
        if (value.HasHorizontalCollision) flags |= 0x02;
        writer.WriteUnsignedByte(flags);
    }

    private static SlotDisplayData.SmithingTrim ReadSlotDisplaySmithingTrim(ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        SlotDisplay @base = reader.ReadSlotDisplay(protocolVersion);
        SlotDisplay material = reader.ReadSlotDisplay(protocolVersion);
        if (protocolVersion <= 769)
        {
            SlotDisplay patternDisplay = reader.ReadSlotDisplay(protocolVersion);
            return new SlotDisplayData.SmithingTrim(@base, material, patternDisplay, null);
        }

        RegistryEntryHolder<ArmorTrimPattern> pattern = reader.ReadRegistryEntryHolder<ArmorTrimPattern>(protocolVersion);
        return new SlotDisplayData.SmithingTrim(@base, material, null, pattern);
    }

    private static string ReadChatTypeParameterTypeValue(int id)
    {
        return id switch
        {
            0 => "content",
            1 => "sender",
            2 => "target",
            _ => throw new InvalidOperationException($"Unknown ChatTypeParameterType id {id}.")
        };
    }

    private static int WriteChatTypeParameterTypeValue(string value)
    {
        return value switch
        {
            "content" => 0,
            "sender" => 1,
            "target" => 2,
            _ => throw new InvalidOperationException($"Unknown ChatTypeParameterType value {value}.")
        };
    }

    private static string ReadRecipeDisplayType(int id)
    {
        return id switch
        {
            0 => "crafting_shapeless",
            1 => "crafting_shaped",
            2 => "furnace",
            3 => "stonecutter",
            4 => "smithing",
            _ => throw new InvalidOperationException($"Unknown RecipeDisplay type id {id}.")
        };
    }

    private static int WriteRecipeDisplayType(string value)
    {
        return value switch
        {
            "crafting_shapeless" => 0,
            "crafting_shaped" => 1,
            "furnace" => 2,
            "stonecutter" => 3,
            "smithing" => 4,
            _ => throw new InvalidOperationException($"Unknown RecipeDisplay type value {value}.")
        };
    }

    private static string ReadSlotDisplayType(int id)
    {
        return id switch
        {
            0 => "empty",
            1 => "any_fuel",
            2 => "item",
            3 => "item_stack",
            4 => "tag",
            5 => "smithing_trim",
            6 => "with_remainder",
            7 => "composite",
            _ => throw new InvalidOperationException($"Unknown SlotDisplay type id {id}.")
        };
    }

    private static int WriteSlotDisplayType(string value)
    {
        return value switch
        {
            "empty" => 0,
            "any_fuel" => 1,
            "item" => 2,
            "item_stack" => 3,
            "tag" => 4,
            "smithing_trim" => 5,
            "with_remainder" => 6,
            "composite" => 7,
            _ => throw new InvalidOperationException($"Unknown SlotDisplay type value {value}.")
        };
    }
}
