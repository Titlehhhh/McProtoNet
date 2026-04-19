using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EnterCombatEvent", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x34)]
[PacketId(759, 759, 0x32)]
[PacketId(760, 760, 0x35)]
[PacketId(761, 761, 0x33)]
[PacketId(762, 763, 0x37)]
[PacketId(764, 765, 0x39)]
[PacketId(766, 767, 0x3B)]
[PacketId(768, 769, 0x3D)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x3C)]
public sealed partial class EnterCombatEventPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
    }
}