using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DamageEvent", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
[PacketId(762, 763, 0x18)]
[PacketId(764, 765, 0x19)]
[PacketId(766, 769, 0x1A)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x19)]
public sealed partial class DamageEventPacket : IServerPacket
{
    public int EntityId { get; set; }
    public int SourceTypeId { get; set; }
    public int SourceCauseId { get; set; }
    public int SourceDirectId { get; set; }
    public Vec3f64? SourcePosition { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(SourceTypeId);
        writer.WriteVarInt(SourceCauseId);
        writer.WriteVarInt(SourceDirectId);
        writer.WriteType(SourcePosition, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        SourceTypeId = reader.ReadVarInt();
        SourceCauseId = reader.ReadVarInt();
        SourceDirectId = reader.ReadVarInt();
        SourcePosition = reader.ReadType<Vec3f64>(protocolVersion);
    }
}