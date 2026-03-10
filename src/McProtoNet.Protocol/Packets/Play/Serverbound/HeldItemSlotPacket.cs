using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("HeldItemSlot", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x24)]
[PacketId(751, 758, 0x25)]
[PacketId(759, 759, 0x27)]
[PacketId(760, 763, 0x28)]
[PacketId(764, 764, 0x2B)]
[PacketId(765, 765, 0x2C)]
[PacketId(766, 767, 0x2F)]
[PacketId(768, 768, 0x31)]
[PacketId(769, 770, 0x33)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x34)]
public sealed partial class HeldItemSlotPacket : IClientPacket
{
    public int ContainerId { get; set; }
    public short SlotId { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(ContainerId);
                writer.WriteSignedShort(SlotId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(HeldItemSlotPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                ContainerId = reader.ReadVarInt();
                SlotId = reader.ReadSignedShort();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(HeldItemSlotPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}