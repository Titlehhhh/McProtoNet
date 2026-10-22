using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.recipe_book_settings", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("CraftingGuiOpen", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("CraftingFilteringCraftable", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("SmeltingGuiOpen", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("SmeltingFilteringCraftable", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("BlastGuiOpen", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("BlastFilteringCraftable", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("SmokerGuiOpen", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("SmokerFilteringCraftable", "bool", Group = "V768_770", From = 768, To = 770)]
[PacketField("Crafting", "RecipeBookSetting", Group = "V771_Last", From = 771)]
[PacketField("Furnace", "RecipeBookSetting", Group = "V771_Last", From = 771)]
[PacketField("Blast", "RecipeBookSetting", Group = "V771_Last", From = 771)]
[PacketField("Smoker", "RecipeBookSetting", Group = "V771_Last", From = 771)]
public sealed partial record RecipeBookSettingsPacket(RecipeBookSettingsPacket.V768_770Layer? V768_770 = null, RecipeBookSettingsPacket.V771_LastLayer? V771_Last = null) : IPacket<RecipeBookSettingsPacket>, IPacket
{
    public readonly record struct V768_770Layer(bool CraftingGuiOpen, bool CraftingFilteringCraftable, bool SmeltingGuiOpen, bool SmeltingFilteringCraftable, bool BlastGuiOpen, bool BlastFilteringCraftable, bool SmokerGuiOpen, bool SmokerFilteringCraftable);
    public readonly record struct V771_LastLayer(RecipeBookSetting Crafting, RecipeBookSetting Furnace, RecipeBookSetting Blast, RecipeBookSetting Smoker);
    public static RecipeBookSettingsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookSettingsPacket>(protocolVersion);
        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            var craftingGuiOpen = reader.ReadBoolean();
            var craftingFilteringCraftable = reader.ReadBoolean();
            var smeltingGuiOpen = reader.ReadBoolean();
            var smeltingFilteringCraftable = reader.ReadBoolean();
            var blastGuiOpen = reader.ReadBoolean();
            var blastFilteringCraftable = reader.ReadBoolean();
            var smokerGuiOpen = reader.ReadBoolean();
            var smokerFilteringCraftable = reader.ReadBoolean();
            return new RecipeBookSettingsPacket(V768_770: new V768_770Layer(craftingGuiOpen, craftingFilteringCraftable, smeltingGuiOpen, smeltingFilteringCraftable, blastGuiOpen, blastFilteringCraftable, smokerGuiOpen, smokerFilteringCraftable));
        }

        if (protocolVersion >= 771)
        {
            var crafting = reader.ReadType<RecipeBookSetting>(protocolVersion);
            var furnace = reader.ReadType<RecipeBookSetting>(protocolVersion);
            var blast = reader.ReadType<RecipeBookSetting>(protocolVersion);
            var smoker = reader.ReadType<RecipeBookSetting>(protocolVersion);
            return new RecipeBookSettingsPacket(V771_Last: new V771_LastLayer(crafting, furnace, blast, smoker));
        }

        throw new System.NotSupportedException($"RecipeBookSettingsPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookSettingsPacket>(protocolVersion);
        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            var layer = V768_770 ?? throw new WrongLayerException("RecipeBookSettingsPacket", protocolVersion, "V768_770");
            bool CraftingGuiOpen = layer.CraftingGuiOpen;
            bool CraftingFilteringCraftable = layer.CraftingFilteringCraftable;
            bool SmeltingGuiOpen = layer.SmeltingGuiOpen;
            bool SmeltingFilteringCraftable = layer.SmeltingFilteringCraftable;
            bool BlastGuiOpen = layer.BlastGuiOpen;
            bool BlastFilteringCraftable = layer.BlastFilteringCraftable;
            bool SmokerGuiOpen = layer.SmokerGuiOpen;
            bool SmokerFilteringCraftable = layer.SmokerFilteringCraftable;
            writer.WriteBoolean(CraftingGuiOpen);
            writer.WriteBoolean(CraftingFilteringCraftable);
            writer.WriteBoolean(SmeltingGuiOpen);
            writer.WriteBoolean(SmeltingFilteringCraftable);
            writer.WriteBoolean(BlastGuiOpen);
            writer.WriteBoolean(BlastFilteringCraftable);
            writer.WriteBoolean(SmokerGuiOpen);
            writer.WriteBoolean(SmokerFilteringCraftable);
            return;
        }

        if (protocolVersion >= 771)
        {
            var layer = V771_Last ?? throw new WrongLayerException("RecipeBookSettingsPacket", protocolVersion, "V771_Last");
            RecipeBookSetting Crafting = layer.Crafting;
            RecipeBookSetting Furnace = layer.Furnace;
            RecipeBookSetting Blast = layer.Blast;
            RecipeBookSetting Smoker = layer.Smoker;
            writer.WriteType<RecipeBookSetting>(Crafting, protocolVersion);
            writer.WriteType<RecipeBookSetting>(Furnace, protocolVersion);
            writer.WriteType<RecipeBookSetting>(Blast, protocolVersion);
            writer.WriteType<RecipeBookSetting>(Smoker, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"RecipeBookSettingsPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.recipe_book_settings", "RecipeBookSettings", PacketPhase.Play, PacketDirection.Clientbound, 81);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
