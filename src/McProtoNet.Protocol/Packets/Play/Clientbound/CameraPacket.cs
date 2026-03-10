using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Camera", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x3E)]
[PacketId(751, 754, 0x3E)]
[PacketId(755, 758, 0x47)]
[PacketId(759, 759, 0x46)]
[PacketId(760, 760, 0x49)]
[PacketId(761, 761, 0x48)]
[PacketId(762, 763, 0x4C)]
[PacketId(764, 764, 0x4E)]
[PacketId(765, 765, 0x50)]
[PacketId(766, 767, 0x52)]
[PacketId(768, 769, 0x57)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x56)]
public sealed partial class CameraPacket : IServerPacket
{
    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(CameraId);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CameraPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                CameraId = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(CameraPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public int CameraId { get; set; }
}