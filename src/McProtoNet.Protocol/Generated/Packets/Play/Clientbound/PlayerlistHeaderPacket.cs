using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.playerlist_header", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("HeaderJson", "string", Group = "VUntil764", To = 764)]
[PacketField("FooterJson", "string", Group = "VUntil764", To = 764)]
[PacketField("Header", "NbtTag", Group = "V765_Last", From = 765)]
[PacketField("Footer", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record PlayerlistHeaderPacket(PlayerlistHeaderPacket.VUntil764Layer? VUntil764 = null, PlayerlistHeaderPacket.V765_LastLayer? V765_Last = null) : IPacket<PlayerlistHeaderPacket>, IPacket
{
    public readonly record struct VUntil764Layer(string HeaderJson, string FooterJson);
    public readonly record struct V765_LastLayer(NbtTag Header, NbtTag Footer);
    public static PlayerlistHeaderPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerlistHeaderPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var headerJson = reader.ReadString();
            var footerJson = reader.ReadString();
            return new PlayerlistHeaderPacket(VUntil764: new VUntil764Layer(headerJson, footerJson));
        }

        if (protocolVersion >= 765)
        {
            var header = reader.ReadNbtTag(false)!;
            var footer = reader.ReadNbtTag(false)!;
            return new PlayerlistHeaderPacket(V765_Last: new V765_LastLayer(header, footer));
        }

        throw new System.NotSupportedException($"PlayerlistHeaderPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerlistHeaderPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var layer = VUntil764 ?? throw new WrongLayerException("PlayerlistHeaderPacket", protocolVersion, "VUntil764");
            string HeaderJson = layer.HeaderJson;
            string FooterJson = layer.FooterJson;
            writer.WriteString(HeaderJson);
            writer.WriteString(FooterJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("PlayerlistHeaderPacket", protocolVersion, "V765_Last");
            NbtTag Header = layer.Header;
            NbtTag Footer = layer.Footer;
            writer.WriteNbt(Header);
            writer.WriteNbt(Footer);
            return;
        }

        throw new System.NotSupportedException($"PlayerlistHeaderPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        if (VUntil764 is { } vUntil764)
        {
            writer.WritePropertyName("HeaderJson");
            writer.WriteStringValue(vUntil764.HeaderJson);
            writer.WritePropertyName("FooterJson");
            writer.WriteStringValue(vUntil764.FooterJson);
        }
        else if (V765_Last is { } v765_Last)
        {
            writer.WritePropertyName("Header");
            v765_Last.Header.WriteJson(writer);
            writer.WritePropertyName("Footer");
            v765_Last.Footer.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.playerlist_header", "PlayerlistHeader", PacketPhase.Play, PacketDirection.Clientbound, 74);

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
