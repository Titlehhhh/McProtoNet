using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.craft_recipe_request", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("WindowId", "int")]
[PacketField("MakeAll", "bool")]
[PacketField("Recipe", "string", Group = "VUntil766", To = 766)]
[PacketField("Recipe", "string", Group = "V767", From = 767, To = 767)]
[PacketField("RecipeId", "int", Group = "V768_Last", From = 768)]
public sealed partial record CraftRecipeRequestPacket(int WindowId, bool MakeAll, CraftRecipeRequestPacket.VUntil766Layer? VUntil766 = null, CraftRecipeRequestPacket.V767Layer? V767 = null, CraftRecipeRequestPacket.V768_LastLayer? V768_Last = null) : IPacket<CraftRecipeRequestPacket>, IPacket
{
    public readonly record struct VUntil766Layer(string Recipe);
    public readonly record struct V767Layer(string Recipe);
    public readonly record struct V768_LastLayer(int RecipeId);
    public static CraftRecipeRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftRecipeRequestPacket>(protocolVersion);
        if (protocolVersion <= 766)
        {
            var windowId = reader.ReadSignedByte();
            var recipe = reader.ReadString();
            var makeAll = reader.ReadBoolean();
            return new CraftRecipeRequestPacket(windowId, makeAll, VUntil766: new VUntil766Layer(recipe));
        }

        if (protocolVersion >= 767 && protocolVersion <= 767)
        {
            var windowId = reader.ReadUnsignedByte();
            var recipe = reader.ReadString();
            var makeAll = reader.ReadBoolean();
            return new CraftRecipeRequestPacket(windowId, makeAll, V767: new V767Layer(recipe));
        }

        if (protocolVersion >= 768)
        {
            var windowId = reader.ReadVarInt();
            var recipeId = reader.ReadVarInt();
            var makeAll = reader.ReadBoolean();
            return new CraftRecipeRequestPacket(windowId, makeAll, V768_Last: new V768_LastLayer(recipeId));
        }

        throw new System.NotSupportedException($"CraftRecipeRequestPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CraftRecipeRequestPacket>(protocolVersion);
        if (protocolVersion <= 766)
        {
            var layer = VUntil766 ?? throw new WrongLayerException("CraftRecipeRequestPacket", protocolVersion, "VUntil766");
            string Recipe = layer.Recipe;
            writer.WriteSignedByte((sbyte)WindowId);
            writer.WriteString(Recipe);
            writer.WriteBoolean(MakeAll);
            return;
        }

        if (protocolVersion >= 767 && protocolVersion <= 767)
        {
            var layer = V767 ?? throw new WrongLayerException("CraftRecipeRequestPacket", protocolVersion, "V767");
            string Recipe = layer.Recipe;
            writer.WriteUnsignedByte((byte)WindowId);
            writer.WriteString(Recipe);
            writer.WriteBoolean(MakeAll);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("CraftRecipeRequestPacket", protocolVersion, "V768_Last");
            int RecipeId = layer.RecipeId;
            writer.WriteVarInt(WindowId);
            writer.WriteVarInt(RecipeId);
            writer.WriteBoolean(MakeAll);
            return;
        }

        throw new System.NotSupportedException($"CraftRecipeRequestPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.craft_recipe_request", "CraftRecipeRequest", PacketPhase.Play, PacketDirection.Serverbound, 15);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x27;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
