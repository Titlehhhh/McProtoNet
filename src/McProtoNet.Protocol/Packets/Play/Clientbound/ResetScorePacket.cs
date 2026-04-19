using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ResetScore", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x42)]
[PacketId(766, 767, 0x44)]
[PacketId(768, 769, 0x49)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x48)]
public sealed partial class ResetScorePacket : IServerPacket
{
    public string EntityName { get; set; }
    public string? ObjectiveName { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(EntityName);
        writer.WriteBoolean(ObjectiveName != null);
        if (ObjectiveName != null)
            writer.WriteString(ObjectiveName);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityName = reader.ReadString();
        bool hasObjective = reader.ReadBoolean();
        ObjectiveName = hasObjective ? reader.ReadString() : null;
    }
}