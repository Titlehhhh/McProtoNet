using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(751, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.recipe_book", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("BookId", "int")]
[PacketField("BookOpen", "bool")]
[PacketField("FilterActive", "bool")]
public sealed partial record RecipeBookPacket(int BookId, bool BookOpen, bool FilterActive) : IPacket<RecipeBookPacket>, IPacket
{
    public static RecipeBookPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookPacket>(protocolVersion);
        var bookId = reader.ReadVarInt();
        var bookOpen = reader.ReadBoolean();
        var filterActive = reader.ReadBoolean();
        return new RecipeBookPacket(bookId, bookOpen, filterActive);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookPacket>(protocolVersion);
        writer.WriteVarInt(BookId);
        writer.WriteBoolean(BookOpen);
        writer.WriteBoolean(FilterActive);
    }

    public static PacketIdentity Identity => new("play.toServer.recipe_book", "RecipeBook", PacketPhase.Play, PacketDirection.Serverbound, 44);

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
