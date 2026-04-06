using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DeclareRecipes", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class DeclareRecipesPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 767),
        new(768, MinecraftVersion.LatestProtocol),
    };

    public VFirst_767Fields? VFirst_767 { get; set; }
    public V768_LastFields? V768_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                var fields = VFirst_767 ?? throw new InvalidOperationException("DeclareRecipes VFirst_767 fields missing.");
                writer.WriteVarInt(fields.Recipes.Length);
                for (int i = 0; i < fields.Recipes.Length; i++)
                {
                    WriteRecipeEntry(writer, fields.Recipes[i], protocolVersion);
                }
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V768_Last ?? throw new InvalidOperationException("DeclareRecipes V768_Last fields missing.");
                writer.WriteVarInt(fields.Recipes.Length);
                for (int i = 0; i < fields.Recipes.Length; i++)
                {
                    writer.WriteString(fields.Recipes[i].Name);
                    writer.WriteVarInt(fields.Recipes[i].Items.Length);
                    for (int j = 0; j < fields.Recipes[i].Items.Length; j++)
                    {
                        writer.WriteVarInt(fields.Recipes[i].Items[j]);
                    }
                }
                writer.WriteVarInt(fields.StoneCutterRecipes.Length);
                for (int i = 0; i < fields.StoneCutterRecipes.Length; i++)
                {
                    writer.WriteIDSet(fields.StoneCutterRecipes[i].Input, protocolVersion);
                    writer.WriteSlotDisplay(fields.StoneCutterRecipes[i].SlotDisplay, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DeclareRecipes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                int count = reader.ReadVarInt();
                var entries = new RecipeEntry[count];
                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i] = ReadRecipeEntry(ref reader, protocolVersion);
                }
                VFirst_767 = new VFirst_767Fields { Recipes = entries };
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                int count = reader.ReadVarInt();
                var recipes = new RecipeGroupEntry[count];
                for (int i = 0; i < recipes.Length; i++)
                {
                    string name = reader.ReadString();
                    int itemCount = reader.ReadVarInt();
                    var items = new int[itemCount];
                    for (int j = 0; j < items.Length; j++)
                    {
                        items[j] = reader.ReadVarInt();
                    }
                    recipes[i] = new RecipeGroupEntry { Name = name, Items = items };
                }

                int stoneCount = reader.ReadVarInt();
                var stoneRecipes = new StoneCutterRecipe[stoneCount];
                for (int i = 0; i < stoneRecipes.Length; i++)
                {
                    stoneRecipes[i] = new StoneCutterRecipe
                    {
                        Input = reader.ReadIDSet(protocolVersion),
                        SlotDisplay = reader.ReadSlotDisplay(protocolVersion)
                    };
                }

                V768_Last = new V768_LastFields { Recipes = recipes, StoneCutterRecipes = stoneRecipes };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DeclareRecipes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static RecipeEntry ReadRecipeEntry(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string type = protocolVersion >= 766
            ? ReadRecipeTypeFromId(reader.ReadVarInt())
            : reader.ReadString();
        string recipeId = reader.ReadString();
        RecipeData data = ReadRecipeData(ref reader, type, protocolVersion);
        return new RecipeEntry { Type = type, RecipeId = recipeId, Data = data };
    }

    private static RecipeData ReadRecipeData(ref MinecraftPrimitiveReader reader, string type, int protocolVersion)
    {
        switch (type)
        {
            case "minecraft:crafting_shapeless":
            {
                string group = reader.ReadString();
                int? category = protocolVersion >= 761 ? reader.ReadVarInt() : null;
                Ingredient[] ingredients = ReadIngredientArray(ref reader, protocolVersion);
                Slot result = reader.ReadSlot(protocolVersion);
                return new RecipeData.CraftingShapeless(group, category, ingredients, result);
            }
            case "minecraft:crafting_shaped":
            {
                if (protocolVersion >= 765)
                {
                    string group = reader.ReadString();
                    int category = reader.ReadVarInt();
                    int width = reader.ReadVarInt();
                    int height = reader.ReadVarInt();
                    Ingredient[][] ingredients = ReadShapedIngredients(ref reader, width, height, protocolVersion,
                        outerIsWidth: true);
                    Slot result = reader.ReadSlot(protocolVersion);
                    bool showNotification = reader.ReadBoolean();
                    return new RecipeData.CraftingShaped(group, category, width, height, ingredients, result, showNotification);
                }

                int widthValue = reader.ReadVarInt();
                int heightValue = reader.ReadVarInt();
                string groupValue = reader.ReadString();
                int? categoryValue = protocolVersion >= 761 ? reader.ReadVarInt() : null;
                Ingredient[][] shapedIngredients = ReadShapedIngredients(ref reader, widthValue, heightValue, protocolVersion,
                    outerIsWidth: protocolVersion < 751 || protocolVersion > 754);
                Slot shapedResult = reader.ReadSlot(protocolVersion);
                bool? showNotificationValue = protocolVersion >= 762 ? reader.ReadBoolean() : null;
                return new RecipeData.CraftingShaped(groupValue, categoryValue, widthValue, heightValue, shapedIngredients,
                    shapedResult, showNotificationValue);
            }
            case "minecraft:crafting_special_armordye":
            case "minecraft:crafting_special_bookcloning":
            case "minecraft:crafting_special_mapcloning":
            case "minecraft:crafting_special_mapextending":
            case "minecraft:crafting_special_firework_rocket":
            case "minecraft:crafting_special_firework_star":
            case "minecraft:crafting_special_firework_star_fade":
            case "minecraft:crafting_special_repairitem":
            case "minecraft:crafting_special_tippedarrow":
            case "minecraft:crafting_special_bannerduplicate":
            case "minecraft:crafting_special_banneraddpattern":
            case "minecraft:crafting_special_shielddecoration":
            case "minecraft:crafting_special_shulkerboxcoloring":
            case "minecraft:crafting_special_suspiciousstew":
            case "minecraft:crafting_decorated_pot":
                if (protocolVersion >= 761)
                {
                    MinecraftSimpleRecipeFormat format = reader.ReadMinecraftSimpleRecipeFormat(protocolVersion);
                    return new RecipeData.SimpleRecipe(format);
                }
                return new RecipeData.SimpleRecipe(null);
            case "minecraft:smelting":
            case "minecraft:blasting":
            case "minecraft:smoking":
            case "minecraft:campfire_cooking":
                return new RecipeData.Smelting(ReadSmeltingFormat(ref reader, protocolVersion));
            case "minecraft:stonecutting":
            {
                string group = reader.ReadString();
                Ingredient ingredient = ReadIngredient(ref reader, protocolVersion);
                Slot result = reader.ReadSlot(protocolVersion);
                return new RecipeData.Stonecutting(group, ingredient, result);
            }
            case "minecraft:smithing":
            {
                Ingredient baseIngredient = ReadIngredient(ref reader, protocolVersion);
                Ingredient addition = ReadIngredient(ref reader, protocolVersion);
                Slot result = reader.ReadSlot(protocolVersion);
                return new RecipeData.Smithing(baseIngredient, addition, result);
            }
            case "minecraft:smithing_transform":
            {
                Ingredient template = ReadIngredient(ref reader, protocolVersion);
                Ingredient baseIngredient = ReadIngredient(ref reader, protocolVersion);
                Ingredient addition = ReadIngredient(ref reader, protocolVersion);
                Slot result = reader.ReadSlot(protocolVersion);
                return new RecipeData.SmithingTransform(template, baseIngredient, addition, result);
            }
            case "minecraft:smithing_trim":
            {
                Ingredient template = ReadIngredient(ref reader, protocolVersion);
                Ingredient baseIngredient = ReadIngredient(ref reader, protocolVersion);
                Ingredient addition = ReadIngredient(ref reader, protocolVersion);
                return new RecipeData.SmithingTrim(template, baseIngredient, addition);
            }
            default:
                throw new InvalidOperationException($"Unknown recipe type {type}.");
        }
    }

    private static void WriteRecipeEntry(MinecraftPrimitiveWriter writer, RecipeEntry entry, int protocolVersion)
    {
        if (protocolVersion >= 766)
        {
            writer.WriteVarInt(GetRecipeTypeId(entry.Type));
        }
        else
        {
            writer.WriteString(entry.Type);
        }

        writer.WriteString(entry.RecipeId);
        WriteRecipeData(writer, entry.Data, entry.Type, protocolVersion);
    }

    private static void WriteRecipeData(MinecraftPrimitiveWriter writer, RecipeData data, string type,
        int protocolVersion)
    {
        switch (data)
        {
            case RecipeData.CraftingShapeless shapeless:
                writer.WriteString(shapeless.Group);
                if (protocolVersion >= 761)
                {
                    writer.WriteVarInt(shapeless.Category ?? 0);
                }
                WriteIngredientArray(writer, shapeless.Ingredients, protocolVersion);
                writer.WriteSlot(shapeless.Result, protocolVersion);
                return;
            case RecipeData.CraftingShaped shaped:
                if (protocolVersion >= 765)
                {
                    writer.WriteString(shaped.Group);
                    writer.WriteVarInt(shaped.Category ?? 0);
                    writer.WriteVarInt(shaped.Width);
                    writer.WriteVarInt(shaped.Height);
                    WriteShapedIngredients(writer, shaped.Ingredients, shaped.Width, shaped.Height, protocolVersion,
                        outerIsWidth: true);
                    writer.WriteSlot(shaped.Result, protocolVersion);
                    writer.WriteBoolean(shaped.ShowNotification ?? false);
                    return;
                }

                writer.WriteVarInt(shaped.Width);
                writer.WriteVarInt(shaped.Height);
                writer.WriteString(shaped.Group);
                if (protocolVersion >= 761)
                {
                    writer.WriteVarInt(shaped.Category ?? 0);
                }
                WriteShapedIngredients(writer, shaped.Ingredients, shaped.Width, shaped.Height, protocolVersion,
                    outerIsWidth: protocolVersion < 751 || protocolVersion > 754);
                writer.WriteSlot(shaped.Result, protocolVersion);
                if (protocolVersion >= 762)
                {
                    writer.WriteBoolean(shaped.ShowNotification ?? false);
                }
                return;
            case RecipeData.SimpleRecipe simple:
                if (protocolVersion >= 761)
                {
                    writer.WriteMinecraftSimpleRecipeFormat(simple.Format ?? new MinecraftSimpleRecipeFormat(0), protocolVersion);
                }
                return;
            case RecipeData.Smelting smelting:
                WriteSmeltingFormat(writer, smelting.Format, protocolVersion);
                return;
            case RecipeData.Stonecutting stonecutting:
                writer.WriteString(stonecutting.Group);
                WriteIngredient(writer, stonecutting.Ingredient, protocolVersion);
                writer.WriteSlot(stonecutting.Result, protocolVersion);
                return;
            case RecipeData.Smithing smithing:
                WriteIngredient(writer, smithing.Base, protocolVersion);
                WriteIngredient(writer, smithing.Addition, protocolVersion);
                writer.WriteSlot(smithing.Result, protocolVersion);
                return;
            case RecipeData.SmithingTransform smithingTransform:
                WriteIngredient(writer, smithingTransform.Template, protocolVersion);
                WriteIngredient(writer, smithingTransform.Base, protocolVersion);
                WriteIngredient(writer, smithingTransform.Addition, protocolVersion);
                writer.WriteSlot(smithingTransform.Result, protocolVersion);
                return;
            case RecipeData.SmithingTrim smithingTrim:
                WriteIngredient(writer, smithingTrim.Template, protocolVersion);
                WriteIngredient(writer, smithingTrim.Base, protocolVersion);
                WriteIngredient(writer, smithingTrim.Addition, protocolVersion);
                return;
            default:
                throw new InvalidOperationException($"Unknown recipe data for type {type}.");
        }
    }

    private static Ingredient ReadIngredient(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        var ingredient = new Ingredient();
        for (int i = 0; i < count; i++)
        {
            ingredient.Add(reader.ReadSlot(protocolVersion));
        }
        return ingredient;
    }

    private static Ingredient[] ReadIngredientArray(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<Ingredient>();
        }

        var ingredients = new Ingredient[count];
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i] = ReadIngredient(ref reader, protocolVersion);
        }

        return ingredients;
    }

    private static Ingredient[][] ReadShapedIngredients(ref MinecraftPrimitiveReader reader, int width, int height,
        int protocolVersion, bool outerIsWidth)
    {
        int outerCount = outerIsWidth ? width : height;
        int innerCount = outerIsWidth ? height : width;
        var result = new Ingredient[outerCount][];
        for (int i = 0; i < outerCount; i++)
        {
            var inner = new Ingredient[innerCount];
            for (int j = 0; j < innerCount; j++)
            {
                inner[j] = ReadIngredient(ref reader, protocolVersion);
            }
            result[i] = inner;
        }
        return result;
    }

    private static void WriteIngredient(MinecraftPrimitiveWriter writer, Ingredient ingredient, int protocolVersion)
    {
        writer.WriteVarInt(ingredient.Count);
        for (int i = 0; i < ingredient.Count; i++)
        {
            writer.WriteSlot(ingredient[i], protocolVersion);
        }
    }

    private static void WriteIngredientArray(MinecraftPrimitiveWriter writer, Ingredient[] ingredients,
        int protocolVersion)
    {
        writer.WriteVarInt(ingredients.Length);
        for (int i = 0; i < ingredients.Length; i++)
        {
            WriteIngredient(writer, ingredients[i], protocolVersion);
        }
    }

    private static void WriteShapedIngredients(MinecraftPrimitiveWriter writer, Ingredient[][] ingredients, int width,
        int height, int protocolVersion, bool outerIsWidth)
    {
        int outerCount = outerIsWidth ? width : height;
        int innerCount = outerIsWidth ? height : width;
        for (int i = 0; i < outerCount; i++)
        {
            for (int j = 0; j < innerCount; j++)
            {
                WriteIngredient(writer, ingredients[i][j], protocolVersion);
            }
        }
    }

    private static SmeltingFormat ReadSmeltingFormat(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        string group = reader.ReadString();
        int? category = protocolVersion >= 761 ? reader.ReadVarInt() : null;
        Ingredient ingredient = ReadIngredient(ref reader, protocolVersion);
        Slot result = reader.ReadSlot(protocolVersion);
        float experience = reader.ReadFloat();
        int cookTime = reader.ReadVarInt();
        return new SmeltingFormat
        {
            Group = group,
            Category = category,
            Ingredient = ingredient,
            Result = result,
            Experience = experience,
            CookTime = cookTime
        };
    }

    private static void WriteSmeltingFormat(MinecraftPrimitiveWriter writer, SmeltingFormat format, int protocolVersion)
    {
        writer.WriteString(format.Group);
        if (protocolVersion >= 761)
        {
            writer.WriteVarInt(format.Category ?? 0);
        }
        WriteIngredient(writer, format.Ingredient, protocolVersion);
        writer.WriteSlot(format.Result, protocolVersion);
        writer.WriteFloat(format.Experience);
        writer.WriteVarInt(format.CookTime);
    }

    private static string ReadRecipeTypeFromId(int id)
    {
        if ((uint)id >= (uint)RecipeTypeMapping.Length)
        {
            throw new InvalidOperationException($"Unknown recipe type id {id}.");
        }

        return RecipeTypeMapping[id];
    }

    private static int GetRecipeTypeId(string name)
    {
        int id = Array.IndexOf(RecipeTypeMapping, name);
        if (id < 0)
        {
            throw new InvalidOperationException($"Unknown recipe type {name}.");
        }
        return id;
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_767Fields
    {
        public RecipeEntry[] Recipes { get; set; }
    }

    public struct V768_LastFields
    {
        public RecipeGroupEntry[] Recipes { get; set; }
        public StoneCutterRecipe[] StoneCutterRecipes { get; set; }
    }

    public struct RecipeEntry
    {
        public string Type { get; set; }
        public string RecipeId { get; set; }
        public RecipeData Data { get; set; }
    }

    public struct RecipeGroupEntry
    {
        public string Name { get; set; }
        public int[] Items { get; set; }
    }

    public struct StoneCutterRecipe
    {
        public IDSet Input { get; set; }
        public SlotDisplay SlotDisplay { get; set; }
    }

    public abstract record RecipeData
    {
        public sealed record CraftingShapeless(string Group, int? Category, Ingredient[] Ingredients, Slot Result) : RecipeData;
        public sealed record CraftingShaped(string Group, int? Category, int Width, int Height, Ingredient[][] Ingredients,
            Slot Result, bool? ShowNotification) : RecipeData;
        public sealed record SimpleRecipe(MinecraftSimpleRecipeFormat? Format) : RecipeData;
        public sealed record Smelting(SmeltingFormat Format) : RecipeData;
        public sealed record Stonecutting(string Group, Ingredient Ingredient, Slot Result) : RecipeData;
        public sealed record Smithing(Ingredient Base, Ingredient Addition, Slot Result) : RecipeData;
        public sealed record SmithingTransform(Ingredient Template, Ingredient Base, Ingredient Addition, Slot Result) : RecipeData;
        public sealed record SmithingTrim(Ingredient Template, Ingredient Base, Ingredient Addition) : RecipeData;
    }

    public struct SmeltingFormat
    {
        public string Group { get; set; }
        public int? Category { get; set; }
        public Ingredient Ingredient { get; set; }
        public Slot Result { get; set; }
        public float Experience { get; set; }
        public int CookTime { get; set; }
    }

    private static readonly string[] RecipeTypeMapping =
    {
        "minecraft:crafting_shaped", "minecraft:crafting_shapeless", "minecraft:crafting_special_armordye",
        "minecraft:crafting_special_bookcloning", "minecraft:crafting_special_mapcloning",
        "minecraft:crafting_special_mapextending", "minecraft:crafting_special_firework_rocket",
        "minecraft:crafting_special_firework_star", "minecraft:crafting_special_firework_star_fade",
        "minecraft:crafting_special_tippedarrow", "minecraft:crafting_special_bannerduplicate",
        "minecraft:crafting_special_shielddecoration", "minecraft:crafting_special_shulkerboxcoloring",
        "minecraft:crafting_special_suspiciousstew", "minecraft:crafting_special_repairitem", "minecraft:smelting",
        "minecraft:blasting", "minecraft:smoking", "minecraft:campfire_cooking", "minecraft:stonecutting",
        "minecraft:smithing_transform", "minecraft:smithing_trim", "minecraft:crafting_decorated_pot"
    };
}
