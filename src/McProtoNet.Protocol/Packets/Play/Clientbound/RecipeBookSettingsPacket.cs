using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RecipeBookSettings", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class RecipeBookSettingsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, MinecraftVersion.LatestProtocol)
    };

    public bool CraftingGuiOpen { get; set; }
    public bool CraftingFilteringCraftable { get; set; }
    public bool SmeltingGuiOpen { get; set; }
    public bool SmeltingFilteringCraftable { get; set; }
    public bool BlastGuiOpen { get; set; }
    public bool BlastFilteringCraftable { get; set; }
    public bool SmokerGuiOpen { get; set; }
    public bool SmokerFilteringCraftable { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                writer.WriteBoolean(CraftingGuiOpen);
                writer.WriteBoolean(CraftingFilteringCraftable);
                writer.WriteBoolean(SmeltingGuiOpen);
                writer.WriteBoolean(SmeltingFilteringCraftable);
                writer.WriteBoolean(BlastGuiOpen);
                writer.WriteBoolean(BlastFilteringCraftable);
                writer.WriteBoolean(SmokerGuiOpen);
                writer.WriteBoolean(SmokerFilteringCraftable);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookSettings), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 768 and <= MinecraftVersion.LatestProtocol:
                CraftingGuiOpen = reader.ReadBoolean();
                CraftingFilteringCraftable = reader.ReadBoolean();
                SmeltingGuiOpen = reader.ReadBoolean();
                SmeltingFilteringCraftable = reader.ReadBoolean();
                BlastGuiOpen = reader.ReadBoolean();
                BlastFilteringCraftable = reader.ReadBoolean();
                SmokerGuiOpen = reader.ReadBoolean();
                SmokerFilteringCraftable = reader.ReadBoolean();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.RecipeBookSettings), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
