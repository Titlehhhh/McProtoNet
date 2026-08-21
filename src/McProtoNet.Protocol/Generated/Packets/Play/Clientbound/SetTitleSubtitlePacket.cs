using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_title_subtitle", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TextJson", "string", Group = "V755_764", From = 755, To = 764)]
[PacketField("Text", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record SetTitleSubtitlePacket(SetTitleSubtitlePacket.V755_764Layer? V755_764 = null, SetTitleSubtitlePacket.V765_LastLayer? V765_Last = null) : IPacket<SetTitleSubtitlePacket>, IPacket
{
    public readonly record struct V755_764Layer(string TextJson);
    public readonly record struct V765_LastLayer(NbtTag Text);
    public static SetTitleSubtitlePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleSubtitlePacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 764)
        {
            var textJson = reader.ReadString();
            return new SetTitleSubtitlePacket(V755_764: new V755_764Layer(textJson));
        }

        if (protocolVersion >= 765)
        {
            var text = reader.ReadNbtTag(false)!;
            return new SetTitleSubtitlePacket(V765_Last: new V765_LastLayer(text));
        }

        throw new System.NotSupportedException($"SetTitleSubtitlePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleSubtitlePacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 764)
        {
            var layer = V755_764 ?? throw new WrongLayerException("SetTitleSubtitlePacket", protocolVersion, "V755_764");
            string TextJson = layer.TextJson;
            writer.WriteString(TextJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("SetTitleSubtitlePacket", protocolVersion, "V765_Last");
            NbtTag Text = layer.Text;
            writer.WriteNbt(Text);
            return;
        }

        throw new System.NotSupportedException($"SetTitleSubtitlePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.set_title_subtitle", "SetTitleSubtitle", PacketPhase.Play, PacketDirection.Clientbound, 84);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x57;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 759)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x59;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x5D;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x6A;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x69;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x6E;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x70;
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
