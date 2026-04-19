using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("StartConfiguration", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 764, 0x65)]
[PacketId(765, 765, 0x67)]
[PacketId(766, 767, 0x69)]
[PacketId(768, 769, 0x70)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x6F)]
public sealed partial class StartConfigurationPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
    }
}