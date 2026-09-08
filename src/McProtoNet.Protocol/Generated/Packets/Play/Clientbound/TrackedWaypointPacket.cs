using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.tracked_waypoint", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Operation", "TrackedWaypointOperation")]
[PacketField("Waypoint", "Waypoint")]
public sealed partial record TrackedWaypointPacket(TrackedWaypointOperation Operation, Waypoint Waypoint) : IPacket<TrackedWaypointPacket>, IPacket
{
    public static TrackedWaypointPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TrackedWaypointPacket>(protocolVersion);
        var operation = reader.ReadType<TrackedWaypointOperation>(protocolVersion);
        var waypoint = reader.ReadType<Waypoint>(protocolVersion);
        return new TrackedWaypointPacket(operation, waypoint);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TrackedWaypointPacket>(protocolVersion);
        writer.WriteType<TrackedWaypointOperation>(Operation, protocolVersion);
        writer.WriteType<Waypoint>(Waypoint, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.tracked_waypoint", "TrackedWaypoint", PacketPhase.Play, PacketDirection.Clientbound, 116);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x83;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x88;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x8A;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
