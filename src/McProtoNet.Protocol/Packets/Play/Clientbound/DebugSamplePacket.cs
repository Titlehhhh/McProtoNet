using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("DebugSample", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 769, 0x1B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x1A)]
public sealed partial class DebugSamplePacket : IServerPacket
{
    public long[] Sample { get; set; }
    public int Type { get;set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteArray<long>(Sample);
        writer.WriteVarInt(Type);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Sample = reader.ReadArray<long>(LengthFormat.VarInt);
        Type = reader.ReadVarInt();
    }
}