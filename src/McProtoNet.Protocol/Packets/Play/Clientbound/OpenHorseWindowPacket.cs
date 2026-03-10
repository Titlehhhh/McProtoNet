using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("OpenHorseWindow", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 765)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1F)]
[PacketId(751, 754, 0x1E)]
[PacketId(755, 758, 0x1F)]
[PacketId(759, 759, 0x1C)]
[PacketId(760, 760, 0x1E)]
[PacketId(761, 761, 0x1D)]
[PacketId(762, 763, 0x20)]
[PacketId(764, 765, 0x21)]
[PacketId(766, 767, 0x23)]
[PacketId(768, 769, 0x24)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x23)]
public sealed partial class OpenHorseWindowPacket : IServerPacket
{
    public int NbSlots { get; set; }
    public int EntityId { get; set; }

    public VFirst_765Fields? VFirst_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                var fields = VFirst_765 ?? throw new InvalidOperationException("OpenHorseWindowPacket 1-765 fields missing.");
                writer.WriteVarInt(NbSlots);
                writer.WriteSignedInt(EntityId);
                writer.WriteUnsignedByte(fields.WindowId);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("OpenHorseWindowPacket 766-last fields missing.");
                writer.WriteVarInt(NbSlots);
                writer.WriteSignedInt(EntityId);
                writer.WriteType<ContainerID>(fields.WindowId, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenHorseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        NbSlots = reader.ReadVarInt();
        EntityId = reader.ReadSignedInt();

        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                VFirst_765 = new VFirst_765Fields { WindowId = reader.ReadUnsignedByte() };
                V766_Last = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                V766_Last = new V766_LastFields { WindowId = reader.ReadType<ContainerID>(protocolVersion) };
                VFirst_765 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenHorseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_765Fields { public byte WindowId { get; set; } }
    public struct V766_LastFields { public ContainerID WindowId { get; set; } }
}