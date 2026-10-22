#pragma warning disable CA2225

using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public readonly partial record struct HeightmapType(int Value) : IProtocolType<HeightmapType>
{
    public static readonly HeightmapType WorldSurfaceWg = new(0);
    public static readonly HeightmapType WorldSurface = new(1);
    public static readonly HeightmapType OceanFloorWg = new(2);
    public static readonly HeightmapType OceanFloor = new(3);
    public static readonly HeightmapType MotionBlocking = new(4);
    public static readonly HeightmapType MotionBlockingNoLeaves = new(5);
    public static explicit operator int (HeightmapType value) => value.Value;
    public static explicit operator HeightmapType(int value) => new(value);
    public static HeightmapType Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeightmapType>(protocolVersion);
        return new HeightmapType((int)reader.ReadVarInt());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeightmapType>(protocolVersion);
        writer.WriteVarInt((int)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "world_surface_wg",
        1 => "world_surface",
        2 => "ocean_floor_wg",
        3 => "ocean_floor",
        4 => "motion_blocking",
        5 => "motion_blocking_no_leaves",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 771)
        {
            return Value switch
            {
                0 => "world_surface_wg",
                1 => "world_surface",
                2 => "ocean_floor_wg",
                3 => "ocean_floor",
                4 => "motion_blocking",
                5 => "motion_blocking_no_leaves",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
