#pragma warning disable CA2225

using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public readonly partial record struct TrackedWaypointOperation(int Value) : IProtocolType<TrackedWaypointOperation>
{
    public static readonly TrackedWaypointOperation Track = new(0);
    public static readonly TrackedWaypointOperation Untrack = new(1);
    public static readonly TrackedWaypointOperation Update = new(2);
    public static explicit operator int (TrackedWaypointOperation value) => value.Value;
    public static explicit operator TrackedWaypointOperation(int value) => new(value);
    public static TrackedWaypointOperation Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TrackedWaypointOperation>(protocolVersion);
        return new TrackedWaypointOperation((int)reader.ReadVarInt());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TrackedWaypointOperation>(protocolVersion);
        writer.WriteVarInt((int)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "track",
        1 => "untrack",
        2 => "update",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 771)
        {
            return Value switch
            {
                0 => "track",
                1 => "untrack",
                2 => "update",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
