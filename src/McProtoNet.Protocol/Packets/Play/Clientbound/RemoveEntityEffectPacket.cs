using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RemoveEntityEffect", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x38)]
[PacketId(751, 754, 0x37)]
[PacketId(755, 758, 0x3B)]
[PacketId(759, 759, 0x39)]
[PacketId(760, 760, 0x3C)]
[PacketId(761, 761, 0x3B)]
[PacketId(762, 763, 0x3F)]
[PacketId(764, 765, 0x41)]
[PacketId(766, 767, 0x43)]
[PacketId(768, 769, 0x48)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x47)]
public sealed partial class RemoveEntityEffectPacket : IServerPacket
{
    public int EntityId { get; set; }
    public int? EffectId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedInt(EntityId);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 757:
                writer.WriteSignedByte((sbyte)EffectId.Value);
                return;
            case >= 758 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EffectId.Value);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(RemoveEntityEffectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadSignedInt();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 757:
                EffectId = reader.ReadSignedByte();
                return;
            case >= 758 and <= MinecraftVersion.LatestProtocol:
                EffectId = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(RemoveEntityEffectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}