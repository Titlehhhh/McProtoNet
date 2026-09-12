using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.reset_score", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityName", "string")]
[PacketField("ObjectiveName", "string?")]
public sealed partial record ResetScorePacket(string EntityName, string? ObjectiveName) : IPacket<ResetScorePacket>, IPacket
{
    public static ResetScorePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResetScorePacket>(protocolVersion);
        var entityName = reader.ReadString();
        string? objectiveName = null;
        if (reader.ReadBoolean())
            objectiveName = reader.ReadString();
        return new ResetScorePacket(entityName, objectiveName);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResetScorePacket>(protocolVersion);
        writer.WriteString(EntityName);
        writer.WriteBoolean(ObjectiveName is not null);
        if (ObjectiveName is { } objectiveNameValue)
            writer.WriteString(objectiveNameValue);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityName");
        writer.WriteStringValue(EntityName);
        if (ObjectiveName is { } objectiveNameValue)
        {
            writer.WritePropertyName("ObjectiveName");
            writer.WriteStringValue(objectiveNameValue);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.reset_score", "ResetScore", PacketPhase.Play, PacketDirection.Clientbound, 81);

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
