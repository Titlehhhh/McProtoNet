using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TrackedWaypoint", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TrackedWaypointPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(771, MinecraftVersion.LatestProtocol),
    };

    public int Operation { get; set; }
    public WaypointData Waypoint { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Operation);
                writer.WriteBoolean(Waypoint.HasUuid);
                if (Waypoint.HasUuid)
                {
                    writer.WriteUUID(Waypoint.Uuid ?? throw new InvalidOperationException("TrackedWaypoint uuid missing."));
                }
                else
                {
                    writer.WriteString(Waypoint.Id ?? throw new InvalidOperationException("TrackedWaypoint id missing."));
                }
                writer.WriteString(Waypoint.Icon.Style);
                if (Waypoint.Icon.Color.HasValue)
                {
                    writer.WriteBoolean(true);
                    writer.WriteSignedInt(Waypoint.Icon.Color.Value);
                }
                else
                {
                    writer.WriteBoolean(false);
                }
                writer.WriteVarInt(Waypoint.Type);
                switch (Waypoint.Type)
                {
                    case 1:
                        writer.WriteVec3i(Waypoint.Position ?? throw new InvalidOperationException("TrackedWaypoint position missing."), protocolVersion);
                        break;
                    case 2:
                        var chunk = Waypoint.Chunk ?? throw new InvalidOperationException("TrackedWaypoint chunk missing.");
                        writer.WriteVarInt(chunk.ChunkX);
                        writer.WriteVarInt(chunk.ChunkZ);
                        break;
                    case 3:
                        writer.WriteFloat(Waypoint.Azimuth ?? throw new InvalidOperationException("TrackedWaypoint azimuth missing."));
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TrackedWaypoint), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
            {
                Operation = reader.ReadVarInt();
                var waypoint = new WaypointData
                {
                    HasUuid = reader.ReadBoolean()
                };
                if (waypoint.HasUuid)
                {
                    waypoint.Uuid = reader.ReadUUID();
                }
                else
                {
                    waypoint.Id = reader.ReadString();
                }

                waypoint.Icon = new WaypointIcon
                {
                    Style = reader.ReadString(),
                    Color = reader.ReadOptional(ReadDelegates.Int32)
                };

                waypoint.Type = reader.ReadVarInt();
                switch (waypoint.Type)
                {
                    case 1:
                        waypoint.Position = reader.ReadVec3i(protocolVersion);
                        break;
                    case 2:
                        waypoint.Chunk = new WaypointChunk
                        {
                            ChunkX = reader.ReadVarInt(),
                            ChunkZ = reader.ReadVarInt()
                        };
                        break;
                    case 3:
                        waypoint.Azimuth = reader.ReadFloat();
                        break;
                }

                Waypoint = waypoint;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TrackedWaypoint), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct WaypointData
    {
        public bool HasUuid { get; set; }
        public Guid? Uuid { get; set; }
        public string? Id { get; set; }
        public WaypointIcon Icon { get; set; }
        public int Type { get; set; }
        public Vec3i? Position { get; set; }
        public WaypointChunk? Chunk { get; set; }
        public float? Azimuth { get; set; }
    }

    public struct WaypointIcon
    {
        public string Style { get; set; }
        public int? Color { get; set; }
    }

    public struct WaypointChunk
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
    }
}
