using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace McProtoNet.NBT;

/// <summary>
/// Provides methods that write NBT data to an <see cref="IBufferWriter{T}"/>.
/// </summary>
/// <remarks>
/// The output is Java Edition NBT only: every number is big-endian and every string is modified UTF-8.
/// Nesting is limited to <see cref="MaxDepth"/> levels. <see cref="NbtSpanReader"/> is the counterpart on
/// the reading side.
/// </remarks>
public static class NbtBufferWriter
{
    /// <summary>The maximum nesting depth the writer accepts, which matches the vanilla limit.</summary>
    public const int MaxDepth = NbtLimits.MaxDepth;

    /// <summary>
    /// Writes a complete tag: the type byte, the root name when requested, then the payload.
    /// </summary>
    /// <typeparam name="TWriter">The type of the destination buffer writer.</typeparam>
    /// <param name="writer">The buffer writer to write to.</param>
    /// <param name="tag">The tag to write.</param>
    /// <param name="writeRootName"><see langword="true"/> to write the root tag's name after the type byte,
    /// as the file format requires; <see langword="false"/> to write the nameless root of the network format
    /// used since 1.20.2. The default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">The tag is malformed, nested too deeply, holds a string
    /// longer than 65535 bytes when encoded, or holds an array whose encoded length does not fit in an
    /// <see cref="int"/>.</exception>
    public static void WriteTag<TWriter>(TWriter writer, NbtTag tag, bool writeRootName = false)
        where TWriter : IBufferWriter<byte>
    {
        ArgumentNullException.ThrowIfNull(tag);

        WriteByte(writer, (byte)tag.TagType);
        if (writeRootName) WriteString(writer, tag.Name ?? string.Empty);
        WritePayload(writer, tag, MaxDepth);
    }

    /// <summary>
    /// Writes only a tag's payload, without the type byte and without the name.
    /// </summary>
    /// <typeparam name="TWriter">The type of the destination buffer writer.</typeparam>
    /// <param name="writer">The buffer writer to write to.</param>
    /// <param name="tag">The tag whose payload is written.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">The tag is malformed, nested too deeply, holds a string
    /// longer than 65535 bytes when encoded, or holds an array whose encoded length does not fit in an
    /// <see cref="int"/>.</exception>
    public static void WritePayload<TWriter>(TWriter writer, NbtTag tag)
        where TWriter : IBufferWriter<byte>
    {
        ArgumentNullException.ThrowIfNull(tag);
        WritePayload(writer, tag, MaxDepth);
    }

    /// <summary>
    /// Writes an NBT string: an unsigned big-endian 16-bit byte count, then the modified UTF-8 bytes.
    /// </summary>
    /// <typeparam name="TWriter">The type of the destination buffer writer.</typeparam>
    /// <param name="writer">The buffer writer to write to.</param>
    /// <param name="value">The text to write.</param>
    /// <exception cref="ArgumentOutOfRangeException">The encoded form of <paramref name="value"/> does not
    /// fit in an <see cref="int"/>.</exception>
    /// <exception cref="NbtFormatException">The encoded text is longer than 65535 bytes.</exception>
    public static void WriteString<TWriter>(TWriter writer, scoped ReadOnlySpan<char> value)
        where TWriter : IBufferWriter<byte>
    {
        var byteCount = ModifiedUtf8.GetByteCount(value);
        if (byteCount > NbtLimits.MaxStringByteLength) ThrowStringTooLong(byteCount);

        var span = writer.GetSpan(sizeof(ushort) + byteCount);
        BinaryPrimitives.WriteUInt16BigEndian(span, (ushort)byteCount);
        ModifiedUtf8.GetBytes(value, span.Slice(sizeof(ushort)));
        writer.Advance(sizeof(ushort) + byteCount);
    }

