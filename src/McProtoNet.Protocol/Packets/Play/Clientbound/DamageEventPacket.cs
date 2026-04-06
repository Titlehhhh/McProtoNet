using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DamageEvent", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class DamageEventPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(762, MinecraftVersion.LatestProtocol),
    };

    public int EntityId { get; set; }
    public int SourceTypeId { get; set; }
    public int SourceCauseId { get; set; }
    public int SourceDirectId { get; set; }
    public Vec3f64? SourcePosition { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 762 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                writer.WriteVarInt(SourceTypeId);
                writer.WriteVarInt(SourceCauseId);
                writer.WriteVarInt(SourceDirectId);
                if (SourcePosition is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVec3f64(SourcePosition.Value, protocolVersion);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DamageEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 762 and <= MinecraftVersion.LatestProtocol:
                EntityId = reader.ReadVarInt();
                SourceTypeId = reader.ReadVarInt();
                SourceCauseId = reader.ReadVarInt();
                SourceDirectId = reader.ReadVarInt();
                SourcePosition = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadVec3f64(protocolVersion));
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DamageEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
