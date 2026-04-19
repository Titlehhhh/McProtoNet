using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("KeepAlive", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x10)]
[PacketId(751, 754, 0x10)]
[PacketId(755, 758, 0x0F)]
[PacketId(759, 759, 0x11)]
[PacketId(760, 760, 0x12)]
[PacketId(761, 761, 0x11)]
[PacketId(762, 763, 0x12)]
[PacketId(764, 764, 0x14)]
[PacketId(765, 765, 0x15)]
[PacketId(766, 767, 0x18)]
[PacketId(768, 770, 0x1A)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x1B)]
public sealed partial class KeepAlivePacket : IClientPacket
{
    public long KeepAliveId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedLong(KeepAliveId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => KeepAliveId = reader.ReadSignedLong();
}