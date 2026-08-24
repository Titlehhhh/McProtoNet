using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace McProtoNet.NBT;

/// <summary>
/// Provides a forward-only reader that reads NBT data from a contiguous buffer.
/// </summary>
/// <remarks>
/// The input is Java Edition NBT only: every number is big-endian and every string is modified UTF-8.
/// Nesting is limited to 512 levels, and every declared length is checked against the bytes that remain
/// before anything is allocated.
/// </remarks>
public ref struct NbtSpanReader
{
    private SpanBinaryReader _reader;

    /// <summary>
    /// Gets the number of bytes consumed from the buffer so far.
    /// </summary>
    public int ConsumedCount => _reader.ConsumedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtSpanReader"/> structure over the specified buffer.
    /// </summary>
    /// <param name="data">The buffer, positioned at the tag type byte.</param>
    public NbtSpanReader(ReadOnlySpan<byte> data)
    {
        _reader = new SpanBinaryReader(data);
    }

    /// <summary>
    /// Reads one complete tag and advances the reader past it.
    /// </summary>
    /// <typeparam name="T">The expected type of the root tag.</typeparam>
    /// <param name="readRootName"><see langword="true"/> to read the root tag's name after the type byte, as
    /// the file format requires; <see langword="false"/> to expect the nameless root of the network format
    /// used since 1.20.2.</param>
    /// <returns>The tag that was read, or <see langword="null"/> if the first byte is TAG_End.</returns>
    /// <exception cref="NbtFormatException">
    /// The data is malformed, truncated, or nested too deeply.
    /// -or-
    /// The root tag is not of type <typeparamref name="T"/>.
    /// </exception>
    public T? ReadAsTag<T>(bool readRootName) where T : NbtTag
    {
        var type = ReadTagType();
        if (type == NbtTagType.End) return null;

        var rootName = readRootName ? ReadString() : null;
        var tag = ReadPayload(type, rootName, NbtLimits.MaxDepth);
        if (tag is T typed) return typed;
        throw new NbtFormatException(
            $"NBT root tag is {tag.TagType}, which cannot be read as {typeof(T).Name}.");
    }

    private NbtTag ReadPayload(NbtTagType type, string? name, int remainingDepth)
    {
        switch (type)
        {
            case NbtTagType.Byte:
                return new NbtByte(name, _reader.Read());
            case NbtTagType.Short:
                return new NbtShort(name, _reader.ReadBigEndian16());
            case NbtTagType.Int:
                return new NbtInt(name, _reader.ReadBigEndian32());
            case NbtTagType.Long:
                return new NbtLong(name, _reader.ReadBigEndian64());
            case NbtTagType.Float:
                return new NbtFloat(name, Unsafe.BitCast<int, float>(_reader.ReadBigEndian32()));
            case NbtTagType.Double:
                return new NbtDouble(name, Unsafe.BitCast<long, double>(_reader.ReadBigEndian64()));
            case NbtTagType.String:
                return new NbtString(name, ReadString());
            case NbtTagType.ByteArray:
                return NbtByteArray.CreateFromArray(_reader.Read(ReadLength(sizeof(byte))).ToArray(), name);
            case NbtTagType.IntArray:
                return NbtIntArray.CreateFromArray(ReadBigEndianArray<int>(ReadLength(sizeof(int))), name);
            case NbtTagType.LongArray:
                return NbtLongArray.CreateFromArray(ReadBigEndianArray<long>(ReadLength(sizeof(long))), name);
            case NbtTagType.List:
                return ReadList(name, remainingDepth);
            case NbtTagType.Compound:
                return ReadCompound(name, remainingDepth);
            default:
                throw new NbtFormatException($"Cannot read NBT tag of type {type} at byte {_reader.ConsumedCount}.");
        }
    }

    private NbtList ReadList(string? name, int remainingDepth)
    {
        if (remainingDepth == 0) ThrowDepthExceeded();

        var elementType = ReadTagType();
        var length = ReadLength(1);
        if (length > 0 && elementType == NbtTagType.End)
            throw new NbtFormatException("Non-empty NBT list of TAG_End elements.");

        if (TryReadPrimitiveList(elementType, length, out var primitives))
        {
            primitives.Name = name;
            return primitives;
        }

        var list = new NbtList(name, elementType);
        for (var i = 0; i < length; i++)
            list.Add(ReadPayload(elementType, null, remainingDepth - 1));
        return list;
    }

    private NbtCompound ReadCompound(string? name, int remainingDepth)
    {
        if (remainingDepth == 0) ThrowDepthExceeded();

        var compound = new NbtCompound { Name = name };
        while (true)
        {
            var childType = ReadTagType();
            if (childType == NbtTagType.End) return compound;

            var childName = ReadString();
            compound.SetOrReplace(ReadPayload(childType, childName, remainingDepth - 1));
        }
    }

    private bool TryReadPrimitiveList(NbtTagType elementType, int length, out NbtList list)
    {
        switch (elementType)
        {
            case NbtTagType.Byte:
            {
                list = new NbtList(NbtTagType.Byte);
                foreach (var value in _reader.Read(length))
                    list.Add(new NbtByte(value));
                return true;
            }
            case NbtTagType.Short:
            {
                list = new NbtList(NbtTagType.Short);
                foreach (var value in ReadBigEndianArray<short>(length))
                    list.Add(new NbtShort(value));
                return true;
            }
            case NbtTagType.Int:
            {
                list = new NbtList(NbtTagType.Int);
                foreach (var value in ReadBigEndianArray<int>(length))
                    list.Add(new NbtInt(value));
                return true;
            }
            case NbtTagType.Long:
            {
                list = new NbtList(NbtTagType.Long);
                foreach (var value in ReadBigEndianArray<long>(length))
                    list.Add(new NbtLong(value));
                return true;
            }
            case NbtTagType.Float:
            {
                list = new NbtList(NbtTagType.Float);
                foreach (var value in ReadBigEndianArray<int>(length))
                    list.Add(new NbtFloat(Unsafe.BitCast<int, float>(value)));
                return true;
            }
            case NbtTagType.Double:
            {
                list = new NbtList(NbtTagType.Double);
                foreach (var value in ReadBigEndianArray<long>(length))
                    list.Add(new NbtDouble(Unsafe.BitCast<long, double>(value)));
                return true;
            }
            default:
                list = null!;
                return false;
        }
    }

    private T[] ReadBigEndianArray<T>(int count) where T : unmanaged
    {
        if (count == 0) return [];
        _reader.EnsureRemaining((long)count * Unsafe.SizeOf<T>());
        var source = _reader.Read(count * Unsafe.SizeOf<T>());
        var result = new T[count];
        BigEndianArray.FromBigEndian<T>(source, result);
        return result;
    }

    private int ReadLength(int elementSize)
    {
        var length = _reader.ReadBigEndian32();
        if (length < 0)
            throw new NbtFormatException($"Negative NBT length {length} at byte {_reader.ConsumedCount}.");
        _reader.EnsureRemaining((long)length * elementSize);
        return length;
    }

    private NbtTagType ReadTagType()
    {
        var type = _reader.Read();
        if (type > (byte)NbtTagType.LongArray)
            throw new NbtFormatException($"NBT tag type out of range: {type} at byte {_reader.ConsumedCount - 1}.");
        return (NbtTagType)type;
    }

    internal string ReadString()
    {
        int length = (ushort)_reader.ReadBigEndian16();
        return length == 0 ? string.Empty : ModifiedUtf8.GetString(_reader.Read(length));
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowDepthExceeded() =>
        throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {NbtLimits.MaxDepth}.");
}
