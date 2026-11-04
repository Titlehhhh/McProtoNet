using System;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

/// <summary>
/// Provides extension methods that write length-prefixed arrays through
/// <see cref="MinecraftPrimitiveWriter"/>.
/// </summary>
public static class ArrayWriteExtensions
{
    /// <summary>
    /// Writes the element count of a length-prefixed array.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="length">The element count.</param>
    /// <param name="format">How the count is encoded on the wire.</param>
    public static void WriteLength(this MinecraftPrimitiveWriter writer, int length,
        LengthFormat format = LengthFormat.VarInt)
    {
        switch (format)
        {
            case LengthFormat.VarInt:
                writer.WriteVarInt(length);
                break;
            case LengthFormat.Int32:
                writer.WriteSignedInt(length);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    /// <summary>
    /// Writes a byte array to the writer, preceded by its length.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The bytes to write.</param>
    /// <remarks>
    /// The length is written first as a VarInt.
    /// </remarks>
    public static void WriteByteArray(this MinecraftPrimitiveWriter writer, byte[] value)
    {
        writer.WriteVarInt(value.Length);
        writer.WriteBuffer(value);
    }

    /// <summary>
    /// Writes a byte array of the expected length to the writer, without a length prefix.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The bytes to write.</param>
    /// <param name="length">The number of bytes that <paramref name="value"/> must contain.</param>
    /// <exception cref="ArgumentException">The length of <paramref name="value"/> is not equal to
    /// <paramref name="length"/>.</exception>
    public static void WriteFixedBytes(this MinecraftPrimitiveWriter writer, byte[] value, int length)
    {
        if (value.Length != length)
        {
            throw new ArgumentException(
                $"Expected exactly {length} bytes, got {value.Length}.", nameof(value));
        }

        writer.WriteBuffer(value);
    }

    /// <summary>
    /// Writes a byte array to the writer, without a length prefix.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The bytes to write.</param>
    public static void WriteRestBytes(this MinecraftPrimitiveWriter writer, byte[] value)
        => writer.WriteBuffer(value);

    /// <summary>Writes an array of length-prefixed byte arrays, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The arrays to write.</param>
    /// <param name="format">How the outer element count is encoded on the wire.</param>
    public static void WriteByteArrayArray(this MinecraftPrimitiveWriter writer, byte[][] value,
        LengthFormat format = LengthFormat.VarInt)
    {
        writer.WriteLength(value.Length, format);
        foreach (var item in value)
        {
            writer.WriteByteArray(item);
        }
    }

    /// <summary>Writes an array of VarInts, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The values to write.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    public static void WriteVarIntArray(this MinecraftPrimitiveWriter writer, int[] value,
        LengthFormat format = LengthFormat.VarInt)
    {
        writer.WriteLength(value.Length, format);
        foreach (var item in value)
        {
            writer.WriteVarInt(item);
        }
    }

    /// <summary>Writes an array of VarLongs, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The values to write.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    public static void WriteVarLongArray(this MinecraftPrimitiveWriter writer, long[] value,
        LengthFormat format = LengthFormat.VarInt)
    {
        writer.WriteLength(value.Length, format);
        foreach (var item in value)
        {
            writer.WriteVarLong(item);
        }
    }

    /// <summary>
    /// Writes an array of fixed-width primitives, preceded by its length. The type parameter selects
    /// the wire encoding; VarInt and VarLong have their own methods.
    /// </summary>
    /// <typeparam name="T">The CLR type of one element.</typeparam>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The values to write.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> has no fixed-width wire
    /// encoding.</exception>
    public static void WritePrimitiveArray<T>(this MinecraftPrimitiveWriter writer, T[] value,
        LengthFormat format = LengthFormat.VarInt)
        where T : unmanaged
    {
        writer.WriteLength(value.Length, format);

        if (typeof(T) == typeof(byte))
        {
            foreach (var item in (byte[])(object)value) writer.WriteUnsignedByte(item);
            return;
        }

        if (typeof(T) == typeof(sbyte))
        {
            foreach (var item in (sbyte[])(object)value) writer.WriteSignedByte(item);
            return;
        }

        if (typeof(T) == typeof(short))
        {
            foreach (var item in (short[])(object)value) writer.WriteSignedShort(item);
            return;
        }

        if (typeof(T) == typeof(ushort))
        {
            foreach (var item in (ushort[])(object)value) writer.WriteUnsignedShort(item);
            return;
        }

        if (typeof(T) == typeof(int))
        {
            foreach (var item in (int[])(object)value) writer.WriteSignedInt(item);
            return;
        }

        if (typeof(T) == typeof(uint))
        {
            foreach (var item in (uint[])(object)value) writer.WriteUnsignedInt(item);
            return;
        }

        if (typeof(T) == typeof(long))
        {
            foreach (var item in (long[])(object)value) writer.WriteSignedLong(item);
            return;
        }

        if (typeof(T) == typeof(ulong))
        {
            foreach (var item in (ulong[])(object)value) writer.WriteUnsignedLong(item);
            return;
        }

        if (typeof(T) == typeof(float))
        {
            foreach (var item in (float[])(object)value) writer.WriteFloat(item);
            return;
        }

        if (typeof(T) == typeof(double))
        {
            foreach (var item in (double[])(object)value) writer.WriteDouble(item);
            return;
        }

        if (typeof(T) == typeof(bool))
        {
            foreach (var item in (bool[])(object)value) writer.WriteBoolean(item);
            return;
        }

        if (typeof(T) == typeof(Guid))
        {
            foreach (var item in (Guid[])(object)value) writer.WriteUUID(item);
            return;
        }

        ThrowHelper.ThrowUnsupportedElementType(typeof(T));
    }

    /// <summary>Writes an array of the specified protocol type, preceded by its length.</summary>
    /// <typeparam name="T">The protocol type of one element.</typeparam>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The elements to write.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    public static void WriteTypeArray<T>(this MinecraftPrimitiveWriter writer, T[] value,
        int protocolVersion, LengthFormat format = LengthFormat.VarInt)
        where T : IProtocolType<T>
    {
        writer.WriteLength(value.Length, format);
        foreach (var item in value)
        {
            item.Write(writer, protocolVersion);
        }
    }

    /// <summary>Writes an array of strings, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The strings to write.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    public static void WriteStringArray(this MinecraftPrimitiveWriter writer, string[] value,
        LengthFormat format = LengthFormat.VarInt)
    {
        writer.WriteLength(value.Length, format);
        foreach (var item in value)
        {
            writer.WriteString(item);
        }
    }
}
