using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("BundleDelimiter", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(762, 763)]
[PacketId(762, MinecraftVersion.LatestProtocol, 0x00)]
public sealed partial class BundleDelimiterPacket : IPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

}