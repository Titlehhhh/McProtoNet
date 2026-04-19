using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

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
    public int CameraId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(CameraId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => CameraId = reader.ReadVarInt();
}