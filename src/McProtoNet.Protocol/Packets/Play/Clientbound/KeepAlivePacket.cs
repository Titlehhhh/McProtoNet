using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("KeepAlive", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x20)]
[PacketId(751, 754, 0x1F)]
[PacketId(755, 758, 0x21)]
[PacketId(759, 759, 0x1E)]
[PacketId(760, 760, 0x20)]
[PacketId(761, 761, 0x1F)]
[PacketId(762, 763, 0x23)]
[PacketId(764, 765, 0x24)]
[PacketId(766, 767, 0x26)]
[PacketId(768, 769, 0x27)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x26)]
public sealed partial class KeepAlivePacket : IServerPacket
{
    public long KeepAliveId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedLong(KeepAliveId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => KeepAliveId = reader.ReadSignedLong();
}