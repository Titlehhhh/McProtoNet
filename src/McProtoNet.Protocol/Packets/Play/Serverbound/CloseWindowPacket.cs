using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("CloseWindow", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0A)]
[PacketId(751, 754, 0x0A)]
[PacketId(755, 758, 0x09)]
[PacketId(759, 759, 0x0B)]
[PacketId(760, 760, 0x0C)]
[PacketId(761, 761, 0x0B)]
[PacketId(762, 763, 0x0C)]
[PacketId(764, 765, 0x0E)]
[PacketId(766, 767, 0x0F)]
[PacketId(768, 770, 0x11)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x12)]
public sealed partial class CloseWindowPacket : IClientPacket
{
    public byte WindowId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUnsignedByte(WindowId);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CloseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        WindowId = reader.ReadUnsignedByte();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CloseWindowPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}