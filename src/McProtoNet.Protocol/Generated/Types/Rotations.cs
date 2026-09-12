using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Rotations(float Pitch, float Yaw, float Roll) : IProtocolType<Rotations>
{
    public static Rotations Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Rotations>(protocolVersion);
        var pitch = reader.ReadFloat();
        var yaw = reader.ReadFloat();
        var roll = reader.ReadFloat();
        return new Rotations(pitch, yaw, roll);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Rotations>(protocolVersion);
        writer.WriteFloat(Pitch);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Roll);
    }

    public readonly void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Pitch");
        if (double.IsFinite(Pitch))
            writer.WriteNumberValue(Pitch);
        else
            writer.WriteStringValue(double.IsNaN(Pitch) ? "NaN" : Pitch > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Yaw");
        if (double.IsFinite(Yaw))
            writer.WriteNumberValue(Yaw);
        else
            writer.WriteStringValue(double.IsNaN(Yaw) ? "NaN" : Yaw > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Roll");
        if (double.IsFinite(Roll))
            writer.WriteNumberValue(Roll);
        else
            writer.WriteStringValue(double.IsNaN(Roll) ? "NaN" : Roll > 0 ? "Infinity" : "-Infinity");
        writer.WriteEndObject();
    }
}
