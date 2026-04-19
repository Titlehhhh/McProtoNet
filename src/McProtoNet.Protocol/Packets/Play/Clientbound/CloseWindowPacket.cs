using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CloseWindow", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x13)]
[PacketId(751, 754, 0x12)]
[PacketId(755, 758, 0x13)]
[PacketId(759, 760, 0x10)]
[PacketId(761, 761, 0x0F)]
[PacketId(762, 763, 0x11)]
[PacketId(764, 769, 0x12)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x11)]
public sealed partial class CloseWindowPacket : IServerPacket
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