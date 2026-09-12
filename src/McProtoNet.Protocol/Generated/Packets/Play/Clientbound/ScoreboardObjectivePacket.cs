using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Name");
        writer.WriteStringValue(Name);
        writer.WritePropertyName("Action");
        writer.WriteNumberValue(Action);
        if (Display is { } displayValue)
        {
            writer.WritePropertyName("Display");
            displayValue.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.scoreboard_objective", "ScoreboardObjective", PacketPhase.Play, PacketDirection.Clientbound, 86);

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
