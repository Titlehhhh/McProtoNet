using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityVelocity", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x46)]
[PacketId(751, 754, 0x46)]
[PacketId(755, 759, 0x4F)]
[PacketId(760, 760, 0x52)]
[PacketId(761, 761, 0x50)]
[PacketId(762, 763, 0x54)]
[PacketId(764, 764, 0x56)]
[PacketId(765, 765, 0x58)]
[PacketId(766, 767, 0x5A)]
[PacketId(768, 769, 0x5F)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x5E)]
public sealed partial class EntityVelocityPacket : IServerPacket
{
    public int EntityId { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort(VelocityX);
        writer.WriteSignedShort(VelocityY);
        writer.WriteSignedShort(VelocityZ);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        VelocityX = reader.ReadSignedShort();
        VelocityY = reader.ReadSignedShort();
        VelocityZ = reader.ReadSignedShort();
    }
}