    private static void WritePayload<TWriter>(TWriter writer, NbtTag tag, int remainingDepth)
        where TWriter : IBufferWriter<byte>
    {
        switch (tag.TagType)
        {
            case NbtTagType.Byte:
                WriteByte(writer, ((NbtByte)tag).Value);
                break;
            case NbtTagType.Short:
                WriteInt16(writer, ((NbtShort)tag).Value);
                break;
            case NbtTagType.Int:
                WriteInt32(writer, ((NbtInt)tag).Value);
                break;
            case NbtTagType.Long:
                WriteInt64(writer, ((NbtLong)tag).Value);
                break;
            case NbtTagType.Float:
                WriteInt32(writer, BitConverter.SingleToInt32Bits(((NbtFloat)tag).Value));
                break;
            case NbtTagType.Double:
                WriteInt64(writer, BitConverter.DoubleToInt64Bits(((NbtDouble)tag).Value));
                break;
            case NbtTagType.String:
                WriteString(writer, ((NbtString)tag).Value);
                break;
            case NbtTagType.ByteArray:
            {
                var data = ((NbtByteArray)tag).Value;
                WriteInt32(writer, data.Length);
                writer.Write(data);
                break;
            }
            case NbtTagType.IntArray:
            {
                var data = ((NbtIntArray)tag).Value;
                WriteInt32(writer, data.Length);
                WriteBigEndian(writer, data);
                break;
            }
            case NbtTagType.LongArray:
            {
                var data = ((NbtLongArray)tag).Value;
                WriteInt32(writer, data.Length);
                WriteBigEndian(writer, data);
                break;
            }
            case NbtTagType.List:
            {
                if (remainingDepth == 0) ThrowDepthExceeded();
                var list = (NbtList)tag;
                WriteByte(writer, (byte)list.ListType);
                WriteInt32(writer, list.Count);
                for (var i = 0; i < list.Count; i++)
                    WritePayload(writer, list[i], remainingDepth - 1);
                break;
            }
            case NbtTagType.Compound:
            {
                if (remainingDepth == 0) ThrowDepthExceeded();
                foreach (var child in (NbtCompound)tag)
                {
                    WriteByte(writer, (byte)child.TagType);
                    WriteString(writer, child.Name!);
                    WritePayload(writer, child, remainingDepth - 1);
                }

                WriteByte(writer, (byte)NbtTagType.End);
                break;
            }
            default:
                throw new NbtFormatException($"Cannot write NBT tag of type {tag.TagType}.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteByte<TWriter>(TWriter writer, byte value) where TWriter : IBufferWriter<byte>
    {
        var span = writer.GetSpan(1);
        span[0] = value;
        writer.Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteInt16<TWriter>(TWriter writer, short value) where TWriter : IBufferWriter<byte>
    {
        BinaryPrimitives.WriteInt16BigEndian(writer.GetSpan(sizeof(short)), value);
        writer.Advance(sizeof(short));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteInt32<TWriter>(TWriter writer, int value) where TWriter : IBufferWriter<byte>
    {
        BinaryPrimitives.WriteInt32BigEndian(writer.GetSpan(sizeof(int)), value);
        writer.Advance(sizeof(int));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteInt64<TWriter>(TWriter writer, long value) where TWriter : IBufferWriter<byte>
    {
        BinaryPrimitives.WriteInt64BigEndian(writer.GetSpan(sizeof(long)), value);
        writer.Advance(sizeof(long));
    }

    private static void WriteBigEndian<TWriter, T>(TWriter writer, scoped ReadOnlySpan<T> values)
        where TWriter : IBufferWriter<byte> where T : unmanaged
    {
        var byteCount = ByteCount(values.Length, Unsafe.SizeOf<T>());
        var span = writer.GetSpan(byteCount);
        BigEndianArray.ToBigEndian(values, span.Slice(0, byteCount));
        writer.Advance(byteCount);
    }

    private static int ByteCount(int elementCount, int elementSize)
    {
        var byteCount = (long)elementCount * elementSize;
        if (byteCount > int.MaxValue)
            throw new NbtFormatException($"NBT array of {elementCount} elements is too large to write.");
        return (int)byteCount;
    }

    [DoesNotReturn]
    private static void ThrowStringTooLong(int byteCount) =>
        throw new NbtFormatException(
            $"NBT string too long ({byteCount} bytes, max {NbtLimits.MaxStringByteLength}).");

    [DoesNotReturn]
    private static void ThrowDepthExceeded() =>
        throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {MaxDepth}.");
}
