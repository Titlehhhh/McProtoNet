using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.teams", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TeamName", "string")]
[PacketField("Action", "TeamAction")]
public sealed partial record TeamsPacket(string TeamName, TeamAction Action) : IPacket<TeamsPacket>, IPacket
{
    public static TeamsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeamsPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var teamName = reader.ReadString();
            var _mode = reader.ReadSignedByte();
            var action = TeamAction.Read(ref reader, protocolVersion, (int)_mode);
            return new TeamsPacket(teamName, action);
        }

        if (protocolVersion >= 771)
        {
            var teamName = reader.ReadString();
            var _mode = reader.ReadSignedByte();
            var action = TeamAction.Read(ref reader, protocolVersion, (int)_mode);
            return new TeamsPacket(teamName, action);
        }

        throw new System.NotSupportedException($"TeamsPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeamsPacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            writer.WriteString(TeamName);
            writer.WriteSignedByte(checked((sbyte)Action.Discriminator(protocolVersion)));
            Action.Write(writer, protocolVersion);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteString(TeamName);
            writer.WriteSignedByte(checked((sbyte)Action.Discriminator(protocolVersion)));
            Action.Write(writer, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"TeamsPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.teams", "Teams", PacketPhase.Play, PacketDirection.Clientbound, 104);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 759)
        {
            id = 0x55;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x56;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x5A;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x5C;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x5E;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x67;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x6B;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x6D;
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
