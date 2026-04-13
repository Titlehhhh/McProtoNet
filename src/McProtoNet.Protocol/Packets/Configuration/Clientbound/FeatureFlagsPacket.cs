using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("FeatureFlags", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 764, 0x07)]
[PacketId(765, 765, 0x08)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x0C)]
public sealed partial class FeatureFlagsPacket : IServerPacket
{
    public string[] Features { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteArray(Features);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Features = reader.ReadArray<string>(LengthFormat.VarInt);

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}