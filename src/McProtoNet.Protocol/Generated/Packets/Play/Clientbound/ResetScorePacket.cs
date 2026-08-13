using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.reset_score", "ResetScore", PacketPhase.Play, PacketDirection.Clientbound, 70);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x42;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x44;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x48;
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
