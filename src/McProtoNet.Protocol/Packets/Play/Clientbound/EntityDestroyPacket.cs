using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityDestroy", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[ProtocolSupport(756, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x37)]
[PacketId(751, 754, 0x36)]
[PacketId(756, 758, 0x3A)]
[PacketId(759, 759, 0x38)]
[PacketId(760, 760, 0x3B)]
[PacketId(761, 761, 0x3A)]
[PacketId(762, 763, 0x3E)]
[PacketId(764, 765, 0x40)]
[PacketId(766, 767, 0x42)]
[PacketId(768, 769, 0x47)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x46)]
public sealed partial class EntityDestroyPacket : IServerPacket
{
    public int[] EntityIds { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarIntArray(EntityIds);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityIds = reader.ReadVarIntArray(LengthFormat.VarInt);
    }
}