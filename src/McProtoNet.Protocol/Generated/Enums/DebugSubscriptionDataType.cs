#pragma warning disable CA2225

using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
public readonly partial record struct DebugSubscriptionDataType(int Value) : IProtocolType<DebugSubscriptionDataType>
{
    public static readonly DebugSubscriptionDataType DedicatedServerTickTime = new(0);
    public static readonly DebugSubscriptionDataType Bees = new(1);
    public static readonly DebugSubscriptionDataType Brains = new(2);
    public static readonly DebugSubscriptionDataType Breezes = new(3);
    public static readonly DebugSubscriptionDataType GoalSelectors = new(4);
    public static readonly DebugSubscriptionDataType EntityPaths = new(5);
    public static readonly DebugSubscriptionDataType EntityBlockIntersections = new(6);
    public static readonly DebugSubscriptionDataType BeeHives = new(7);
    public static readonly DebugSubscriptionDataType Pois = new(8);
    public static readonly DebugSubscriptionDataType RedstoneWireOrientations = new(9);
    public static readonly DebugSubscriptionDataType VillageSections = new(10);
    public static readonly DebugSubscriptionDataType Raids = new(11);
    public static readonly DebugSubscriptionDataType Structures = new(12);
    public static readonly DebugSubscriptionDataType GameEventListeners = new(13);
    public static readonly DebugSubscriptionDataType NeighborUpdates = new(14);
    public static readonly DebugSubscriptionDataType GameEvents = new(15);
    public static explicit operator int (DebugSubscriptionDataType value) => value.Value;
    public static explicit operator DebugSubscriptionDataType(int value) => new(value);
    public static DebugSubscriptionDataType Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSubscriptionDataType>(protocolVersion);
        return new DebugSubscriptionDataType((int)reader.ReadVarInt());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSubscriptionDataType>(protocolVersion);
        writer.WriteVarInt((int)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "DedicatedServerTickTime",
        1 => "Bees",
        2 => "Brains",
        3 => "Breezes",
        4 => "GoalSelectors",
        5 => "EntityPaths",
        6 => "EntityBlockIntersections",
        7 => "BeeHives",
        8 => "Pois",
        9 => "RedstoneWireOrientations",
        10 => "VillageSections",
        11 => "Raids",
        12 => "Structures",
        13 => "GameEventListeners",
        14 => "NeighborUpdates",
        15 => "GameEvents",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 773)
        {
            return Value switch
            {
                0 => "DedicatedServerTickTime",
                1 => "Bees",
                2 => "Brains",
                3 => "Breezes",
                4 => "GoalSelectors",
                5 => "EntityPaths",
                6 => "EntityBlockIntersections",
                7 => "BeeHives",
                8 => "Pois",
                9 => "RedstoneWireOrientations",
                10 => "VillageSections",
                11 => "Raids",
                12 => "Structures",
                13 => "GameEventListeners",
                14 => "NeighborUpdates",
                15 => "GameEvents",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
