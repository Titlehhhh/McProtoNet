using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("Pong", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x04)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x05)]
public sealed partial class PongPacket : IPacket
{
    public int Id { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedInt(Id);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Id = reader.ReadSignedInt();
}