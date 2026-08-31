using System;
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
}
