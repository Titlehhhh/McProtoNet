using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RecipeBookSettings", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class RecipeBookSettingsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, 770),
        new(771, MinecraftVersion.LatestProtocol)
    };

    public V768_770Fields? V768_770 { get; set; }
    public V771_LastFields? V771_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= 770:
            {
                var fields = V768_770 ?? throw new InvalidOperationException("RecipeBookSettings V768_770 missing.");
                writer.WriteBoolean(fields.CraftingGuiOpen);
                writer.WriteBoolean(fields.CraftingFilteringCraftable);
                writer.WriteBoolean(fields.SmeltingGuiOpen);
                writer.WriteBoolean(fields.SmeltingFilteringCraftable);
                writer.WriteBoolean(fields.BlastGuiOpen);
                writer.WriteBoolean(fields.BlastFilteringCraftable);
                writer.WriteBoolean(fields.SmokerGuiOpen);
                writer.WriteBoolean(fields.SmokerFilteringCraftable);
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V771_Last ?? throw new InvalidOperationException("RecipeBookSettings V771_Last missing.");
                writer.WriteRecipeBookSetting(fields.Crafting, protocolVersion);
                writer.WriteRecipeBookSetting(fields.Furnace, protocolVersion);
                writer.WriteRecipeBookSetting(fields.Blast, protocolVersion);
                writer.WriteRecipeBookSetting(fields.Smoker, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookSettings), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= 770:
            {
                var fields = new V768_770Fields();
                fields.CraftingGuiOpen = reader.ReadBoolean();
                fields.CraftingFilteringCraftable = reader.ReadBoolean();
                fields.SmeltingGuiOpen = reader.ReadBoolean();
                fields.SmeltingFilteringCraftable = reader.ReadBoolean();
                fields.BlastGuiOpen = reader.ReadBoolean();
                fields.BlastFilteringCraftable = reader.ReadBoolean();
                fields.SmokerGuiOpen = reader.ReadBoolean();
                fields.SmokerFilteringCraftable = reader.ReadBoolean();
                V768_770 = fields;
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V771_LastFields();
                fields.Crafting = reader.ReadRecipeBookSetting(protocolVersion);
                fields.Furnace = reader.ReadRecipeBookSetting(protocolVersion);
                fields.Blast = reader.ReadRecipeBookSetting(protocolVersion);
                fields.Smoker = reader.ReadRecipeBookSetting(protocolVersion);
                V771_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookSettings), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V768_770Fields
    {
        public bool CraftingGuiOpen { get; set; }
        public bool CraftingFilteringCraftable { get; set; }
        public bool SmeltingGuiOpen { get; set; }
        public bool SmeltingFilteringCraftable { get; set; }
        public bool BlastGuiOpen { get; set; }
        public bool BlastFilteringCraftable { get; set; }
        public bool SmokerGuiOpen { get; set; }
        public bool SmokerFilteringCraftable { get; set; }
    }

    public struct V771_LastFields
    {
        public RecipeBookSetting Crafting { get; set; }
        public RecipeBookSetting Furnace { get; set; }
        public RecipeBookSetting Blast { get; set; }
        public RecipeBookSetting Smoker { get; set; }
    }
}
