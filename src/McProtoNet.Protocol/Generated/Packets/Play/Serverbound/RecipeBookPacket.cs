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

    public static PacketIdentity Identity => new("play.toServer.recipe_book", "RecipeBook", PacketPhase.Play, PacketDirection.Serverbound, 40);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x2E;
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
