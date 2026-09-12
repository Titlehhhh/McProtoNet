using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("TeamName");
        writer.WriteStringValue(TeamName);
        writer.WritePropertyName("Action");
        Action.WriteJson(writer);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.teams", "Teams", PacketPhase.Play, PacketDirection.Clientbound, 113);

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
