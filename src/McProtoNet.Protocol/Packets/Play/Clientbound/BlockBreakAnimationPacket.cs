using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("BlockBreakAnimation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x08)]
[PacketId(751, 754, 0x08)]
[PacketId(755, 758, 0x09)]
[PacketId(759, 761, 0x06)]
[PacketId(762, 763, 0x07)]
[PacketId(764, 769, 0x06)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x05)]
public sealed partial class BlockBreakAnimationPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Position Location { get; set; }
    public sbyte DestroyStage { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteSignedByte(DestroyStage);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        Location = reader.ReadType<Position>(protocolVersion);
        DestroyStage = reader.ReadSignedByte();
    }
}