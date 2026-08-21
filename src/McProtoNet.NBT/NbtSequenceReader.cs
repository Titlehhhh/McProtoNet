using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace McProtoNet.NBT;

/// <summary>
///     Reads NBT tags directly from a <see cref="SequenceReader{T}" />, so callers can parse
///     tags out of a possibly multi-segment <see cref="ReadOnlySequence{T}" /> without copying
///     the buffer first. Counterpart of <see cref="NbtSpanReader" /> for non-contiguous input.
///     The reader is advanced by exactly the number of bytes the tag occupies.
/// </summary>
public static class NbtSequenceReader
{
    private const int StackAllocByteThreshold = 256;

    /// <summary>
    ///     Reads one NBT tag from the sequence. Returns null when the first byte is TAG_End.
    /// </summary>
    /// <param name="reader">The sequence reader positioned at the tag type byte.</param>
    /// <param name="readRootName">Whether the root tag carries a name (pre-network NBT format).</param>
    /// <exception cref="NbtFormatException">The data is malformed, truncated, or too deeply nested.</exception>
    public static NbtTag? ReadTag(ref SequenceReader<byte> reader, bool readRootName)
    {
        var type = ReadTagType(ref reader);
        if (type == NbtTagType.End)
            return null;

        var rootName = readRootName ? ReadString(ref reader) : null;
        return ReadPayload(ref reader, type, rootName, NbtLimits.MaxDepth);
    }

    private static NbtTag ReadPayload(ref SequenceReader<byte> reader, NbtTagType type, string? name,
        int remainingDepth)
    {
        switch (type)
        {
            case NbtTagType.Byte:
                return new NbtByte(name, ReadByte(ref reader));
            case NbtTagType.Short:
                return new NbtShort(name, ReadInt16(ref reader));
            case NbtTagType.Int:
                return new NbtInt(name, ReadInt32(ref reader));
            case NbtTagType.Long:
                return new NbtLong(name, ReadInt64(ref reader));
            case NbtTagType.Float:
                return new NbtFloat(name, Unsafe.BitCast<int, float>(ReadInt32(ref reader)));
            case NbtTagType.Double:
                return new NbtDouble(name, Unsafe.BitCast<long, double>(ReadInt64(ref reader)));
            case NbtTagType.String:
                return new NbtString(name, ReadString(ref reader));
            case NbtTagType.ByteArray:
            {
                var result = new byte[ReadLength(ref reader, sizeof(byte))];
                Fill(ref reader, result);
                return NbtByteArray.CreateFromArray(result, name);
            }
            case NbtTagType.IntArray:
            {
                var result = new int[ReadLength(ref reader, sizeof(int))];
                Fill(ref reader, MemoryMarshal.AsBytes(result.AsSpan()));
                if (BitConverter.IsLittleEndian)
                    BinaryPrimitives.ReverseEndianness(result, result);
                return NbtIntArray.CreateFromArray(result, name);
            }
            case NbtTagType.LongArray:
            {
                var result = new long[ReadLength(ref reader, sizeof(long))];
                Fill(ref reader, MemoryMarshal.AsBytes(result.AsSpan()));
                if (BitConverter.IsLittleEndian)
                    BinaryPrimitives.ReverseEndianness(result, result);
                return NbtLongArray.CreateFromArray(result, name);
            }
            case NbtTagType.List:
                return ReadList(ref reader, name, remainingDepth);
            case NbtTagType.Compound:
                return ReadCompound(ref reader, name, remainingDepth);
            default:
                throw new NbtFormatException($"Cannot read NBT tag of type {type}.");
        }
    }

    private static NbtList ReadList(ref SequenceReader<byte> reader, string? name, int remainingDepth)
    {
        if (remainingDepth == 0)
            ThrowDepthExceeded();

        var elementType = ReadTagType(ref reader);
        var length = ReadLength(ref reader, 1);
        if (length > 0 && elementType == NbtTagType.End)
            throw new NbtFormatException("Non-empty NBT list of TAG_End elements.");

        var list = new NbtList(name, elementType);
        for (var i = 0; i < length; i++)
            list.Add(ReadPayload(ref reader, elementType, null, remainingDepth - 1));
        return list;
    }

    private static NbtCompound ReadCompound(ref SequenceReader<byte> reader, string? name, int remainingDepth)
    {
        if (remainingDepth == 0)
            ThrowDepthExceeded();

        var compound = new NbtCompound { Name = name };
        while (true)
        {
            var childType = ReadTagType(ref reader);
            if (childType == NbtTagType.End)
                return compound;

            var childName = ReadString(ref reader);
            compound.SetOrReplace(ReadPayload(ref reader, childType, childName, remainingDepth - 1));
        }
    }

    private static NbtTagType ReadTagType(ref SequenceReader<byte> reader)
    {
        var type = ReadByte(ref reader);
        if (type > (byte)NbtTagType.LongArray)
            throw new NbtFormatException("NBT tag type out of range: " + type);
        return (NbtTagType)type;
    }

    private static string ReadString(ref SequenceReader<byte> reader)
    {
        int length = (ushort)ReadInt16(ref reader);
        if (length == 0)
            return string.Empty;
        if (length > reader.Remaining)
            ThrowEndOfData();

        if (reader.UnreadSpan.Length >= length)
        {
            var result = ModifiedUtf8.GetString(reader.UnreadSpan.Slice(0, length));
            reader.Advance(length);
            return result;
        }

        if (length <= StackAllocByteThreshold)
        {
            Span<byte> buffer = stackalloc byte[StackAllocByteThreshold];
            Fill(ref reader, buffer.Slice(0, length));
            return ModifiedUtf8.GetString(buffer.Slice(0, length));
        }

        var rented = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            Fill(ref reader, rented.AsSpan(0, length));
            return ModifiedUtf8.GetString(rented.AsSpan(0, length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private static int ReadLength(ref SequenceReader<byte> reader, int elementSize)
    {
        var length = ReadInt32(ref reader);
        if (length < 0)
            throw new NbtFormatException($"Negative tag length given: {length}");
        if ((long)length * elementSize > reader.Remaining)
            ThrowEndOfData();
        return length;
    }

    private static void Fill(ref SequenceReader<byte> reader, scoped Span<byte> destination)
    {
        if (!reader.TryCopyTo(destination))
            ThrowEndOfData();
        reader.Advance(destination.Length);
    }

    private static byte ReadByte(ref SequenceReader<byte> reader)
    {
        if (!reader.TryRead(out var value))
            ThrowEndOfData();
        return value;
    }

    private static short ReadInt16(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadBigEndian(out short value))
            ThrowEndOfData();
        return value;
    }

    private static int ReadInt32(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadBigEndian(out int value))
            ThrowEndOfData();
        return value;
    }

    private static long ReadInt64(ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadBigEndian(out long value))
            ThrowEndOfData();
        return value;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowEndOfData() =>
        throw new NbtFormatException("Unexpected end of NBT data.");

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowDepthExceeded() =>
        throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {NbtLimits.MaxDepth}.");
}
