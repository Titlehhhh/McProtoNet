using System;
using System.IO;
using System.Text.Json;
using McProtoNet.NBT;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Defines the read and write operations of a type that appears in the Minecraft protocol.
/// </summary>
/// <typeparam name="TSelf">The type that implements the interface.</typeparam>
public interface IProtocolType<TSelf> where TSelf : IProtocolType<TSelf>
{
    /// <summary>
    /// Reads a value of type <typeparamref name="TSelf"/> from the specified reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The value that was read.</returns>
    static abstract TSelf Read(ref MinecraftPrimitiveReader reader, int protocolVersion);

    /// <summary>
    /// Writes the current value to the specified writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    void Write(MinecraftPrimitiveWriter writer, int protocolVersion);

    /// <summary>
    /// Writes the current value as JSON: the decoded model, not the wire layout.
    /// </summary>
    /// <param name="writer">The JSON writer to write to.</param>
    void WriteJson(Utf8JsonWriter writer);
}

/// <summary>
/// Provides extension methods that read and write protocol types and byte arrays through
/// <see cref="MinecraftPrimitiveReader"/> and <see cref="MinecraftPrimitiveWriter"/>.
/// </summary>
public static class ProtocolTypeExtensions
{
    /// <summary>
    /// Reads a value of the specified protocol type from the reader.
    /// </summary>
    /// <typeparam name="T">The protocol type to read.</typeparam>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The value that was read.</returns>
    public static T ReadType<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion)
        where T : IProtocolType<T>
        => T.Read(ref reader, protocolVersion);

    /// <summary>
    /// Writes a value of the specified protocol type to the writer.
    /// </summary>
    /// <typeparam name="T">The protocol type to write.</typeparam>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    public static void WriteType<T>(this MinecraftPrimitiveWriter writer, T value, int protocolVersion)
        where T : IProtocolType<T>
        => value.Write(writer, protocolVersion);

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
    /// Reads all remaining bytes from the reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <returns>The bytes that remained in the reader.</returns>
    public static byte[] ReadRestBytes(this ref MinecraftPrimitiveReader reader)
        => reader.ReadRestBuffer();

    /// <summary>
    /// Writes a byte array to the writer, without a length prefix.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The bytes to write.</param>
    public static void WriteRestBytes(this MinecraftPrimitiveWriter writer, byte[] value)
        => writer.WriteBuffer(value);

    /// <summary>The largest number of elements a length-prefixed array may declare when the call site
    /// gives no tighter limit.</summary>
    public const int DefaultMaxArrayCount = 1 << 20;

    /// <summary>
    /// Reads the element count of a length-prefixed array and checks that the elements can fit in what
    /// is left of the frame.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="minBytesPerElement">The smallest number of bytes one element occupies on the wire.</param>
    /// <param name="maxCount">The largest count to accept.</param>
    /// <returns>The count that was read.</returns>
    /// <exception cref="InvalidDataException">The count is negative, is larger than
    /// <paramref name="maxCount"/>, or declares more elements than the remaining bytes can hold.</exception>
    /// <remarks>
    /// The count is read as a VarInt. Both bounds are needed: the frame bound alone still lets a full
    /// frame declare millions of references, and <paramref name="maxCount"/> alone does not know how
    /// many bytes are actually left.
    /// </remarks>
    public static int ReadCount(this ref MinecraftPrimitiveReader reader, int minBytesPerElement = 1,
        int maxCount = DefaultMaxArrayCount)
    {
        var count = reader.ReadVarInt();
        if ((uint)count > (uint)maxCount || (long)count * minBytesPerElement > reader.RemainingCount)
        {
            ThrowHelper.ThrowInvalidArrayLength(count, maxCount, reader.RemainingCount);
        }

        return count;
    }

    /// <summary>Reads a length-prefixed array of the specified protocol type.</summary>
    /// <typeparam name="T">The protocol type of one element.</typeparam>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <param name="minBytesPerElement">The smallest number of bytes one element occupies on the wire.</param>
    /// <param name="maxCount">The largest element count to accept.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is out of range, or the data runs out.</exception>
    public static T[] ReadTypeArray<T>(this ref MinecraftPrimitiveReader reader, int protocolVersion,
        int minBytesPerElement = 1, int maxCount = DefaultMaxArrayCount)
        where T : IProtocolType<T>
    {
        var count = reader.ReadCount(minBytesPerElement, maxCount);
        if (count == 0) return [];

        var result = new T[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = T.Read(ref reader, protocolVersion);
        }

        return result;
    }

    /// <summary>Writes an array of the specified protocol type, preceded by its length.</summary>
    /// <typeparam name="T">The protocol type of one element.</typeparam>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The elements to write.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    public static void WriteTypeArray<T>(this MinecraftPrimitiveWriter writer, T[] value, int protocolVersion)
        where T : IProtocolType<T>
    {
        writer.WriteVarInt(value.Length);
        foreach (var item in value)
        {
            item.Write(writer, protocolVersion);
        }
    }

    /// <summary>Reads a length-prefixed array of strings.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="maxLength">The maximum number of characters one string may contain.</param>
    /// <param name="maxCount">The largest element count to accept.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is out of range, a string is longer than
    /// <paramref name="maxLength"/>, or the data runs out.</exception>
    public static string[] ReadStringArray(this ref MinecraftPrimitiveReader reader,
        int maxLength = short.MaxValue, int maxCount = DefaultMaxArrayCount)
    {
        var count = reader.ReadCount(1, maxCount);
        if (count == 0) return [];

        var result = new string[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadString(maxLength);
        }

        return result;
    }

    /// <summary>Writes an array of strings, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The strings to write.</param>
    public static void WriteStringArray(this MinecraftPrimitiveWriter writer, string[] value)
    {
        writer.WriteVarInt(value.Length);
        foreach (var item in value)
        {
            writer.WriteString(item);
        }
    }

    /// <summary>Reads a length-prefixed array of VarInts.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="maxCount">The largest element count to accept.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is out of range, or the data runs out.</exception>
    public static int[] ReadVarIntArray(this ref MinecraftPrimitiveReader reader,
        int maxCount = DefaultMaxArrayCount)
    {
        var count = reader.ReadCount(1, maxCount);
        if (count == 0) return [];

        var result = new int[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadVarInt();
        }

        return result;
    }

    /// <summary>Writes an array of VarInts, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The values to write.</param>
    public static void WriteVarIntArray(this MinecraftPrimitiveWriter writer, int[] value)
    {
        writer.WriteVarInt(value.Length);
        foreach (var item in value)
        {
            writer.WriteVarInt(item);
        }
    }

    /// <summary>Reads a length-prefixed array of signed 64-bit integers.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="maxCount">The largest element count to accept.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is out of range, or the data runs out.</exception>
    public static long[] ReadSignedLongArray(this ref MinecraftPrimitiveReader reader,
        int maxCount = DefaultMaxArrayCount)
    {
        var count = reader.ReadCount(sizeof(long), maxCount);
        if (count == 0) return [];

        var result = new long[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadSignedLong();
        }

        return result;
    }

    /// <summary>Writes an array of signed 64-bit integers, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The values to write.</param>
    public static void WriteSignedLongArray(this MinecraftPrimitiveWriter writer, long[] value)
    {
        writer.WriteVarInt(value.Length);
        foreach (var item in value)
        {
            writer.WriteSignedLong(item);
        }
    }

    /// <summary>Reads a length-prefixed array of UUIDs.</summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="maxCount">The largest element count to accept.</param>
    /// <returns>The array that was read.</returns>
    /// <exception cref="InvalidDataException">The count is out of range, or the data runs out.</exception>
    public static Guid[] ReadUuidArray(this ref MinecraftPrimitiveReader reader,
        int maxCount = DefaultMaxArrayCount)
    {
        var count = reader.ReadCount(16, maxCount);
        if (count == 0) return [];

        var result = new Guid[count];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = reader.ReadUUID();
        }

        return result;
    }

    /// <summary>Writes an array of UUIDs, preceded by its length.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The values to write.</param>
    public static void WriteUuidArray(this MinecraftPrimitiveWriter writer, Guid[] value)
    {
        writer.WriteVarInt(value.Length);
        foreach (var item in value)
        {
            writer.WriteUUID(item);
        }
    }
}
