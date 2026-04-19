using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("FinishConfiguration", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x02)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class FinishConfigurationPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion) { }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion) { }
}