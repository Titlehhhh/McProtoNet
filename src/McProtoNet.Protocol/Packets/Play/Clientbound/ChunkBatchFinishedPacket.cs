using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ChunkBatchFinished", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 769, 0x0C)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x0B)]
public sealed partial class ChunkBatchFinishedPacket : IServerPacket
{
    public int BatchSize { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(BatchSize);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        BatchSize = reader.ReadVarInt();
    }
}