using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RecipeBookSettings", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[PacketId(768, 769, 0x46)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x45)]
public sealed partial class RecipeBookSettingsPacket : IServerPacket
{
    public V768_770Fields? V768_770 { get; set; }
    public V771_LastFields? V771_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                var fields = V768_770 ?? throw new InvalidOperationException("RecipeBookSettingsPacket 768-770 fields missing.");
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
                var fields = V771_Last ?? throw new InvalidOperationException("RecipeBookSettingsPacket 771-last fields missing.");
                writer.WriteType<RecipeBookSetting>(fields.Crafting, protocolVersion);
                writer.WriteType<RecipeBookSetting>(fields.Furnace, protocolVersion);
                writer.WriteType<RecipeBookSetting>(fields.Blast, protocolVersion);
                writer.WriteType<RecipeBookSetting>(fields.Smoker, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(RecipeBookSettingsPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
            {
                V768_770 = new V768_770Fields
                {
                    CraftingGuiOpen = reader.ReadBoolean(),
                    CraftingFilteringCraftable = reader.ReadBoolean(),
                    SmeltingGuiOpen = reader.ReadBoolean(),
                    SmeltingFilteringCraftable = reader.ReadBoolean(),
                    BlastGuiOpen = reader.ReadBoolean(),
                    BlastFilteringCraftable = reader.ReadBoolean(),
                    SmokerGuiOpen = reader.ReadBoolean(),
                    SmokerFilteringCraftable = reader.ReadBoolean()
                };
                V771_Last = null;
                return;
            }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                V771_Last = new V771_LastFields
                {
                    Crafting = reader.ReadType<RecipeBookSetting>(protocolVersion),
                    Furnace = reader.ReadType<RecipeBookSetting>(protocolVersion),
                    Blast = reader.ReadType<RecipeBookSetting>(protocolVersion),
                    Smoker = reader.ReadType<RecipeBookSetting>(protocolVersion)
                };
                V768_770 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(RecipeBookSettingsPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

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