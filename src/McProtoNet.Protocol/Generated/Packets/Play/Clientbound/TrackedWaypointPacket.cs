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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
