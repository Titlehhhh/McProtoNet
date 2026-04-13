using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Tags", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 764, 0x08)]
[PacketId(765, 765, 0x09)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x0D)]
public sealed partial class TagsPacket : IServerPacket
{
    public Tags[] Tags { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteArray<Tags>(Tags, protocolVersion);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Tags = reader.ReadArray<Tags>(LengthFormat.VarInt, protocolVersion);

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}