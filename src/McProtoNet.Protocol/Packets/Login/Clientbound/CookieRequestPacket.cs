using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("CookieRequest", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x05)]
public sealed partial class CookieRequestPacket : IServerPacket
{
    public string Name { get; set; } = string.Empty;

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteString(Name);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadString();

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}