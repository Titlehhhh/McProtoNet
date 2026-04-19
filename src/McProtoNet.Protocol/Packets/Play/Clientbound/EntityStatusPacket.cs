using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityStatus", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1B)]
[PacketId(751, 754, 0x1A)]
[PacketId(755, 758, 0x1B)]
[PacketId(759, 759, 0x18)]
[PacketId(760, 760, 0x1A)]
[PacketId(761, 761, 0x19)]
[PacketId(762, 763, 0x1C)]
[PacketId(764, 765, 0x1D)]
[PacketId(766, 769, 0x1F)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x1E)]
public sealed partial class EntityStatusPacket : IServerPacket
{
    public int EntityId { get; set; }
    public sbyte EntityStatus { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedInt(EntityId);
        writer.WriteSignedByte(EntityStatus);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadSignedInt();
        EntityStatus = reader.ReadSignedByte();
    }
}