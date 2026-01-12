using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("InitializeWorldBorder", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class InitializeWorldBorderPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 758),
        new(759, MinecraftVersion.LatestProtocol),
    };

    public double X { get; set; }
    public double Z { get; set; }
    public double OldDiameter { get; set; }
    public double NewDiameter { get; set; }
    public int PortalTeleportBoundary { get; set; }
    public int WarningBlocks { get; set; }
    public int WarningTime { get; set; }
    public long Speed { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 758:
            {
                writer.WriteDouble(X);
                writer.WriteDouble(Z);
                writer.WriteDouble(OldDiameter);
                writer.WriteDouble(NewDiameter);
                writer.WriteVarLong(Speed);
                writer.WriteVarInt(PortalTeleportBoundary);
                writer.WriteVarInt(WarningBlocks);
                writer.WriteVarInt(WarningTime);
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteDouble(X);
                writer.WriteDouble(Z);
                writer.WriteDouble(OldDiameter);
                writer.WriteDouble(NewDiameter);
                writer.WriteVarInt(Speed);
                writer.WriteVarInt(PortalTeleportBoundary);
                writer.WriteVarInt(WarningBlocks);
                writer.WriteVarInt(WarningTime);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.InitializeWorldBorder), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 758:
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarLong();
                PortalTeleportBoundary = reader.ReadVarInt();
                WarningBlocks = reader.ReadVarInt();
                WarningTime = reader.ReadVarInt();
                return;
            }
            case >= 759 and <= MinecraftVersion.LatestProtocol:
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarInt();
                PortalTeleportBoundary = reader.ReadVarInt();
                WarningBlocks = reader.ReadVarInt();
                WarningTime = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.InitializeWorldBorder), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
