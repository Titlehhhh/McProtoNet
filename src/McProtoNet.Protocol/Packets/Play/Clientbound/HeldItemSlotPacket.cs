using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("HeldItemSlot", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 768)]
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x3F)]
[PacketId(751, 754, 0x3F)]
[PacketId(755, 758, 0x48)]
[PacketId(759, 759, 0x47)]
[PacketId(760, 760, 0x4A)]
[PacketId(761, 761, 0x49)]
[PacketId(762, 763, 0x4D)]
[PacketId(764, 764, 0x4F)]
[PacketId(765, 765, 0x51)]
[PacketId(766, 767, 0x53)]
[PacketId(768, 769, 0x63)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x62)]
public sealed partial class HeldItemSlotPacket : IServerPacket
{
    public VFirst_768Fields? VFirst_768 { get; set; }
    public V769_LastFields? V769_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 768:
            {
                var fields = VFirst_768 ?? throw new InvalidOperationException("HeldItemSlotPacket first-768 fields missing.");
                writer.WriteSignedByte(fields.Slot);
                return;
            }
            case >= 769 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V769_Last ?? throw new InvalidOperationException("HeldItemSlotPacket 769-last fields missing.");
                writer.WriteVarInt(fields.Slot);
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
            case >= MinecraftVersion.StartProtocol and <= 768:
                VFirst_768 = new VFirst_768Fields { Slot = reader.ReadSignedByte() };
                return;
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                V769_Last = new V769_LastFields { Slot = reader.ReadVarInt() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(HeldItemSlotPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_768Fields { public sbyte Slot { get; set; } }
    public struct V769_LastFields { public int Slot { get; set; } }
}