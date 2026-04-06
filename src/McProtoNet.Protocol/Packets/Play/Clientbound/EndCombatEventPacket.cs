using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EndCombatEvent", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class EndCombatEventPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, 762),
        new(763, MinecraftVersion.LatestProtocol),
    };

    public int Duration { get; set; }
    public int EntityId { get; set; }



    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 762:
            {
                writer.WriteVarInt(Duration);
                writer.WriteSignedInt(EntityId);
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Duration);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EndCombatEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= 762:
            {
                Duration = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                Duration = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.EndCombatEvent), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);


}
