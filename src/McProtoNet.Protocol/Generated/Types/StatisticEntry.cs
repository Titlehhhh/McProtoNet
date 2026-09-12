using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct StatisticEntry(int CategoryId, int StatisticId, int Value) : IProtocolType<StatisticEntry>
{
    public static StatisticEntry Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StatisticEntry>(protocolVersion);
        var categoryId = reader.ReadVarInt();
        var statisticId = reader.ReadVarInt();
        var value = reader.ReadVarInt();
        return new StatisticEntry(categoryId, statisticId, value);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StatisticEntry>(protocolVersion);
        writer.WriteVarInt(CategoryId);
        writer.WriteVarInt(StatisticId);
        writer.WriteVarInt(Value);
    }

    public readonly void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("CategoryId");
        writer.WriteNumberValue(CategoryId);
        writer.WritePropertyName("StatisticId");
        writer.WriteNumberValue(StatisticId);
        writer.WritePropertyName("Value");
        writer.WriteNumberValue(Value);
        writer.WriteEndObject();
    }
}
