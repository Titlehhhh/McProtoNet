using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("OpenHorseWindow", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
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
    public byte WindowId { get; set; }
    public int NbSlots { get; set; }
    public int EntityId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUnsignedByte(WindowId);
        writer.WriteVarInt(NbSlots);
        writer.WriteSignedInt(EntityId);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenHorseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        WindowId = reader.ReadUnsignedByte();
        NbSlots = reader.ReadVarInt();
        EntityId = reader.ReadSignedInt();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenHorseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}