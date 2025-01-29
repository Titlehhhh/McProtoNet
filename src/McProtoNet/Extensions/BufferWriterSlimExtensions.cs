using System.Runtime.CompilerServices;
using Cysharp.Text;
using DotNext.Buffers;

namespace McProtoNet;

/// <summary>
/// Extension methods for BufferWriterSlim to write Minecraft protocol data types
/// </summary>
public static class BufferWriterSlimExtensions
{
    /// <summary>
    /// Writes a VarInt to the buffer
    /// </summary>
    /// <param name="writer">The buffer writer to write to</param>
    /// <param name="value">The integer value to write as a VarInt</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteVarInt(this ref BufferWriterSlim<byte> writer, int value)
    {
        if (value == 0)
        {
            writer.GetSpan(1)[0] = 0;
            writer.Advance(1);
            return;
        }


        scoped var data = writer.GetSpan();

        if (data.Length >= 5)
        {
            var len = value.GetVarIntLength(data);
            writer.Advance(len);
        }
        else
        {
            data = stackalloc byte[5];
            var len = value.GetVarIntLength(data);
            writer.Write(data);
        }
    }

    /// <summary>
    /// Writes a string to the buffer in UTF-8 format, prefixed with its length as a VarInt
    /// </summary>
    /// <param name="writer">The buffer writer to write to</param>
    /// <param name="value">The string to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteString(this ref BufferWriterSlim<byte> writer, string value)
    {
        var builder = ZString.CreateUtf8StringBuilder();
        try
        {
            builder.Append(value);
            writer.WriteVarInt(builder.Length);
            writer.Write(builder.AsSpan());
        }
        finally
        {
            builder.Dispose();
        }
    }

    /// <summary>
    /// Writes a byte buffer to the writer, prefixed with its length as a VarInt
    /// </summary>
    /// <param name="writer">The buffer writer to write to</param>
    /// <param name="value">The byte span to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBuffer(this ref BufferWriterSlim<byte> writer, ReadOnlySpan<byte> value)
    {
        writer.WriteVarInt(value.Length);
        writer.Write(value);
    }
}