using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CookieRequest", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 769, 0x16)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x15)]
public sealed partial class CookieRequestPacket : IServerPacket
{
    public string Cookie { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Cookie);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Cookie = reader.ReadString();
    }
}