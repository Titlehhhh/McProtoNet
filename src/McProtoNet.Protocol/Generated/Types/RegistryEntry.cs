using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class RegistryEntry : IProtocolType<RegistryEntry>
{
    public string Key { get; }
    public NbtTag? Value { get; }

    public RegistryEntry(string key, NbtTag? value)
    {
        Key = key;
        Value = value;
    }

    public static RegistryEntry Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RegistryEntry>(protocolVersion);
        var key = reader.ReadString();
        NbtTag? value = null;
        if (reader.ReadBoolean())
            value = reader.ReadNbtTag(false)!;
        return new RegistryEntry(key, value);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RegistryEntry>(protocolVersion);
        writer.WriteString(Key);
        writer.WriteBoolean(Value is not null);
        if (Value is { } valueValue)
            writer.WriteNbt(valueValue);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Key");
        writer.WriteStringValue(Key);
        if (Value is { } valueValue)
        {
            writer.WritePropertyName("Value");
            valueValue.WriteJson(writer);
        }

        writer.WriteEndObject();
    }
}
