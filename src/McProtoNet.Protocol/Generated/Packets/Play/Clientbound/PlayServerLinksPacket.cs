using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Links");
        writer.WriteStartArray();
        foreach (var item0 in Links)
        {
            item0.WriteJson(writer);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.server_links", "PlayServerLinks", PacketPhase.Play, PacketDirection.Clientbound, 89);

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
