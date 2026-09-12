using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
public sealed partial class GlobalPos : IProtocolType<GlobalPos>
{
    public string DimensionName { get; }
    public Position Location { get; }

    public GlobalPos(string dimensionName, Position location)
    {
        DimensionName = dimensionName;
        Location = location;
    }

    public static GlobalPos Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GlobalPos>(protocolVersion);
        var dimensionName = reader.ReadString();
        var location = reader.ReadType<Position>(protocolVersion);
        return new GlobalPos(dimensionName, location);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GlobalPos>(protocolVersion);
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
