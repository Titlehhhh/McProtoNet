using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;
using McProtoNet.Protocol.Extensions;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DeathCombatEvent", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class DeathCombatEventPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 762),
        new(763, 764),
        new(765, MinecraftVersion.LatestProtocol),
    };

    public int PlayerId { get; set; }
    public int EntityId { get; set; }
    public string Message { get; set; }
    public string Message { get; set; }



    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 762:
            {
                writer.WriteVarInt(PlayerId);
                writer.WriteSignedInt(EntityId);
                writer.WriteString(Message);
                return;
            }
            case >= 763 and <= 764:
            {
                writer.WriteVarInt(PlayerId);
                writer.WriteString(Message);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(PlayerId);
                writer.WriteAnonymousNbtTag(Message ?? throw new InvalidOperationException("DeathCombatEvent Message missing."), protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DeathCombatEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 762:
            {
                PlayerId = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
                Message = reader.ReadString();
                return;
            }
            case >= 763 and <= 764:
            {
                PlayerId = reader.ReadVarInt();
                Message = reader.ReadString();
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                PlayerId = reader.ReadVarInt();
                Message = reader.ReadNbtTag(false);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.DeathCombatEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
