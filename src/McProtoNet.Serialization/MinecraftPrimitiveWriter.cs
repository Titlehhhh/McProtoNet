using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using McProtoNet.NBT;

namespace McProtoNet.Serialization;

/// <summary>
/// Writer for Minecraft protocol primitive types. Backed by <see cref="ArrayBufferWriter{T}"/>.
/// </summary>
public sealed class MinecraftPrimitiveWriter
{
    private readonly ArrayBufferWriter<byte> _writer;

    public ReadOnlySpan<byte> WrittenSpan => _writer.WrittenSpan;
    public ReadOnlyMemory<byte> WrittenMemory => _writer.WrittenMemory;
    internal int Capacity => _writer.Capacity;

    public MinecraftPrimitiveWriter() : this(64) { }

    public MinecraftPrimitiveWriter(int initialCapacity)
    {
        _writer = new ArrayBufferWriter<byte>(initialCapacity);
    }

    internal void Reset() => _writer.ResetWrittenCount();

    public void WriteBoolean(bool value)
    {
        var span = _writer.GetSpan(1);
        span[0] = value ? (byte)1 : (byte)0;
        _writer.Advance(1);
    }

    public void Write(ReadOnlySequence<byte> sequence)
    {
        if (sequence.IsSingleSegment)
        {
            _writer.Write(sequence.FirstSpan);
        }
        else
        {
            foreach (var memory in sequence)
                _writer.Write(memory.Span);
        }
    }

    public Span<byte> GetSpan(int size = 0) => _writer.GetSpan(size);

    public void Advance(int count) => _writer.Advance(count);

    public void WriteSignedByte(sbyte value)
    {
        var span = _writer.GetSpan(1);
        span[0] = (byte)value;
        _writer.Advance(1);
    }

    public void WriteUnsignedByte(byte value)
    {
        var span = _writer.GetSpan(1);
        span[0] = value;
        _writer.Advance(1);
    }

    public void WriteUnsignedShort(ushort value)
    {
        var span = _writer.GetSpan(sizeof(ushort));
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        _writer.Advance(sizeof(ushort));
    }

    public void WriteSignedShort(short value)
    {
        var span = _writer.GetSpan(sizeof(short));
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        _writer.Advance(sizeof(short));
    }

    public void WriteSignedInt(int value)
    {
        var span = _writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        _writer.Advance(sizeof(int));
    }

    public void WriteUnsignedInt(uint value)
    {
        var span = _writer.GetSpan(sizeof(uint));
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        _writer.Advance(sizeof(uint));
    }

    public void WriteSignedLong(long value)
    {
        var span = _writer.GetSpan(sizeof(long));
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        _writer.Advance(sizeof(long));
    }

    public void WriteUnsignedLong(ulong value)
    {
        var span = _writer.GetSpan(sizeof(ulong));
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        _writer.Advance(sizeof(ulong));
    }

    public void WriteFloat(float value)
    {
        var span = _writer.GetSpan(sizeof(float));
        BinaryPrimitives.WriteInt32BigEndian(span, BitConverter.SingleToInt32Bits(value));
        _writer.Advance(sizeof(float));
    }

    public void WriteDouble(double value)
    {
        var span = _writer.GetSpan(sizeof(double));
        BinaryPrimitives.WriteInt64BigEndian(span, BitConverter.DoubleToInt64Bits(value));
        _writer.Advance(sizeof(double));
    }

    public void WriteUUID(Guid value)
    {
        var span = _writer.GetSpan(16);
        if (!value.TryWriteBytes(span, bigEndian: true, out _))
            throw new InvalidOperationException("Guid no write");
        _writer.Advance(16);
    }

    public void WriteBuffer(ReadOnlySpan<byte> value)
    {
        _writer.Write(value);
    }

