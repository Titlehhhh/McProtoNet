using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("UseItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[ProtocolSupport(759, 766)]
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
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
    public V759_766Fields? V759_766 { get; set; }
    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
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
                var fields = V759_766 ?? throw new InvalidOperationException("UseItemPacket 759-766 fields missing.");
                writer.WriteVarInt(fields.Sequence);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Hand);
                var fields = V767_Last ?? throw new InvalidOperationException("UseItemPacket 767-last fields missing.");
                writer.WriteVarInt(fields.Sequence);
                writer.WriteType(fields.Rotation, protocolVersion);
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
                V759_766 = null;
                V767_Last = null;
                return;
            }
            case >= 759 and <= 766:
            {
                Hand = reader.ReadVarInt();
                V759_766 = new V759_766Fields { Sequence = reader.ReadVarInt() };
                V767_Last = null;
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                Hand = reader.ReadVarInt();
                V767_Last = new V767_LastFields { Sequence = reader.ReadVarInt(), Rotation = reader.ReadType<Vec2f>(protocolVersion) };
                V759_766 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(UseItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_766Fields { public int Sequence { get; set; } }
    public struct V767_LastFields { public int Sequence { get; set; } public Vec2f Rotation { get; set; } }
}