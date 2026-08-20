using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.scoreboard_display_objective", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Position", "int")]
[PacketField("Name", "string")]
public sealed partial record ScoreboardDisplayObjectivePacket(int Position, string Name) : IPacket<ScoreboardDisplayObjectivePacket>, IPacket
{
    public static ScoreboardDisplayObjectivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ScoreboardDisplayObjectivePacket>(protocolVersion);
        if (protocolVersion <= 763)
        {
            var position = reader.ReadSignedByte();
            var name = reader.ReadString();
            return new ScoreboardDisplayObjectivePacket(position, name);
        }

        if (protocolVersion >= 764)
        {
            var position = reader.ReadVarInt();
            var name = reader.ReadString();
            return new ScoreboardDisplayObjectivePacket(position, name);
        }

        throw new System.NotSupportedException($"ScoreboardDisplayObjectivePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ScoreboardDisplayObjectivePacket>(protocolVersion);
        if (protocolVersion <= 763)
        {
            writer.WriteSignedByte((sbyte)Position);
            writer.WriteString(Name);
            return;
        }

        if (protocolVersion >= 764)
        {
            writer.WriteVarInt(Position);
            writer.WriteString(Name);
            return;
        }

        throw new System.NotSupportedException($"ScoreboardDisplayObjectivePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.scoreboard_display_objective", "ScoreboardDisplayObjective", PacketPhase.Play, PacketDirection.Clientbound, 77);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 759)
        {
            id = 0x4C;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x4F;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x4D;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x51;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x55;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x57;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x5C;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x62;
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
