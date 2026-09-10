using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toServer.craft_recipe_request", "CraftRecipeRequest", PacketPhase.Play, PacketDirection.Serverbound, 18);

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
