using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[PacketInfo("EndCombatEvent", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x33)]
[PacketId(759, 759, 0x31)]
[PacketId(760, 760, 0x34)]
[PacketId(761, 761, 0x32)]
[PacketId(762, 763, 0x36)]
[PacketId(764, 765, 0x38)]
[PacketId(766, 767, 0x3A)]
[PacketId(768, 769, 0x3C)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x3B)]
public sealed partial class EndCombatEventPacket : IPacket
{
    public int Duration { get; set; }
    public int? EntityId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
            {
                writer.WriteVarInt(Duration);
                writer.WriteSignedInt(EntityId ?? throw new InvalidOperationException("EndCombatEventPacket 755-762 fields missing."));
                return;
            }
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Duration);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EndCombatEventPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
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
                ThrowHelper.ThrowProtocolNotSupported(nameof(EndCombatEventPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}