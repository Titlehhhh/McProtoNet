using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SpawnEntityLiving", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x02)]
[PacketId(751, 758, 0x02)]
public sealed partial class SpawnEntityLivingPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Guid EntityUUID { get; set; }
    public int Type { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public sbyte HeadPitch { get; set; }
    public short VelocityX { get; set; }
    public short VelocityY { get; set; }
    public short VelocityZ { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(EntityId)
                 .WriteUUID(EntityUUID)
                 .WriteVarInt(Type)
                 .WriteDouble(X)
                 .WriteDouble(Y)
                 .WriteDouble(Z)
                 .WriteSignedByte(Yaw)
                 .WriteSignedByte(Pitch)
                 .WriteSignedByte(HeadPitch)
                 .WriteSignedShort(VelocityX)
                 .WriteSignedShort(VelocityY)
                 .WriteSignedShort(VelocityZ);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        EntityUUID = reader.ReadUUID();
        Type = reader.ReadVarInt();
        X = reader.ReadDouble();
        Y = reader.ReadDouble();
        Z = reader.ReadDouble();
        Yaw = reader.ReadSignedByte();
        Pitch = reader.ReadSignedByte();
        HeadPitch = reader.ReadSignedByte();
        VelocityX = reader.ReadSignedShort();
        VelocityY = reader.ReadSignedShort();
        VelocityZ = reader.ReadSignedShort();
    }
}