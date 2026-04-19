using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SpawnEntityPainting", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x03)]
[PacketId(751, 758, 0x03)]
public sealed partial class SpawnEntityPaintingPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Guid EntityUUID { get; set; }
    public int Title { get; set; }
    public Position Location { get; set; }
    public byte Direction { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteUUID(EntityUUID);
        writer.WriteVarInt(Title);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteUnsignedByte(Direction);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        EntityUUID = reader.ReadUUID();
        Title = reader.ReadVarInt();
        Location = reader.ReadType<Position>(protocolVersion);
        Direction = reader.ReadUnsignedByte();
    }
}