using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderLerpSize", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class WorldBorderLerpSizePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 758),
        new(759, MinecraftVersion.LatestProtocol),
    };

    public double OldDiameter { get; set; }
    public double NewDiameter { get; set; }

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759_LastFields? V759_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("WorldBorderLerpSize VFirst_758 missing.");
                writer.WriteDouble(OldDiameter);
                writer.WriteDouble(NewDiameter);
                writer.WriteVarLong(fields.Speed);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V759_Last ?? throw new InvalidOperationException("WorldBorderLerpSize V759_Last missing.");
                writer.WriteDouble(OldDiameter);
                writer.WriteDouble(NewDiameter);
                writer.WriteVarInt(fields.Speed);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldBorderLerpSize), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 758:
            {
                var fields = new VFirst_758Fields();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                fields.Speed = reader.ReadVarLong();
                VFirst_758 = fields;
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V759_LastFields();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                fields.Speed = reader.ReadVarInt();
                V759_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldBorderLerpSize), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public long Speed { get; set; }
    }

    public struct V759_LastFields
    {
        public int Speed { get; set; }
    }
}
