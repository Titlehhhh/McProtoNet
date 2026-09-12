using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class ServerLink : IProtocolType<ServerLink>
{
    public ServerLinkLabel Label { get; }
    public string Link { get; }

    public ServerLink(ServerLinkLabel label, string link)
    {
        Label = label;
        Link = link;
    }

    public static ServerLink Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLink>(protocolVersion);
        var _hasKnownType = reader.ReadUnsignedByte();
        var label = ServerLinkLabel.Read(ref reader, protocolVersion, (int)_hasKnownType);
        var link = reader.ReadString();
        return new ServerLink(label, link);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLink>(protocolVersion);
        writer.WriteUnsignedByte(checked((byte)Label.Discriminator(protocolVersion)));
        Label.Write(writer, protocolVersion);
        writer.WriteString(Link);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Label");
        Label.WriteJson(writer);
        writer.WritePropertyName("Link");
        writer.WriteStringValue(Link);
        writer.WriteEndObject();
    }
}
