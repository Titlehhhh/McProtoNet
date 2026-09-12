using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class MinecartStep : IProtocolType<MinecartStep>
{
    public Vec3f Position { get; }
    public Vec3f Movement { get; }
    public float Yaw { get; }
    public float Pitch { get; }
    public float Weight { get; }

    public MinecartStep(Vec3f position, Vec3f movement, float yaw, float pitch, float weight)
    {
        Position = position;
        Movement = movement;
        Yaw = yaw;
        Pitch = pitch;
        Weight = weight;
    }

    public static MinecartStep Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MinecartStep>(protocolVersion);
        var position = reader.ReadType<Vec3f>(protocolVersion);
        var movement = reader.ReadType<Vec3f>(protocolVersion);
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        var weight = reader.ReadFloat();
        return new MinecartStep(position, movement, yaw, pitch, weight);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MinecartStep>(protocolVersion);
        writer.WriteType<Vec3f>(Position, protocolVersion);
        writer.WriteType<Vec3f>(Movement, protocolVersion);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteFloat(Weight);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Position");
        Position.WriteJson(writer);
        writer.WritePropertyName("Movement");
        Movement.WriteJson(writer);
        writer.WritePropertyName("Yaw");
        if (double.IsFinite(Yaw))
            writer.WriteNumberValue(Yaw);
        else
            writer.WriteStringValue(double.IsNaN(Yaw) ? "NaN" : Yaw > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Pitch");
        if (double.IsFinite(Pitch))
            writer.WriteNumberValue(Pitch);
        else
            writer.WriteStringValue(double.IsNaN(Pitch) ? "NaN" : Pitch > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Weight");
        if (double.IsFinite(Weight))
            writer.WriteNumberValue(Weight);
        else
            writer.WriteStringValue(double.IsNaN(Weight) ? "NaN" : Weight > 0 ? "Infinity" : "-Infinity");
        writer.WriteEndObject();
    }
}
