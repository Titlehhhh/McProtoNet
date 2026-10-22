using Dunet;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Union]
public partial record TrackedWaypointData
{
    partial record Empty();
    partial record Position(Vec3i Coordinates);
    partial record Chunk(int ChunkX, int ChunkZ);
    partial record Azimuth(float Angle);
    public static TrackedWaypointData Read(ref MinecraftPrimitiveReader reader, int protocolVersion, int discriminator)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TrackedWaypointData>(protocolVersion);
        switch (discriminator)
        {
            case 0:
            {
                return new Empty();
            }

            case 1:
            {
                var coordinates = reader.ReadType<Vec3i>(protocolVersion);
                return new Position(coordinates);
            }

            case 2:
            {
                var chunkX = reader.ReadVarInt();
                var chunkZ = reader.ReadVarInt();
                return new Chunk(chunkX, chunkZ);
            }

            case 3:
            {
                var angle = reader.ReadFloat();
                return new Azimuth(angle);
            }
        }

        throw new System.NotSupportedException($"TrackedWaypointData has no case for discriminator {discriminator} at protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TrackedWaypointData>(protocolVersion);
        switch (this)
        {
            case Empty _:
            {
                return;
            }

            case Position arm:
            {
                Vec3i Coordinates = arm.Coordinates;
                writer.WriteType<Vec3i>(Coordinates, protocolVersion);
                return;
            }

            case Chunk arm:
            {
                int ChunkX = arm.ChunkX;
                int ChunkZ = arm.ChunkZ;
                writer.WriteVarInt(ChunkX);
                writer.WriteVarInt(ChunkZ);
                return;
            }

            case Azimuth arm:
            {
                float Angle = arm.Angle;
                writer.WriteFloat(Angle);
                return;
            }
        }

        throw new System.NotSupportedException($"TrackedWaypointData case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }

    public int Discriminator(int protocolVersion)
    {
        switch (this)
        {
            case Empty _:
                return 0;
            case Position _:
                return 1;
            case Chunk _:
                return 2;
            case Azimuth _:
                return 3;
        }

        throw new System.NotSupportedException($"TrackedWaypointData case {GetType().Name} has no wire layout for protocol version {protocolVersion}.");
    }
}
