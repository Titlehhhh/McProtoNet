using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_title_time", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("FadeIn", "int")]
[PacketField("Stay", "int")]
[PacketField("FadeOut", "int")]
public sealed partial record SetTitleTimePacket(int FadeIn, int Stay, int FadeOut) : IPacket<SetTitleTimePacket>, IPacket
{
    public static SetTitleTimePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleTimePacket>(protocolVersion);
        var fadeIn = reader.ReadSignedInt();
        var stay = reader.ReadSignedInt();
        var fadeOut = reader.ReadSignedInt();
        return new SetTitleTimePacket(fadeIn, stay, fadeOut);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleTimePacket>(protocolVersion);
        writer.WriteSignedInt(FadeIn);
        writer.WriteSignedInt(Stay);
        writer.WriteSignedInt(FadeOut);
    }

    public static PacketIdentity Identity => new("play.toClient.set_title_time", "SetTitleTime", PacketPhase.Play, PacketDirection.Clientbound, 86);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x5A;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 759)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x5E;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x5C;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x62;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x64;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x6D;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x6C;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x71;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x73;
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
