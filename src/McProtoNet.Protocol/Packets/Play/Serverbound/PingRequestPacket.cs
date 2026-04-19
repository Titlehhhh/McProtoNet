using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PingRequest", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 764, 0x1D)]
[PacketId(765, 765, 0x1E)]
[PacketId(766, 767, 0x21)]
[PacketId(768, 768, 0x23)]
[PacketId(769, 770, 0x24)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x25)]
public sealed partial class PingRequestPacket : IClientPacket
{
    public long Id { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedLong(Id);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Id = reader.ReadSignedLong();
    }
}