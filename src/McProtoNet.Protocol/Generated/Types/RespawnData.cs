using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
public sealed partial class RespawnData : IProtocolType<RespawnData>
{
    public GlobalPos GlobalPos { get; }
    public float Yaw { get; }
    public float Pitch { get; }

    public RespawnData(GlobalPos globalPos, float yaw, float pitch)
    {
        GlobalPos = globalPos;
        Yaw = yaw;
        Pitch = pitch;
    }

    public static RespawnData Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RespawnData>(protocolVersion);
        var globalPos = reader.ReadType<GlobalPos>(protocolVersion);
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        return new RespawnData(globalPos, yaw, pitch);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RespawnData>(protocolVersion);
        writer.WriteType<GlobalPos>(GlobalPos, protocolVersion);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("GlobalPos");
        GlobalPos.WriteJson(writer);
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
        writer.WriteEndObject();
    }
}
