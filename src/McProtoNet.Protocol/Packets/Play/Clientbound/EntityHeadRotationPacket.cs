using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityHeadRotation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x3B)]
[PacketId(751, 754, 0x3A)]
[PacketId(755, 758, 0x3E)]
[PacketId(759, 759, 0x3C)]
[PacketId(760, 760, 0x3F)]
[PacketId(761, 761, 0x3E)]
[PacketId(762, 763, 0x42)]
[PacketId(764, 764, 0x44)]
[PacketId(765, 765, 0x46)]
[PacketId(766, 767, 0x48)]
[PacketId(768, 769, 0x4D)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x4C)]
public sealed partial class EntityHeadRotationPacket : IServerPacket
{
    public int EntityId { get; set; }
    public sbyte HeadYaw { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteSignedByte(HeadYaw);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        HeadYaw = reader.ReadSignedByte();
    }
}