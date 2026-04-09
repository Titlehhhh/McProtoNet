using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RecipeBookAdd", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class RecipeBookAddPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, MinecraftVersion.LatestProtocol),
    };

    public RecipeEntry[] Entries { get; set; } = Array.Empty<RecipeEntry>();
    public bool Replace { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Entries.Length);
                for (int i = 0; i < Entries.Length; i++)
                {
                    WriteRecipeData(writer, Entries[i].Recipe, protocolVersion);
                    byte flags = 0;
                    if (Entries[i].Notification) flags |= 0x01;
                    if (Entries[i].Highlight) flags |= 0x02;
                    writer.WriteUnsignedByte(flags);
                }
                writer.WriteBoolean(Replace);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookAdd), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                int count = reader.ReadVarInt();
                var entries = new RecipeEntry[count];
                for (int i = 0; i < entries.Length; i++)
                {
                    RecipeData recipe = ReadRecipeData(ref reader, protocolVersion);
                    byte flags = reader.ReadUnsignedByte();
                    entries[i] = new RecipeEntry
                    {
                        Recipe = recipe,
                        Notification = (flags & 0x01) != 0,
                        Highlight = (flags & 0x02) != 0
                    };
                }
                Entries = entries;
                Replace = reader.ReadBoolean();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookAdd), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static RecipeData ReadRecipeData(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int displayId = reader.ReadVarInt();
        RecipeDisplay display = reader.ReadRecipeDisplay(protocolVersion);
        int group = reader.ReadVarInt();
        int category = reader.ReadVarInt();
        IDSet[]? craftingRequirements = reader.ReadBoolean()
            ? ReadRequirements(ref reader, protocolVersion)
            : null;
        return new RecipeData
        {
            DisplayId = displayId,
            Display = display,
            Group = group,
            Category = category,
            CraftingRequirements = craftingRequirements
        };
    }

    private static IDSet[] ReadRequirements(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<IDSet>();
        }

        var requirements = new IDSet[count];
        for (int i = 0; i < requirements.Length; i++)
        {
            requirements[i] = reader.ReadIDSet(protocolVersion);
        }
        return requirements;
    }

    private static void WriteRecipeData(MinecraftPrimitiveWriter writer, RecipeData recipe, int protocolVersion)
    {
        writer.WriteVarInt(recipe.DisplayId);
        writer.WriteRecipeDisplay(recipe.Display, protocolVersion);
        writer.WriteVarInt(recipe.Group);
        writer.WriteVarInt(recipe.Category);
        if (recipe.CraftingRequirements is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteVarInt(recipe.CraftingRequirements.Length);
            for (int i = 0; i < recipe.CraftingRequirements.Length; i++)
            {
                writer.WriteIDSet(recipe.CraftingRequirements[i], protocolVersion);
            }
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct RecipeEntry
    {
        public RecipeData Recipe { get; set; }
        public bool Notification { get; set; }
        public bool Highlight { get; set; }
    }

    public struct RecipeData
    {
        public int DisplayId { get; set; }
        public RecipeDisplay Display { get; set; }
        public int Group { get; set; }
        public int Category { get; set; }
        public IDSet[]? CraftingRequirements { get; set; }
    }
}
