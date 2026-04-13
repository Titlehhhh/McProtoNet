using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("KeepAlive", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x03)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x04)]
public sealed partial class KeepAlivePacket : IServerPacket
{
    public long KeepAliveId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedLong(KeepAliveId);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => KeepAliveId = reader.ReadSignedLong();

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}