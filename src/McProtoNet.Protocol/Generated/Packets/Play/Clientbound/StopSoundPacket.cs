using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.stop_sound", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Flags", "int")]
[PacketField("Source", "int?")]
[PacketField("Sound", "string?")]
public sealed partial record StopSoundPacket(int Flags, int? Source, string? Sound) : IPacket<StopSoundPacket>, IPacket
{
    public static StopSoundPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StopSoundPacket>(protocolVersion);
        var flags = reader.ReadSignedByte();
        int? source = default;
        if (flags == 1 || flags == 3)
        {
            var sourceValue = reader.ReadVarInt();
            source = sourceValue;
        }

        string? sound = default;
        if (flags == 2 || flags == 3)
        {
            var soundValue = reader.ReadString();
            sound = soundValue;
        }

        return new StopSoundPacket(flags, source, sound);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StopSoundPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)Flags);
        if ((sbyte)Flags == 1 || (sbyte)Flags == 3)
        {
            writer.WriteVarInt((Source ?? throw new System.InvalidOperationException("Source is required at this protocol version.")));
        }
        else if (Source is not null)
        {
            throw new System.InvalidOperationException("Source is set, but 'flags' does not select it at this protocol version.");
        }

        if ((sbyte)Flags == 2 || (sbyte)Flags == 3)
        {
            writer.WriteString((Sound ?? throw new System.InvalidOperationException("Sound is required at this protocol version.")));
        }
        else if (Sound is not null)
        {
            throw new System.InvalidOperationException("Sound is set, but 'flags' does not select it at this protocol version.");
        }
    }

    public static PacketIdentity Identity => new("play.toClient.stop_sound", "StopSound", PacketPhase.Play, PacketDirection.Clientbound, 106);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x52;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x52;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x5D;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 759)
        {
            id = 0x5E;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x68;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x6A;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x71;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x70;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x75;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x77;
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
