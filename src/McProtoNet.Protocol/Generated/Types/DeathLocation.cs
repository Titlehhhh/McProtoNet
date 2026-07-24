using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
public sealed partial class DeathLocation : IProtocolType<DeathLocation>
{
    public string DimensionName { get; }
    public Position Location { get; }

    public DeathLocation(string dimensionName, Position location)
    {
        DimensionName = dimensionName;
        Location = location;
    }

    public static DeathLocation Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DeathLocation>(protocolVersion);
        var dimensionName = reader.ReadString();
        var location = reader.ReadType<Position>(protocolVersion);
        return new DeathLocation(dimensionName, location);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DeathLocation>(protocolVersion);
        writer.WriteString(DimensionName);
        writer.WriteType<Position>(Location, protocolVersion);
    }
}
