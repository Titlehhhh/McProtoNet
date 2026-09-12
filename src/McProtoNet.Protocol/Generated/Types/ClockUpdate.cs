using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
public readonly partial record struct ClockUpdate(int Id, long TotalTicks, float PartialTick, float Rate) : IProtocolType<ClockUpdate>
{
    public static ClockUpdate Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClockUpdate>(protocolVersion);
        var id = reader.ReadVarInt();
        var totalTicks = reader.ReadVarLong();
        var partialTick = reader.ReadFloat();
        var rate = reader.ReadFloat();
        return new ClockUpdate(id, totalTicks, partialTick, rate);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClockUpdate>(protocolVersion);
        writer.WriteVarInt(Id);
        writer.WriteVarLong(TotalTicks);
        writer.WriteFloat(PartialTick);
        writer.WriteFloat(Rate);
    }

    public readonly void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Id");
        writer.WriteNumberValue(Id);
        writer.WritePropertyName("TotalTicks");
        writer.WriteNumberValue(TotalTicks);
        writer.WritePropertyName("PartialTick");
        if (double.IsFinite(PartialTick))
            writer.WriteNumberValue(PartialTick);
        else
            writer.WriteStringValue(double.IsNaN(PartialTick) ? "NaN" : PartialTick > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Rate");
        if (double.IsFinite(Rate))
            writer.WriteNumberValue(Rate);
        else
            writer.WriteStringValue(double.IsNaN(Rate) ? "NaN" : Rate > 0 ? "Infinity" : "-Infinity");
        writer.WriteEndObject();
    }
}
