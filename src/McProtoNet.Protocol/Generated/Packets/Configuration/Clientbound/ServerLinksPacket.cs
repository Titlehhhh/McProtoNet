using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.server_links", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Links", "ServerLink[]")]
public sealed partial record ServerLinksPacket(ServerLink[] Links) : IPacket<ServerLinksPacket>, IPacket
{
    public static ServerLinksPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinksPacket>(protocolVersion);
        int linksCount = reader.ReadVarInt();
        var links = new ServerLink[linksCount];
        for (int i = 0; i < links.Length; i++)
            links[i] = reader.ReadType<ServerLink>(protocolVersion);
        return new ServerLinksPacket(links);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinksPacket>(protocolVersion);
        writer.WriteVarInt(Links.Length);
        foreach (var linksItem in Links)
            writer.WriteType<ServerLink>(linksItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toClient.server_links", "ServerLinks", PacketPhase.Configuration, PacketDirection.Clientbound, 15);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 767 && protocolVersion <= 776)
        {
            id = 0x10;
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
