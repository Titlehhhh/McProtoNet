using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UnlockRecipes", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class UnlockRecipesPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 736),
        new(751, 767),
    };

    public int Action { get; set; }
    public bool CraftingBookOpen { get; set; }
    public bool FilteringCraftable { get; set; }
    public bool SmeltingBookOpen { get; set; }
    public bool FilteringSmeltable { get; set; }
    public string[] Recipes1 { get; set; } = Array.Empty<string>();
    public string[]? Recipes2 { get; set; }

    public V751_767Fields? V751_767 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
                writer.WriteVarInt(Action);
                writer.WriteBoolean(CraftingBookOpen);
                writer.WriteBoolean(FilteringCraftable);
                writer.WriteBoolean(SmeltingBookOpen);
                writer.WriteBoolean(FilteringSmeltable);
                writer.WriteVarInt(Recipes1.Length);
                for (int i = 0; i < Recipes1.Length; i++)
                {
                    writer.WriteString(Recipes1[i]);
                }
                if (Action == 0)
                {
                    writer.WriteVarInt(Recipes2?.Length ?? 0);
                    if (Recipes2 is not null)
                    {
                        for (int i = 0; i < Recipes2.Length; i++)
                        {
                            writer.WriteString(Recipes2[i]);
                        }
                    }
                }
                return;
            case >= 751 and <= 767:
            {
                var fields = V751_767 ?? throw new InvalidOperationException("UnlockRecipes V751_767 fields missing.");
                writer.WriteVarInt(Action);
                writer.WriteBoolean(CraftingBookOpen);
                writer.WriteBoolean(FilteringCraftable);
                writer.WriteBoolean(SmeltingBookOpen);
                writer.WriteBoolean(FilteringSmeltable);
                writer.WriteBoolean(fields.BlastFurnaceOpen);
                writer.WriteBoolean(fields.FilteringBlastFurnace);
                writer.WriteBoolean(fields.SmokerBookOpen);
                writer.WriteBoolean(fields.FilteringSmoker);
                writer.WriteVarInt(Recipes1.Length);
                for (int i = 0; i < Recipes1.Length; i++)
                {
                    writer.WriteString(Recipes1[i]);
                }
                if (Action == 0)
                {
                    writer.WriteVarInt(Recipes2?.Length ?? 0);
                    if (Recipes2 is not null)
                    {
                        for (int i = 0; i < Recipes2.Length; i++)
                        {
                            writer.WriteString(Recipes2[i]);
                        }
                    }
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UnlockRecipes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 736:
                Action = reader.ReadVarInt();
                CraftingBookOpen = reader.ReadBoolean();
                FilteringCraftable = reader.ReadBoolean();
                SmeltingBookOpen = reader.ReadBoolean();
                FilteringSmeltable = reader.ReadBoolean();
                Recipes1 = ReadRecipeList(ref reader);
                Recipes2 = Action == 0 ? ReadRecipeList(ref reader) : null;
                return;
            case >= 751 and <= 767:
            {
                var fields = new V751_767Fields();
                Action = reader.ReadVarInt();
                CraftingBookOpen = reader.ReadBoolean();
                FilteringCraftable = reader.ReadBoolean();
                SmeltingBookOpen = reader.ReadBoolean();
                FilteringSmeltable = reader.ReadBoolean();
                fields.BlastFurnaceOpen = reader.ReadBoolean();
                fields.FilteringBlastFurnace = reader.ReadBoolean();
                fields.SmokerBookOpen = reader.ReadBoolean();
                fields.FilteringSmoker = reader.ReadBoolean();
                Recipes1 = ReadRecipeList(ref reader);
                Recipes2 = Action == 0 ? ReadRecipeList(ref reader) : null;
                V751_767 = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.UnlockRecipes), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static string[] ReadRecipeList(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<string>();
        }

        var recipes = new string[count];
        for (int i = 0; i < recipes.Length; i++)
        {
            recipes[i] = reader.ReadString();
        }
        return recipes;
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V751_767Fields
    {
        public bool BlastFurnaceOpen { get; set; }
        public bool FilteringBlastFurnace { get; set; }
        public bool SmokerBookOpen { get; set; }
        public bool FilteringSmoker { get; set; }
    }
}
