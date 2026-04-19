using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UseItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2E)]
[PacketId(751, 758, 0x2F)]
[PacketId(759, 759, 0x31)]
[PacketId(760, 763, 0x32)]
[PacketId(764, 764, 0x35)]
[PacketId(765, 765, 0x36)]
[PacketId(766, 767, 0x39)]
[PacketId(768, 768, 0x3B)]
[PacketId(769, 769, 0x3D)]
[PacketId(770, 770, 0x3F)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x40)]
public sealed partial class UseItemPacket : IClientPacket
{
    public int Hand { get; set; }
    public int? Sequence { get; set; }
    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                writer.WriteVarInt(Hand);
                return;
            }
            case >= 759 and <= 766:
            {
                writer.WriteVarInt(Hand);
                writer.WriteVarInt(Sequence.Value);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V767_Last ?? throw new InvalidOperationException("UseItemPacket 767-last fields missing.");
                writer.WriteVarInt(Hand);
                writer.WriteVarInt(Sequence.Value);
                writer.WriteType<Vec2f>(fields.Rotation, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UseItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                Hand = reader.ReadVarInt();
                Sequence = null;
                V767_Last = null;
                return;
            }
            case >= 759 and <= 766:
            {
                Hand = reader.ReadVarInt();
                Sequence = reader.ReadVarInt();
                V767_Last = null;
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                Hand = reader.ReadVarInt();
                Sequence = reader.ReadVarInt();
                V767_Last = new V767_LastFields { Rotation = reader.ReadType<Vec2f>(protocolVersion) };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UseItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V767_LastFields
    {
        public Vec2f Rotation { get; set; }
    }
}