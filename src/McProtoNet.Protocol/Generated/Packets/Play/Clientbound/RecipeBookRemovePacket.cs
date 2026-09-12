using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.recipe_book_remove", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("RecipeIds", "int[]")]
public sealed partial record RecipeBookRemovePacket(int[] RecipeIds) : IPacket<RecipeBookRemovePacket>, IPacket
{
    public static RecipeBookRemovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookRemovePacket>(protocolVersion);
        int recipeIdsCount = reader.ReadVarInt();
        var recipeIds = new int[recipeIdsCount];
        for (int i = 0; i < recipeIds.Length; i++)
            recipeIds[i] = reader.ReadVarInt();
        return new RecipeBookRemovePacket(recipeIds);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookRemovePacket>(protocolVersion);
        writer.WriteVarInt(RecipeIds.Length);
        foreach (var recipeIdsItem in RecipeIds)
            writer.WriteVarInt(recipeIdsItem);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("RecipeIds");
        writer.WriteStartArray();
        foreach (var item0 in RecipeIds)
        {
            writer.WriteNumberValue(item0);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.recipe_book_remove", "RecipeBookRemove", PacketPhase.Play, PacketDirection.Clientbound, 77);

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
