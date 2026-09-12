using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("DimensionName");
        writer.WriteStringValue(DimensionName);
        writer.WritePropertyName("Location");
        Location.WriteJson(writer);
        writer.WriteEndObject();
    }
}
