using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.scoreboard_objective", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Name", "string")]
[PacketField("Action", "int")]
[PacketField("Display", "ObjectiveDisplay?")]
public sealed partial record ScoreboardObjectivePacket(string Name, int Action, ObjectiveDisplay? Display) : IPacket<ScoreboardObjectivePacket>, IPacket
{
    public static ScoreboardObjectivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ScoreboardObjectivePacket>(protocolVersion);
        var name = reader.ReadString();
        var action = reader.ReadSignedByte();
        ObjectiveDisplay? display = default;
        if (action == 0 || action == 2)
        {
            var displayValue = reader.ReadType<ObjectiveDisplay>(protocolVersion);
            display = displayValue;
        }

        return new ScoreboardObjectivePacket(name, action, display);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ScoreboardObjectivePacket>(protocolVersion);
        writer.WriteString(Name);
        writer.WriteSignedByte((sbyte)Action);
        if ((sbyte)Action == 0 || (sbyte)Action == 2)
        {
            writer.WriteType<ObjectiveDisplay>((Display ?? throw new System.InvalidOperationException("Display is required at this protocol version.")), protocolVersion);
        }
        else if (Display is not null)
        {
            throw new System.InvalidOperationException("Display is set, but 'action' does not select it at this protocol version.");
        }
    }

    public static PacketIdentity Identity => new("play.toClient.scoreboard_objective", "ScoreboardObjective", PacketPhase.Play, PacketDirection.Clientbound, 84);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 759)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x56;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x54;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x5A;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x5C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x5E;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x64;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x68;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x6A;
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
