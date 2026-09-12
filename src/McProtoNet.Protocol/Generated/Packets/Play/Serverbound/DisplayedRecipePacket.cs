using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        if (V751_767 is { } v751_767)
        {
            writer.WritePropertyName("RecipeId");
            writer.WriteStringValue(v751_767.RecipeId);
        }
        else if (V768_Last is { } v768_Last)
        {
            writer.WritePropertyName("RecipeIdInt");
            writer.WriteNumberValue(v768_Last.RecipeIdInt);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.displayed_recipe", "DisplayedRecipe", PacketPhase.Play, PacketDirection.Serverbound, 23);

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
