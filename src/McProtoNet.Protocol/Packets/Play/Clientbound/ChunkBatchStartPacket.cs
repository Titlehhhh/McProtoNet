using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ChunkBatchStart", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 769, 0x0D)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x0C)]
public sealed partial class ChunkBatchStartPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion) { }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion) { }
}