using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.damage_event", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("SourceTypeId", "int")]
[PacketField("SourceCauseId", "int")]
[PacketField("SourceDirectId", "int")]
[PacketField("SourcePosition", "Vec3f64?")]
public sealed partial record DamageEventPacket(int EntityId, int SourceTypeId, int SourceCauseId, int SourceDirectId, Vec3f64? SourcePosition) : IPacket<DamageEventPacket>, IPacket
{
    public static DamageEventPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DamageEventPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var sourceTypeId = reader.ReadVarInt();
        var sourceCauseId = reader.ReadVarInt();
        var sourceDirectId = reader.ReadVarInt();
        Vec3f64? sourcePosition = null;
        if (reader.ReadBoolean())
            sourcePosition = reader.ReadType<Vec3f64>(protocolVersion);
        return new DamageEventPacket(entityId, sourceTypeId, sourceCauseId, sourceDirectId, sourcePosition);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DamageEventPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(SourceTypeId);
        writer.WriteVarInt(SourceCauseId);
        writer.WriteVarInt(SourceDirectId);
        writer.WriteBoolean(SourcePosition is not null);
        if (SourcePosition is { } sourcePositionValue)
            writer.WriteType<Vec3f64>(sourcePositionValue, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.damage_event", "DamageEvent", PacketPhase.Play, PacketDirection.Clientbound, 24);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x19;
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
