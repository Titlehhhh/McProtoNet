using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using McProtoNet.NBT;

namespace McProtoNet.Primitives;

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
        NbtBufferWriter.WriteTag(_writer, value, writeRootTag);
    }

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
