using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorder", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x3D)]
[PacketId(751, 754, 0x3D)]
public sealed partial class WorldBorderPacket : IServerPacket
{
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

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Action);
        switch (Action)
        {
            case 0:
                writer.WriteDouble(Radius ?? throw new InvalidOperationException("Radius is required for action 0."));
                break;
            case 1:
                writer.WriteDouble(OldRadius ?? throw new InvalidOperationException("OldRadius is required for action 1."));
                writer.WriteDouble(NewRadius ?? throw new InvalidOperationException("NewRadius is required for action 1."));
                writer.WriteVarLong(Speed ?? throw new InvalidOperationException("Speed is required for action 1."));
                break;
            case 2:
                writer.WriteDouble(X ?? throw new InvalidOperationException("X is required for action 2."));
                writer.WriteDouble(Z ?? throw new InvalidOperationException("Z is required for action 2."));
                break;
            case 3:
                writer.WriteDouble(X ?? throw new InvalidOperationException("X is required for action 3."));
                writer.WriteDouble(Z ?? throw new InvalidOperationException("Z is required for action 3."));
                writer.WriteDouble(OldRadius ?? throw new InvalidOperationException("OldRadius is required for action 3."));
                writer.WriteDouble(NewRadius ?? throw new InvalidOperationException("NewRadius is required for action 3."));
                writer.WriteVarLong(Speed ?? throw new InvalidOperationException("Speed is required for action 3."));
                writer.WriteVarInt(PortalBoundary ?? throw new InvalidOperationException("PortalBoundary is required for action 3."));
                writer.WriteVarInt(WarningTime ?? throw new InvalidOperationException("WarningTime is required for action 3."));
                writer.WriteVarInt(WarningBlocks ?? throw new InvalidOperationException("WarningBlocks is required for action 3."));
                break;
            case 4:
                writer.WriteVarInt(WarningTime ?? throw new InvalidOperationException("WarningTime is required for action 4."));
                break;
            case 5:
                writer.WriteVarInt(WarningBlocks ?? throw new InvalidOperationException("WarningBlocks is required for action 5."));
                break;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
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
    }
}