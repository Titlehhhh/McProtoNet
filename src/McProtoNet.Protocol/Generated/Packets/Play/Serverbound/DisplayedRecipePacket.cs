using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(751, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.displayed_recipe", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("RecipeId", "string", Group = "V751_767", From = 751, To = 767)]
[PacketField("RecipeIdInt", "int", Group = "V768_Last", From = 768)]
public sealed partial record DisplayedRecipePacket(DisplayedRecipePacket.V751_767Layer? V751_767 = null, DisplayedRecipePacket.V768_LastLayer? V768_Last = null) : IPacket<DisplayedRecipePacket>, IPacket
{
    public readonly record struct V751_767Layer(string RecipeId);
    public readonly record struct V768_LastLayer(int RecipeIdInt);
    public static DisplayedRecipePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DisplayedRecipePacket>(protocolVersion);
        if (protocolVersion >= 751 && protocolVersion <= 767)
        {
            var recipeId = reader.ReadString();
            return new DisplayedRecipePacket(V751_767: new V751_767Layer(recipeId));
        }

        if (protocolVersion >= 768)
        {
            var recipeIdInt = reader.ReadVarInt();
            return new DisplayedRecipePacket(V768_Last: new V768_LastLayer(recipeIdInt));
        }

        throw new System.NotSupportedException($"DisplayedRecipePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DisplayedRecipePacket>(protocolVersion);
        if (protocolVersion >= 751 && protocolVersion <= 767)
        {
            var layer = V751_767 ?? throw new WrongLayerException("DisplayedRecipePacket", protocolVersion, "V751_767");
            string RecipeId = layer.RecipeId;
            writer.WriteString(RecipeId);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("DisplayedRecipePacket", protocolVersion, "V768_Last");
            int RecipeIdInt = layer.RecipeIdInt;
            writer.WriteVarInt(RecipeIdInt);
            return;
        }

        throw new System.NotSupportedException($"DisplayedRecipePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.displayed_recipe", "DisplayedRecipe", PacketPhase.Play, PacketDirection.Serverbound, 20);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x2F;
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
