using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(735, 767)]
[Packet("play.toClient.unlock_recipes", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Action", "int")]
[PacketField("CraftingBookOpen", "bool")]
[PacketField("FilteringCraftable", "bool")]
[PacketField("SmeltingBookOpen", "bool")]
[PacketField("FilteringSmeltable", "bool")]
[PacketField("Recipes1", "string[]")]
[PacketField("Recipes2", "string[]?")]
[PacketField("BlastFurnaceOpen", "bool", Group = "V751_767", From = 751, To = 767)]
[PacketField("FilteringBlastFurnace", "bool", Group = "V751_767", From = 751, To = 767)]
[PacketField("SmokerBookOpen", "bool", Group = "V751_767", From = 751, To = 767)]
[PacketField("FilteringSmoker", "bool", Group = "V751_767", From = 751, To = 767)]
public sealed partial record UnlockRecipesPacket(int Action, bool CraftingBookOpen, bool FilteringCraftable, bool SmeltingBookOpen, bool FilteringSmeltable, string[] Recipes1, string[]? Recipes2, UnlockRecipesPacket.V751_767Layer? V751_767 = null) : IPacket<UnlockRecipesPacket>, IPacket
{
    public readonly record struct V751_767Layer(bool BlastFurnaceOpen, bool FilteringBlastFurnace, bool SmokerBookOpen, bool FilteringSmoker);
    public static UnlockRecipesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UnlockRecipesPacket>(protocolVersion);
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            var action = reader.ReadVarInt();
            var craftingBookOpen = reader.ReadBoolean();
            var filteringCraftable = reader.ReadBoolean();
            var smeltingBookOpen = reader.ReadBoolean();
            var filteringSmeltable = reader.ReadBoolean();
            int recipes1Count = reader.ReadVarInt();
            var recipes1 = new string[recipes1Count];
            for (int i = 0; i < recipes1.Length; i++)
                recipes1[i] = reader.ReadString();
            string[]? recipes2 = default;
            if (action == 0)
            {
                int recipes2ValueCount = reader.ReadVarInt();
                var recipes2Value = new string[recipes2ValueCount];
                for (int i = 0; i < recipes2Value.Length; i++)
                    recipes2Value[i] = reader.ReadString();
                recipes2 = recipes2Value;
            }

            return new UnlockRecipesPacket(action, craftingBookOpen, filteringCraftable, smeltingBookOpen, filteringSmeltable, recipes1, recipes2);
        }

        if (protocolVersion >= 751 && protocolVersion <= 767)
        {
            var action = reader.ReadVarInt();
            var craftingBookOpen = reader.ReadBoolean();
            var filteringCraftable = reader.ReadBoolean();
            var smeltingBookOpen = reader.ReadBoolean();
            var filteringSmeltable = reader.ReadBoolean();
            var blastFurnaceOpen = reader.ReadBoolean();
            var filteringBlastFurnace = reader.ReadBoolean();
            var smokerBookOpen = reader.ReadBoolean();
            var filteringSmoker = reader.ReadBoolean();
            int recipes1Count = reader.ReadVarInt();
            var recipes1 = new string[recipes1Count];
            for (int i = 0; i < recipes1.Length; i++)
                recipes1[i] = reader.ReadString();
            string[]? recipes2 = default;
            if (action == 0)
            {
                int recipes2ValueCount = reader.ReadVarInt();
                var recipes2Value = new string[recipes2ValueCount];
                for (int i = 0; i < recipes2Value.Length; i++)
                    recipes2Value[i] = reader.ReadString();
                recipes2 = recipes2Value;
            }

            return new UnlockRecipesPacket(action, craftingBookOpen, filteringCraftable, smeltingBookOpen, filteringSmeltable, recipes1, recipes2, V751_767: new V751_767Layer(blastFurnaceOpen, filteringBlastFurnace, smokerBookOpen, filteringSmoker));
        }

        throw new System.NotSupportedException($"UnlockRecipesPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UnlockRecipesPacket>(protocolVersion);
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            writer.WriteVarInt(Action);
            writer.WriteBoolean(CraftingBookOpen);
            writer.WriteBoolean(FilteringCraftable);
            writer.WriteBoolean(SmeltingBookOpen);
            writer.WriteBoolean(FilteringSmeltable);
            writer.WriteVarInt(Recipes1.Length);
            foreach (var recipes1Item in Recipes1)
                writer.WriteString(recipes1Item);
            if (Action == 0)
            {
                var recipes2Value = Recipes2 ?? throw new System.InvalidOperationException("Recipes2 is required at this protocol version.");
                writer.WriteVarInt(recipes2Value.Length);
                foreach (var recipes2Item in recipes2Value)
                    writer.WriteString(recipes2Item);
            }
            else if (Recipes2 is not null)
            {
                throw new System.InvalidOperationException("Recipes2 is set, but 'action' does not select it at this protocol version.");
            }

            return;
        }

        if (protocolVersion >= 751 && protocolVersion <= 767)
        {
            var layer = V751_767 ?? throw new WrongLayerException("UnlockRecipesPacket", protocolVersion, "V751_767");
            bool BlastFurnaceOpen = layer.BlastFurnaceOpen;
            bool FilteringBlastFurnace = layer.FilteringBlastFurnace;
            bool SmokerBookOpen = layer.SmokerBookOpen;
            bool FilteringSmoker = layer.FilteringSmoker;
            writer.WriteVarInt(Action);
            writer.WriteBoolean(CraftingBookOpen);
            writer.WriteBoolean(FilteringCraftable);
            writer.WriteBoolean(SmeltingBookOpen);
            writer.WriteBoolean(FilteringSmeltable);
            writer.WriteBoolean(BlastFurnaceOpen);
            writer.WriteBoolean(FilteringBlastFurnace);
            writer.WriteBoolean(SmokerBookOpen);
            writer.WriteBoolean(FilteringSmoker);
            writer.WriteVarInt(Recipes1.Length);
            foreach (var recipes1Item in Recipes1)
                writer.WriteString(recipes1Item);
            if (Action == 0)
            {
                var recipes2Value = Recipes2 ?? throw new System.InvalidOperationException("Recipes2 is required at this protocol version.");
                writer.WriteVarInt(recipes2Value.Length);
                foreach (var recipes2Item in recipes2Value)
                    writer.WriteString(recipes2Item);
            }
            else if (Recipes2 is not null)
            {
                throw new System.InvalidOperationException("Recipes2 is set, but 'action' does not select it at this protocol version.");
            }

            return;
        }

        throw new System.NotSupportedException($"UnlockRecipesPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.unlock_recipes", "UnlockRecipes", PacketPhase.Play, PacketDirection.Clientbound, 121);

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
