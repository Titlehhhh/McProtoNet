using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.server_links", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Links", "ServerLink[]")]
public sealed partial record PlayServerLinksPacket(ServerLink[] Links) : IPacket<PlayServerLinksPacket>, IPacket
{
    public static PlayServerLinksPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayServerLinksPacket>(protocolVersion);
        int linksCount = reader.ReadVarInt();
        var links = new ServerLink[linksCount];
        for (int i = 0; i < links.Length; i++)
            links[i] = reader.ReadType<ServerLink>(protocolVersion);
        return new PlayServerLinksPacket(links);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayServerLinksPacket>(protocolVersion);
        writer.WriteVarInt(Links.Length);
        foreach (var linksItem in Links)
            writer.WriteType<ServerLink>(linksItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.server_links", "PlayServerLinks", PacketPhase.Play, PacketDirection.Clientbound, 88);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 767 && protocolVersion <= 767)
        {
            id = 0x7B;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x82;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x87;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x89;
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
