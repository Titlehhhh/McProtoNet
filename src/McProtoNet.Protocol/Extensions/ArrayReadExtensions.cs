using System;
using System.IO;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

/// <summary>
/// Provides extension methods that read length-prefixed arrays through
/// <see cref="MinecraftPrimitiveReader"/>. Every method funnels its element count through
/// <see cref="ReadLength"/>.
/// </summary>
public static class ArrayReadExtensions
{
    /// <summary>The largest number of elements a length-prefixed array may declare when the call site
    /// gives no tighter limit.</summary>
    public const int DefaultMaxArrayCount = 1_048_576;

    /// <summary>
    /// Reads the element count of a length-prefixed array.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="format">How the count is encoded on the wire.</param>
    /// <returns>The count that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative or the data runs out.</exception>
    public static int ReadLength(this ref MinecraftPrimitiveReader reader,
        LengthFormat format = LengthFormat.VarInt)
    {
        var count = format switch
        {
            LengthFormat.VarInt => reader.ReadVarInt(),
            LengthFormat.Int32 => reader.ReadSignedInt(),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        if (count < 0)
        {
            ThrowHelper.ThrowInvalidLength(count);
        }

        return count;
    }

    /// <summary>
    /// Reads a length-prefixed byte array from the reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <returns>The bytes that were read.</returns>
    /// <exception cref="InvalidDataException">Fewer bytes are left than the length says.</exception>
    /// <remarks>
    /// The length is read first as a VarInt.
    /// </remarks>
    public static byte[] ReadByteArray(this ref MinecraftPrimitiveReader reader)
        => reader.ReadBuffer(reader.ReadVarInt());

    /// <summary>
    /// Reads the specified number of bytes from the reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>The bytes that were read.</returns>
    /// <exception cref="InvalidDataException">Fewer than <paramref name="length"/> bytes are
    /// left.</exception>
    public static byte[] ReadFixedBytes(this ref MinecraftPrimitiveReader reader, int length)
        => reader.ReadBuffer(length);

    /// <summary>
    /// Reads all remaining bytes from the reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <returns>The bytes that remained in the reader.</returns>
    public static byte[] ReadRestBytes(this ref MinecraftPrimitiveReader reader)
        => reader.ReadRestBuffer();

    /// <summary>Reads a length-prefixed array of VarInts.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, does not fit the frame, or the
    /// data runs out.</exception>
    public static int[] ReadVarIntArray(this ref MinecraftPrimitiveReader reader,
        LengthFormat format = LengthFormat.VarInt)
    {
        var count = reader.ReadLength(format);
        if (count == 0) return [];

        EnsureFits(ref reader, count, 1);

        var result = new int[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadVarInt();
        }

        return result;
    }

    /// <summary>Reads a length-prefixed array of VarLongs.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, does not fit the frame, or the
    /// data runs out.</exception>
    public static long[] ReadVarLongArray(this ref MinecraftPrimitiveReader reader,
        LengthFormat format = LengthFormat.VarInt)
    {
        var count = reader.ReadLength(format);
        if (count == 0) return [];

        EnsureFits(ref reader, count, 1);

        var result = new long[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadVarLong();
        }

        return result;
    }

    /// <summary>
    /// Reads a length-prefixed array of fixed-width primitives. The type parameter selects the wire
    /// encoding: <see cref="bool"/>, <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>,
    /// <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>,
    /// <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/> or <see cref="Guid"/>.
    /// </summary>
    /// <typeparam name="T">The CLR type of one element.</typeparam>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, or the data runs out.</exception>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> has no fixed-width wire
    /// encoding.</exception>
    /// <remarks>
    /// VarInt and VarLong are deliberately not covered: their CLR type (<see cref="int"/> and
    /// <see cref="long"/>) collides with the fixed-width encodings of the same type, so they have
    /// their own methods.
    /// </remarks>
    public static T[] ReadPrimitiveArray<T>(this ref MinecraftPrimitiveReader reader,
        LengthFormat format = LengthFormat.VarInt)
        where T : unmanaged
    {
        var count = reader.ReadLength(format);
        if (count == 0) return [];

        if (typeof(T) == typeof(byte))
        {
            EnsureFits(ref reader, count, 1);
            var values = new byte[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadUnsignedByte();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(sbyte))
        {
            EnsureFits(ref reader, count, 1);
            var values = new sbyte[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadSignedByte();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(short))
        {
            EnsureFits(ref reader, count, 2);
            var values = new short[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadSignedShort();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(ushort))
        {
            EnsureFits(ref reader, count, 2);
            var values = new ushort[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadUnsignedShort();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(int))
        {
            EnsureFits(ref reader, count, 4);
            var values = new int[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadSignedInt();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(uint))
        {
            EnsureFits(ref reader, count, 4);
            var values = new uint[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadUnsignedInt();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(long))
        {
            EnsureFits(ref reader, count, 8);
            var values = new long[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadSignedLong();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(ulong))
        {
            EnsureFits(ref reader, count, 8);
            var values = new ulong[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadUnsignedLong();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(float))
        {
            EnsureFits(ref reader, count, 4);
            var values = new float[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadFloat();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(double))
        {
            EnsureFits(ref reader, count, 8);
            var values = new double[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadDouble();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(bool))
        {
            EnsureFits(ref reader, count, 1);
            var values = new bool[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadBoolean();
            return (T[])(object)values;
        }

        if (typeof(T) == typeof(Guid))
        {
            EnsureFits(ref reader, count, 16);
            var values = new Guid[count];
            for (var i = 0; i < values.Length; i++) values[i] = reader.ReadUUID();
            return (T[])(object)values;
        }

        ThrowHelper.ThrowUnsupportedElementType(typeof(T));
        return [];
    }

    /// <summary>Reads a length-prefixed array of length-prefixed byte arrays.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="format">How the outer element count is encoded on the wire.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, does not fit the frame, or the
    /// data runs out.</exception>
    public static byte[][] ReadByteArrayArray(this ref MinecraftPrimitiveReader reader,
        LengthFormat format = LengthFormat.VarInt)
    {
        var count = reader.ReadLength(format);
        if (count == 0) return [];

        EnsureFits(ref reader, count, 1);

        var result = new byte[count][];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadByteArray();
        }

        return result;
    }

    /// <summary>Reads a length-prefixed array of the specified protocol type.</summary>
    /// <typeparam name="T">The protocol type of one element.</typeparam>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, does not fit the frame, or the
    /// data runs out.</exception>
    public static T[] ReadTypeArray<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion,
        LengthFormat format = LengthFormat.VarInt)
        where T : IProtocolType<T>
    {
        var count = reader.ReadLength(format);
        if (count == 0) return [];

        EnsureFits(ref reader, count, 1);

        var result = new T[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = T.Read(ref reader, protocolVersion);
        }

        return result;
    }

    /// <summary>Reads a length-prefixed array of strings.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="maxLength">The maximum number of characters one string may contain.</param>
    /// <param name="format">How the element count is encoded on the wire.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, does not fit the frame, a string
    /// is longer than <paramref name="maxLength"/>, or the data runs out.</exception>
    public static string[] ReadStringArray(this ref MinecraftPrimitiveReader reader,
        int maxLength = short.MaxValue, LengthFormat format = LengthFormat.VarInt)
    {
        var count = reader.ReadLength(format);
        if (count == 0) return [];

        EnsureFits(ref reader, count, 1);

        var result = new string[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadString(maxLength);
        }

        return result;
    }

    /// <summary>
    /// Rejects a count whose elements cannot physically fit in what is left of the frame. Call it
    /// before the array is allocated: the check costs one multiply and one compare.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="count">The element count that was read.</param>
    /// <param name="bytesPerElement">The number of bytes one element occupies on the wire at
    /// minimum; for fixed-width primitives it is exact.</param>
    private static void EnsureFits(ref MinecraftPrimitiveReader reader, int count, int bytesPerElement)
    {
        if ((long)count * bytesPerElement > reader.RemainingCount)
        {
            ThrowHelper.ThrowArrayExceedsFrame(count, bytesPerElement, reader.RemainingCount);
        }
    }
}
