using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PlayerLoaded", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[PacketId(769, 770, 0x2A)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2B)]
public sealed partial class PlayerLoadedPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion) { }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion) { }
}