using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CombatEvent", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class CombatEventPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
    };

    public int Event { get; set; }
    public int? Duration { get; set; }
    public int? PlayerId { get; set; }
    public int? EntityId { get; set; }
    public string? Message { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                writer.WriteVarInt(Event);
                switch (Event)
                {
                    case 1:
                        writer.WriteVarInt(Duration ?? throw new InvalidOperationException("CombatEvent duration missing."));
                        writer.WriteSignedInt(EntityId ?? throw new InvalidOperationException("CombatEvent entityId missing."));
                        break;
                    case 2:
                        writer.WriteVarInt(PlayerId ?? throw new InvalidOperationException("CombatEvent playerId missing."));
                        writer.WriteSignedInt(EntityId ?? throw new InvalidOperationException("CombatEvent entityId missing."));
                        writer.WriteString(Message ?? throw new InvalidOperationException("CombatEvent message missing."));
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CombatEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                Event = reader.ReadVarInt();
                switch (Event)
                {
                    case 1:
                        Duration = reader.ReadVarInt();
                        EntityId = reader.ReadSignedInt();
                        break;
                    case 2:
                        PlayerId = reader.ReadVarInt();
                        EntityId = reader.ReadSignedInt();
                        Message = reader.ReadString();
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CombatEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
