using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("RecipeBookSettings", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class RecipeBookSettingsPacket : IServerPacket
    {
        public bool CraftingGuiOpen { get; set; }
        public bool CraftingFilteringCraftable { get; set; }
        public bool SmeltingGuiOpen { get; set; }
        public bool SmeltingFilteringCraftable { get; set; }
        public bool BlastGuiOpen { get; set; }
        public bool BlastFilteringCraftable { get; set; }
        public bool SmokerGuiOpen { get; set; }
        public bool SmokerFilteringCraftable { get; set; }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : RecipeBookSettingsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                CraftingGuiOpen = reader.ReadBoolean();
                CraftingFilteringCraftable = reader.ReadBoolean();
                SmeltingGuiOpen = reader.ReadBoolean();
                SmeltingFilteringCraftable = reader.ReadBoolean();
                BlastGuiOpen = reader.ReadBoolean();
                BlastFilteringCraftable = reader.ReadBoolean();
                SmokerGuiOpen = reader.ReadBoolean();
                SmokerFilteringCraftable = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}