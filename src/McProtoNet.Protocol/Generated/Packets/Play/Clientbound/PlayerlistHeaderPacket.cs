using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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

    public static PacketIdentity Identity => new("play.toClient.playerlist_header", "PlayerlistHeader", PacketPhase.Play, PacketDirection.Clientbound, 69);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x5E;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x65;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x68;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x6A;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x6D;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x74;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x73;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x78;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x7A;
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
