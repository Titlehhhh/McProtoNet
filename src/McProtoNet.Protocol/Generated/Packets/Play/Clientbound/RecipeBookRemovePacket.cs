using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.recipe_book_remove", "RecipeBookRemove", PacketPhase.Play, PacketDirection.Clientbound, 66);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x45;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x44;
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
