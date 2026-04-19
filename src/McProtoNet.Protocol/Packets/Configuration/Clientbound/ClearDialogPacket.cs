using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ClearDialog", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x11)]
public sealed partial class ClearDialogPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion) { }
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion) { }
}