using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemSoundEvent : IProtocolType<ItemSoundEvent>
{
    public string SoundName { get; }
    public float? FixedRange { get; }

    public ItemSoundEvent(string soundName, float? fixedRange)
    {
        SoundName = soundName;
        FixedRange = fixedRange;
    }

    public static ItemSoundEvent Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemSoundEvent>(protocolVersion);
        var soundName = reader.ReadString();
        float? fixedRange = null;
        if (reader.ReadBoolean())
            fixedRange = reader.ReadFloat();
        return new ItemSoundEvent(soundName, fixedRange);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ItemSoundEvent>(protocolVersion);
        writer.WriteString(SoundName);
        writer.WriteBoolean(FixedRange is not null);
        if (FixedRange is { } fixedRangeValue)
            writer.WriteFloat(fixedRangeValue);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("SoundName");
        writer.WriteStringValue(SoundName);
        if (FixedRange is { } fixedRangeValue)
        {
            writer.WritePropertyName("FixedRange");
            if (double.IsFinite(fixedRangeValue))
                writer.WriteNumberValue(fixedRangeValue);
            else
                writer.WriteStringValue(double.IsNaN(fixedRangeValue) ? "NaN" : fixedRangeValue > 0 ? "Infinity" : "-Infinity");
        }

        writer.WriteEndObject();
    }
}
