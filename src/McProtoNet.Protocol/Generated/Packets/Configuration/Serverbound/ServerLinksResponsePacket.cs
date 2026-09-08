using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(767, 770)]
[Packet("configuration.toServer.server_links", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Links", "ServerLink[]")]
public sealed partial record ServerLinksResponsePacket(ServerLink[] Links) : IPacket<ServerLinksResponsePacket>, IPacket
{
    public static ServerLinksResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinksResponsePacket>(protocolVersion);
        int linksCount = reader.ReadVarInt();
        var links = new ServerLink[linksCount];
        for (int i = 0; i < links.Length; i++)
            links[i] = reader.ReadType<ServerLink>(protocolVersion);
        return new ServerLinksResponsePacket(links);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinksResponsePacket>(protocolVersion);
        writer.WriteVarInt(Links.Length);
        foreach (var linksItem in Links)
            writer.WriteType<ServerLink>(linksItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toServer.server_links", "ServerLinksResponse", PacketPhase.Configuration, PacketDirection.Serverbound, 10);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 767 && protocolVersion <= 770)
        {
            id = 0x09;
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
