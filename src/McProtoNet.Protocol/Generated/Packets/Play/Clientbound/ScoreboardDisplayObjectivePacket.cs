using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.scoreboard_display_objective", "ScoreboardDisplayObjective", PacketPhase.Play, PacketDirection.Clientbound, 84);

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
