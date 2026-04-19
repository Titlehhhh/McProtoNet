using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Ping", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x30)]
[PacketId(759, 759, 0x2D)]
[PacketId(760, 760, 0x2F)]
[PacketId(761, 761, 0x2E)]
[PacketId(762, 763, 0x32)]
[PacketId(764, 765, 0x33)]
[PacketId(766, 767, 0x35)]
[PacketId(768, 769, 0x37)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x36)]
public sealed partial class PingPacket : IServerPacket
{
    public int Id { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedInt(Id);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Id = reader.ReadSignedInt();
}