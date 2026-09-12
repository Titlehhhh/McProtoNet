using System.Text.Json;

namespace McProtoNet.NBT;

/// <summary>
/// Writes a tag tree as JSON: a compound becomes an object keyed by the child names, a list and
/// the array tags become arrays, every other tag becomes its value.
/// </summary>
/// <remarks>
/// The name of the tag itself is not written; the caller decides what the value is called.
/// </remarks>
public static class NbtJson
{
    /// <summary>
    /// Writes the value of the tag to the specified JSON writer.
    /// </summary>
    /// <param name="tag">The tag to write.</param>
    /// <param name="writer">The writer to write to.</param>
    public static void WriteJson(this NbtTag tag, Utf8JsonWriter writer)
    {
        switch (tag)
        {
            case NbtCompound compound:
                writer.WriteStartObject();
                foreach (var child in compound)
                {
                    writer.WritePropertyName(child.Name ?? string.Empty);
                    child.WriteJson(writer);
                }

                writer.WriteEndObject();
                break;
            case NbtList list:
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    item.WriteJson(writer);
                }

                writer.WriteEndArray();
                break;
            case NbtByte b:
                writer.WriteNumberValue(b.Value);
                break;
            case NbtShort s:
                writer.WriteNumberValue(s.Value);
                break;
            case NbtInt i:
                writer.WriteNumberValue(i.Value);
                break;
            case NbtLong l:
                writer.WriteNumberValue(l.Value);
                break;
            case NbtFloat f:
                WriteFloating(writer, f.Value);
                break;
            case NbtDouble d:
                WriteFloating(writer, d.Value);
                break;
            case NbtString str:
                writer.WriteStringValue(str.Value);
                break;
            case NbtByteArray bytes:
                writer.WriteStartArray();
                foreach (var v in bytes.Value)
                {
                    writer.WriteNumberValue(v);
                }

                writer.WriteEndArray();
                break;
            case NbtIntArray ints:
                writer.WriteStartArray();
                foreach (var v in ints.Value)
                {
                    writer.WriteNumberValue(v);
                }

                writer.WriteEndArray();
                break;
            case NbtLongArray longs:
                writer.WriteStartArray();
                foreach (var v in longs.Value)
                {
                    writer.WriteNumberValue(v);
                }

                writer.WriteEndArray();
                break;
            default:
                writer.WriteNullValue();
                break;
        }
    }

    /// <summary>
    /// Writes a floating-point value; NaN and the infinities have no JSON number, so they go out as
    /// the strings <c>NaN</c>, <c>Infinity</c> and <c>-Infinity</c>.
    /// </summary>
    private static void WriteFloating(Utf8JsonWriter writer, double value)
    {
        if (double.IsFinite(value))
        {
            writer.WriteNumberValue(value);
            return;
        }

        writer.WriteStringValue(double.IsNaN(value) ? "NaN" : value > 0 ? "Infinity" : "-Infinity");
    }
}
