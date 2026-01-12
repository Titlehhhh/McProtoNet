using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SculkVibrationSignal", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SculkVibrationSignalPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 758),
    };

    public Position SourcePosition { get; set; }
    public string DestinationIdentifier { get; set; } = string.Empty;
    public Position? DestinationBlock { get; set; }
    public int? DestinationEntityId { get; set; }
    public int ArrivalTicks { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 758:
                writer.WritePosition(SourcePosition, protocolVersion);
                writer.WriteString(DestinationIdentifier);
                switch (DestinationIdentifier)
                {
                    case "block":
                        writer.WritePosition(DestinationBlock ?? throw new InvalidOperationException("SculkVibrationSignal destination block missing."), protocolVersion);
                        break;
                    case "entityId":
                        writer.WriteVarInt(DestinationEntityId ?? throw new InvalidOperationException("SculkVibrationSignal destination entityId missing."));
                        break;
                }
                writer.WriteVarInt(ArrivalTicks);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SculkVibrationSignal), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 758:
                SourcePosition = reader.ReadPosition(protocolVersion);
                DestinationIdentifier = reader.ReadString();
                switch (DestinationIdentifier)
                {
                    case "block":
                        DestinationBlock = reader.ReadPosition(protocolVersion);
                        break;
                    case "entityId":
                        DestinationEntityId = reader.ReadVarInt();
                        break;
                }
                ArrivalTicks = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SculkVibrationSignal), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
