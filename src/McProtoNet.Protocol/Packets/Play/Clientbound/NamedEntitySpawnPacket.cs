using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("NamedEntitySpawn", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 763)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x04)]
[PacketId(751, 758, 0x04)]
[PacketId(759, 761, 0x02)]
[PacketId(762, 763, 0x03)]
public sealed partial class NamedEntitySpawnPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Guid PlayerUUID { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteUUID(PlayerUUID);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteSignedByte(Yaw);
        writer.WriteSignedByte(Pitch);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        PlayerUUID = reader.ReadUUID();
        X = reader.ReadDouble();
        Y = reader.ReadDouble();
        Z = reader.ReadDouble();
        Yaw = reader.ReadSignedByte();
        Pitch = reader.ReadSignedByte();
    }
}