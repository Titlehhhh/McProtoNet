using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorder", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class WorldBorderPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
    };

    public int Action { get; set; }
    public double? Radius { get; set; }
    public double? X { get; set; }
    public double? Z { get; set; }
    public double? OldRadius { get; set; }
    public double? NewRadius { get; set; }
    public long? Speed { get; set; }
    public int? PortalBoundary { get; set; }
    public int? WarningTime { get; set; }
    public int? WarningBlocks { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                writer.WriteVarInt(Action);
                switch (Action)
                {
                    case 0:
                        writer.WriteDouble(Radius ?? throw new InvalidOperationException("WorldBorder radius missing."));
                        break;
                    case 1:
                        writer.WriteDouble(OldRadius ?? throw new InvalidOperationException("WorldBorder old_radius missing."));
                        writer.WriteDouble(NewRadius ?? throw new InvalidOperationException("WorldBorder new_radius missing."));
                        writer.WriteVarLong(Speed ?? throw new InvalidOperationException("WorldBorder speed missing."));
                        break;
                    case 2:
                        writer.WriteDouble(X ?? throw new InvalidOperationException("WorldBorder x missing."));
                        writer.WriteDouble(Z ?? throw new InvalidOperationException("WorldBorder z missing."));
                        break;
                    case 3:
                        writer.WriteDouble(X ?? throw new InvalidOperationException("WorldBorder x missing."));
                        writer.WriteDouble(Z ?? throw new InvalidOperationException("WorldBorder z missing."));
                        writer.WriteDouble(OldRadius ?? throw new InvalidOperationException("WorldBorder old_radius missing."));
                        writer.WriteDouble(NewRadius ?? throw new InvalidOperationException("WorldBorder new_radius missing."));
                        writer.WriteVarLong(Speed ?? throw new InvalidOperationException("WorldBorder speed missing."));
                        writer.WriteVarInt(PortalBoundary ?? throw new InvalidOperationException("WorldBorder portalBoundary missing."));
                        writer.WriteVarInt(WarningTime ?? throw new InvalidOperationException("WorldBorder warning_time missing."));
                        writer.WriteVarInt(WarningBlocks ?? throw new InvalidOperationException("WorldBorder warning_blocks missing."));
                        break;
                    case 4:
                        writer.WriteVarInt(WarningTime ?? throw new InvalidOperationException("WorldBorder warning_time missing."));
                        break;
                    case 5:
                        writer.WriteVarInt(WarningBlocks ?? throw new InvalidOperationException("WorldBorder warning_blocks missing."));
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldBorder), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                Action = reader.ReadVarInt();
                switch (Action)
                {
                    case 0:
                        Radius = reader.ReadDouble();
                        break;
                    case 1:
                        OldRadius = reader.ReadDouble();
                        NewRadius = reader.ReadDouble();
                        Speed = reader.ReadVarLong();
                        break;
                    case 2:
                        X = reader.ReadDouble();
                        Z = reader.ReadDouble();
                        break;
                    case 3:
                        X = reader.ReadDouble();
                        Z = reader.ReadDouble();
                        OldRadius = reader.ReadDouble();
                        NewRadius = reader.ReadDouble();
                        Speed = reader.ReadVarLong();
                        PortalBoundary = reader.ReadVarInt();
                        WarningTime = reader.ReadVarInt();
                        WarningBlocks = reader.ReadVarInt();
                        break;
                    case 4:
                        WarningTime = reader.ReadVarInt();
                        break;
                    case 5:
                        WarningBlocks = reader.ReadVarInt();
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldBorder), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
