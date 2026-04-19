using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TrackedWaypoint", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x83)]
public sealed partial class TrackedWaypointPacket : IServerPacket
{
    public int Operation { get; set; }
    public bool HasUUID { get; set; }
    public Guid? Uuid { get; set; }
    public string? Id { get; set; }
    public string IconStyle { get; set; } = default!;
    public int? IconColor { get; set; }
    public int Type { get; set; }
    public Vec3i? DataVec3i { get; set; }
    public int? ChunkX { get; set; }
    public int? ChunkZ { get; set; }
    public float? Azimuth { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Operation);
        writer.WriteBoolean(HasUUID);
        if (HasUUID)
            writer.WriteUUID(Uuid!.Value);
        else
            writer.WriteString(Id!);
        writer.WriteString(IconStyle);
        if (IconColor.HasValue)
            writer.WriteSignedInt(IconColor.Value);
        else
            writer.WriteSignedInt(0);
        writer.WriteVarInt(Type);
        switch (Type)
        {
            case 1: // vec3i
                writer.WriteType<DataVec3i>(DataVec3i!.Value, protocolVersion);
                break;
            case 2: // chunk
                writer.WriteVarInt(ChunkX!.Value);
                writer.WriteVarInt(ChunkZ!.Value);
                break;
            case 3: // azimuth
                writer.WriteFloat(Azimuth!.Value);
                break;
            default:
                break;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Operation = reader.ReadVarInt();
        HasUUID = reader.ReadBoolean();
        if (HasUUID)
            Uuid = reader.ReadUUID();
        else
            Id = reader.ReadString();
        IconStyle = reader.ReadString();
        IconColor = reader.ReadSignedInt();
        Type = reader.ReadVarInt();
        switch (Type)
        {
            case 1: // vec3i
                DataVec3i = reader.ReadType<Vec3i>(protocolVersion);
                break;
            case 2: // chunk
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
                break;
            case 3: // azimuth
                Azimuth = reader.ReadFloat();
                break;
            default:
                break;
        }
    }
}