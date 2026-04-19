using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("PingResponse", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x34)]
[PacketId(766, 767, 0x36)]
[PacketId(768, 769, 0x38)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x37)]
public sealed partial class PingResponsePacket : IServerPacket
{
    public long Id { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedLong(Id);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Id = reader.ReadSignedLong();
}