using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CraftProgressBar", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x15)]
[PacketId(751, 754, 0x14)]
[PacketId(755, 758, 0x15)]
[PacketId(759, 760, 0x12)]
[PacketId(761, 761, 0x11)]
[PacketId(762, 763, 0x13)]
[PacketId(764, 769, 0x14)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x13)]
public sealed partial class CraftProgressBarPacket : IServerPacket
{
    public byte WindowId { get; set; }
    public short Property { get; set; }
    public short Value { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUnsignedByte(WindowId);
        writer.WriteSignedShort(Property);
        writer.WriteSignedShort(Value);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CraftProgressBarPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        WindowId = reader.ReadUnsignedByte();
        Property = reader.ReadSignedShort();
        Value = reader.ReadSignedShort();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CraftProgressBarPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}