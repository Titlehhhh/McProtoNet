using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("EntityAction", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1C)]
[PacketId(751, 754, 0x1C)]
[PacketId(755, 758, 0x1B)]
[PacketId(759, 759, 0x1D)]
[PacketId(760, 760, 0x1E)]
[PacketId(761, 761, 0x1D)]
[PacketId(762, 763, 0x1E)]
[PacketId(764, 764, 0x21)]
[PacketId(765, 765, 0x22)]
[PacketId(766, 767, 0x25)]
[PacketId(768, 768, 0x27)]
[PacketId(769, 770, 0x28)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x29)]
public sealed partial class EntityActionPacket : IClientPacket
{
    public int EntityId { get; set; }
    public int ActionId { get; set; }
    public int JumpBoost { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteVarInt(ActionId);
        writer.WriteVarInt(JumpBoost);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                return;
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EntityActionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        ActionId = reader.ReadVarInt();
        JumpBoost = reader.ReadVarInt();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 770:
                {
                    return;
                }
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                {
                    return;
                }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EntityActionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}