    public void WriteVarInt(int? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        WriteVarInt(value.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVarInt(int value)
    {
        Span<byte> data = stackalloc byte[5];
        var unsigned = (uint)value;
        byte len = 0;
        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;
            if (unsigned != 0) temp |= 128;
            data[len++] = temp;
        } while (unsigned != 0);

        _writer.Write(data.Slice(0, len));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVarLong(long value)
    {
        var unsigned = (ulong)value;
        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;
            if (unsigned != 0) temp |= 128;
            var span = _writer.GetSpan(1);
            span[0] = temp;
            _writer.Advance(1);
        } while (unsigned != 0);
    }

    private static readonly Encoding _utf8 = new UTF8Encoding();

    public void WriteString(scoped ReadOnlySpan<char> chars)
    {
        int length = _utf8.GetByteCount(chars);
        WriteVarInt(length);
        var span = _writer.GetSpan(length);
        if (!_utf8.TryGetBytes(chars, span, out var written))
            throw new ArgumentException("Failed to write string to buffer", nameof(chars));
        _writer.Advance(written);
    }

    public void WriteString(string value) => WriteString(value.AsSpan());

    /// <summary>
    /// Writes a presence flag followed by the NBT tag, or a lone <c>false</c> flag for null.
    /// Mirrors <see cref="MinecraftPrimitiveReader.ReadOptionalNbtTag"/>.
    /// </summary>
    /// <param name="value">The tag to write, or null.</param>
    /// <param name="writeRootTag">Whether to write the root tag's name (pre-network NBT format).</param>
    public void WriteOptionalNbt(NbtTag? value, bool writeRootTag = false)
    {
        if (value is null)
        {
            WriteBoolean(false);
        }
        else
        {
            WriteBoolean(true);
            WriteNbt(value, writeRootTag);
        }
    }

    /// <summary>
    /// Writes an NBT tag in wire format: the tag type byte, optionally the root name,
    /// then the payload. Mirrors <see cref="MinecraftPrimitiveReader.ReadNbtTag"/>.
    /// </summary>
    /// <param name="value">The tag to write.</param>
    /// <param name="writeRootTag">
    /// Whether to write the root tag's name (pre-network NBT format).
    /// False writes the nameless network root: type byte, then payload.
    /// </param>
    public void WriteNbt(NbtTag value, bool writeRootTag = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        WriteUnsignedByte((byte)value.TagType);
        if (writeRootTag)
            WriteNbtString(value.Name ?? string.Empty);
        WriteNbtPayload(value, NbtMaxDepth);
    }

    /// <summary>Nesting limit guarding against runaway recursion; matches vanilla's depth cap.</summary>
    private const int NbtMaxDepth = 512;

    private void WriteNbtPayload(NbtTag tag, int remainingDepth)
    {
        switch (tag.TagType)
        {
            case NbtTagType.Byte:
                WriteUnsignedByte(((NbtByte)tag).Value);
                break;
            case NbtTagType.Short:
                WriteSignedShort(((NbtShort)tag).Value);
                break;
            case NbtTagType.Int:
                WriteSignedInt(((NbtInt)tag).Value);
                break;
            case NbtTagType.Long:
                WriteSignedLong(((NbtLong)tag).Value);
                break;
            case NbtTagType.Float:
                WriteFloat(((NbtFloat)tag).Value);
                break;
            case NbtTagType.Double:
                WriteDouble(((NbtDouble)tag).Value);
                break;
            case NbtTagType.String:
                WriteNbtString(((NbtString)tag).Value);
                break;
            case NbtTagType.ByteArray:
            {
                byte[] data = ((NbtByteArray)tag).Value;
                WriteSignedInt(data.Length);
                WriteBuffer(data);
                break;
            }
            case NbtTagType.IntArray:
            {
                int[] data = ((NbtIntArray)tag).Value;
                WriteSignedInt(data.Length);
                WriteBigEndian(data);
                break;
            }
            case NbtTagType.LongArray:
            {
                long[] data = ((NbtLongArray)tag).Value;
                WriteSignedInt(data.Length);
                WriteBigEndian(data);
                break;
            }
            case NbtTagType.List:
            {
                if (remainingDepth == 0)
                    ThrowNbtDepthExceeded();
                var list = (NbtList)tag;
                if (list.ListType == NbtTagType.Unknown)
                    throw new NbtFormatException("NbtList had no elements and an Unknown ListType");
                WriteUnsignedByte((byte)list.ListType);
                WriteSignedInt(list.Count);
                for (var i = 0; i < list.Count; i++)
                    WriteNbtPayload(list[i], remainingDepth - 1);
                break;
            }
            case NbtTagType.Compound:
            {
                if (remainingDepth == 0)
                    ThrowNbtDepthExceeded();
                foreach (NbtTag child in (NbtCompound)tag)
                {
                    WriteUnsignedByte((byte)child.TagType);
                    // Tags inside a compound always carry a name.
                    WriteNbtString(child.Name!);
                    WriteNbtPayload(child, remainingDepth - 1);
                }

                WriteUnsignedByte((byte)NbtTagType.End);
                break;
            }
            default:
                throw new NbtFormatException($"Cannot write NBT tag of type {tag.TagType}.");
        }
    }

    /// <summary>Writes an NBT string: unsigned short big-endian byte length, then UTF-8 bytes.</summary>
    private void WriteNbtString(scoped ReadOnlySpan<char> chars)
    {
        var byteCount = _utf8.GetByteCount(chars);
        if (byteCount > ushort.MaxValue)
            throw new NbtFormatException($"NBT string too long ({byteCount} bytes, max {ushort.MaxValue}).");
        WriteUnsignedShort((ushort)byteCount);
        var span = _writer.GetSpan(byteCount);
        _utf8.GetBytes(chars, span);
        _writer.Advance(byteCount);
    }

    /// <summary>
    /// Bulk-writes ints as big-endian, byte-swapping in one vectorized pass.
    /// On little-endian hosts the batch <see cref="BinaryPrimitives.ReverseEndianness(ReadOnlySpan{int}, Span{int})"/>
    /// swaps while copying (its vector stores tolerate the unaligned destination); on big-endian hosts
    /// memory already matches the wire order, so the bytes are copied verbatim.
    /// </summary>
    private void WriteBigEndian(scoped ReadOnlySpan<int> values)
    {
        var byteCount = values.Length * sizeof(int);
        var span = _writer.GetSpan(byteCount);
        if (BitConverter.IsLittleEndian)
        {
            var target = MemoryMarshal.Cast<byte, int>(span).Slice(0, values.Length);
            BinaryPrimitives.ReverseEndianness(values, target);
        }
        else
        {
            MemoryMarshal.AsBytes(values).CopyTo(span);
        }

        _writer.Advance(byteCount);
    }

    /// <summary>
    /// Bulk-writes longs as big-endian, byte-swapping in one vectorized pass.
    /// Same endianness contract as <see cref="WriteBigEndian(ReadOnlySpan{int})"/>.
    /// </summary>
    private void WriteBigEndian(scoped ReadOnlySpan<long> values)
    {
        var byteCount = values.Length * sizeof(long);
        var span = _writer.GetSpan(byteCount);
        if (BitConverter.IsLittleEndian)
        {
            var target = MemoryMarshal.Cast<byte, long>(span).Slice(0, values.Length);
            BinaryPrimitives.ReverseEndianness(values, target);
        }
        else
        {
            MemoryMarshal.AsBytes(values).CopyTo(span);
        }

        _writer.Advance(byteCount);
    }

    [DoesNotReturn]
    private static void ThrowNbtDepthExceeded() =>
        throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {NbtMaxDepth}.");

    /// <summary>
    /// Copies written bytes into a pooled <see cref="MemoryOwner{T}"/> buffer.
    /// </summary>
    public MemoryOwner<byte> GetWrittenMemory()
    {
        var written = _writer.WrittenSpan;
        var owner = MemoryOwner<byte>.Allocate(written.Length);
        written.CopyTo(owner.Span);
        return owner;
    }

    /// <summary>
    /// No-op. <see cref="ArrayBufferWriter{T}"/> holds no rented resources.
    /// </summary>
    public void Dispose() { }
}
