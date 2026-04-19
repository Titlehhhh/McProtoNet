using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChunkBatchReceived", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x07)]
[PacketId(766, 767, 0x08)]
[PacketId(768, 770, 0x09)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x0A)]
public sealed partial class ChunkBatchReceivedPacket : IClientPacket
{
    public float ChunksPerTick { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteFloat(ChunksPerTick);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => ChunksPerTick = reader.ReadFloat();
}