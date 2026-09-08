using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class Waypoint : IProtocolType<Waypoint>
{
    public WaypointIdentity Identity { get; }
    public WaypointIcon Icon { get; }
    public TrackedWaypointData Data { get; }

    public Waypoint(WaypointIdentity identity, WaypointIcon icon, TrackedWaypointData data)
    {
        Identity = identity;
        Icon = icon;
        Data = data;
    }

    public static Waypoint Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Waypoint>(protocolVersion);
        var _hasUUID = reader.ReadUnsignedByte();
        var identity = WaypointIdentity.Read(ref reader, protocolVersion, (int)_hasUUID);
        var icon = reader.ReadType<WaypointIcon>(protocolVersion);
        var _type = reader.ReadVarInt();
        var data = TrackedWaypointData.Read(ref reader, protocolVersion, (int)_type);
        return new Waypoint(identity, icon, data);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Waypoint>(protocolVersion);
        writer.WriteUnsignedByte(checked((byte)Identity.Discriminator(protocolVersion)));
        Identity.Write(writer, protocolVersion);
        writer.WriteType<WaypointIcon>(Icon, protocolVersion);
        writer.WriteVarInt(Data.Discriminator(protocolVersion));
        Data.Write(writer, protocolVersion);
    }
}
