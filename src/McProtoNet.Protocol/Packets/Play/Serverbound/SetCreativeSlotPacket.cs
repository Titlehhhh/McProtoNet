using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SetCreativeSlot", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x27)]
[PacketId(751, 758, 0x28)]
[PacketId(759, 759, 0x2A)]
[PacketId(760, 763, 0x2B)]
[PacketId(764, 764, 0x2E)]
[PacketId(765, 765, 0x2F)]
[PacketId(766, 767, 0x32)]
[PacketId(768, 768, 0x34)]
[PacketId(769, 770, 0x36)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x37)]
public sealed partial class SetCreativeSlotPacket : IClientPacket
{
    public short Slot { get; set; }

    public VFirst_765Fields? VFirst_765 { get; set; }
    public V766_769Fields? V766_769 { get; set; }
    public V770_LastFields? V770_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedShort(Slot);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                var fields = VFirst_765 ?? throw new InvalidOperationException("SetCreativeSlotPacket 1-765 fields missing.");
                writer.WriteType<Slot>(fields.Item, protocolVersion);
                return;
            }
            case >= 766 and <= 769:
            {
                var fields = V766_769 ?? throw new InvalidOperationException("SetCreativeSlotPacket 766-769 fields missing.");
                writer.WriteType<Slot>(fields.Item, protocolVersion);
                return;
            }
            case >= 770 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V770_Last ?? throw new InvalidOperationException("SetCreativeSlotPacket 770-last fields missing.");
                writer.WriteType<UntrustedSlot>(fields.Item, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetCreativeSlotPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Slot = reader.ReadSignedShort();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                VFirst_765 = new VFirst_765Fields { Item = reader.ReadType<Slot>(protocolVersion) };
                V766_769 = null;
                V770_Last = null;
                return;
            }
            case >= 766 and <= 769:
            {
                VFirst_765 = null;
                V766_769 = new V766_769Fields { Item = reader.ReadType<Slot>(protocolVersion) };
                V770_Last = null;
                return;
            }
            case >= 770 and <= MinecraftVersion.LatestProtocol:
            {
                VFirst_765 = null;
                V766_769 = null;
                V770_Last = new V770_LastFields { Item = reader.ReadType<UntrustedSlot>(protocolVersion) };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SetCreativeSlotPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_765Fields { public Slot Item { get; set; } }
    public struct V766_769Fields { public Slot Item { get; set; } }
    public struct V770_LastFields { public UntrustedSlot Item { get; set; } }
}