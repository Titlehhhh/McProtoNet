using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("FacePlayer", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class FacePlayerPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public int FeetEyes { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public bool IsEntity { get; set; }
    public int? EntityId { get; set; }

    public VFirst_764Fields? VFirst_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = VFirst_764 ?? throw new InvalidOperationException("FacePlayer VFirst_764 fields missing.");
                writer.WriteVarInt(FeetEyes);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteBoolean(IsEntity);
                if (IsEntity)
                {
                    writer.WriteVarInt(EntityId ?? throw new InvalidOperationException("FacePlayer entityId missing."));
                    writer.WriteString(fields.EntityFeetEyes ?? throw new InvalidOperationException("FacePlayer entity feet/eyes missing."));
                }
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("FacePlayer V765_Last fields missing.");
                writer.WriteVarInt(FeetEyes);
                writer.WriteDouble(X);
                writer.WriteDouble(Y);
                writer.WriteDouble(Z);
                writer.WriteBoolean(IsEntity);
                if (IsEntity)
                {
                    writer.WriteVarInt(EntityId ?? throw new InvalidOperationException("FacePlayer entityId missing."));
                    writer.WriteVarInt(fields.EntityFeetEyes ?? throw new InvalidOperationException("FacePlayer entity feet/eyes missing."));
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.FacePlayer), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 764:
            {
                var fields = new VFirst_764Fields();
                FeetEyes = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                IsEntity = reader.ReadBoolean();
                if (IsEntity)
                {
                    EntityId = reader.ReadVarInt();
                    fields.EntityFeetEyes = reader.ReadString();
                }
                VFirst_764 = fields;
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V765_LastFields();
                FeetEyes = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                IsEntity = reader.ReadBoolean();
                if (IsEntity)
                {
                    EntityId = reader.ReadVarInt();
                    fields.EntityFeetEyes = reader.ReadVarInt();
                }
                V765_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.FacePlayer), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_764Fields
    {
        public string? EntityFeetEyes { get; set; }
    }

    public struct V765_LastFields
    {
        public int? EntityFeetEyes { get; set; }
    }
}
