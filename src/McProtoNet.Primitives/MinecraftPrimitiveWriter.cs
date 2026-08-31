using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;
using McProtoNet.NBT;

namespace McProtoNet.Primitives;

/// <summary>
/// Provides a forward-only writer for the primitive types of the Minecraft protocol, backed by an
/// <see cref="ArrayBufferWriter{T}"/>.
/// </summary>
/// <remarks>
/// This type is not thread safe. The buffer grows as needed, and
/// <see cref="WrittenSpan"/> and <see cref="WrittenMemory"/> are windows into it, so a write invalidates
/// any window handed out earlier.
/// </remarks>
public sealed class MinecraftPrimitiveWriter
{
    private readonly ArrayBufferWriter<byte> _writer;

    /// <summary>
    /// Gets the bytes written so far as a read-only span.
    /// </summary>
    public ReadOnlySpan<byte> WrittenSpan => _writer.WrittenSpan;

    /// <summary>
    /// Gets the bytes written so far as read-only memory.
    /// </summary>
    public ReadOnlyMemory<byte> WrittenMemory => _writer.WrittenMemory;

    internal int Capacity => _writer.Capacity;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftPrimitiveWriter"/> class with a buffer of 64
    /// bytes.
    /// </summary>
    public MinecraftPrimitiveWriter() : this(64) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftPrimitiveWriter"/> class with a buffer of the
    /// specified size.
    /// </summary>
    /// <param name="initialCapacity">The initial size of the buffer, in bytes. This value must be greater
    /// than zero.</param>
    /// <exception cref="ArgumentException"><paramref name="initialCapacity"/> is less than or equal to
    /// zero.</exception>
    public MinecraftPrimitiveWriter(int initialCapacity)
    {
        _writer = new ArrayBufferWriter<byte>(initialCapacity);
    }

    internal void Reset() => _writer.ResetWrittenCount();

    /// <summary>
    /// Writes a Boolean value as a single byte.
    /// </summary>
    /// <param name="value">The value to write. <see langword="true"/> is written as 1 and
    /// <see langword="false"/> as 0.</param>
    public void WriteBoolean(bool value)
    {
        var span = _writer.GetSpan(1);
        span[0] = value ? (byte)1 : (byte)0;
        _writer.Advance(1);
    }

    /// <summary>
    /// Writes the contents of a sequence.
    /// </summary>
    /// <param name="sequence">The sequence whose bytes are copied into the buffer.</param>
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

    /// <summary>
    /// Returns a span to write to that is at least the requested size.
    /// </summary>
    /// <param name="size">The minimum number of bytes the returned span must hold. This value must not be
    /// negative. The default is 0, which requests a non-empty span of any size.</param>
    /// <returns>A span of at least <paramref name="size"/> bytes. The bytes it holds are not
    /// cleared.</returns>
    /// <exception cref="ArgumentException"><paramref name="size"/> is negative.</exception>
    /// <remarks>
    /// The bytes written into the returned span become part of the output only after
    /// <see cref="Advance"/> is called.
    /// </remarks>
    public Span<byte> GetSpan(int size = 0) => _writer.GetSpan(size);

    /// <summary>
    /// Marks the specified number of bytes of the span returned by <see cref="GetSpan"/> as written.
    /// </summary>
    /// <param name="count">The number of bytes written. This value must not be negative.</param>
    /// <exception cref="ArgumentException"><paramref name="count"/> is negative.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="count"/> advances past the end of the
    /// buffer.</exception>
    public void Advance(int count) => _writer.Advance(count);

    /// <summary>
    /// Writes a signed 8-bit integer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteSignedByte(sbyte value)
    {
        var span = _writer.GetSpan(1);
        span[0] = (byte)value;
        _writer.Advance(1);
    }

    /// <summary>
    /// Writes an unsigned 8-bit integer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUnsignedByte(byte value)
    {
        var span = _writer.GetSpan(1);
        span[0] = value;
        _writer.Advance(1);
    }

    /// <summary>
    /// Writes an unsigned 16-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUnsignedShort(ushort value)
    {
        var span = _writer.GetSpan(sizeof(ushort));
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        _writer.Advance(sizeof(ushort));
    }

    /// <summary>
    /// Writes a signed 16-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteSignedShort(short value)
    {
        var span = _writer.GetSpan(sizeof(short));
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        _writer.Advance(sizeof(short));
    }

    /// <summary>
    /// Writes a signed 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteSignedInt(int value)
    {
        var span = _writer.GetSpan(sizeof(int));
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        _writer.Advance(sizeof(int));
    }

    /// <summary>
    /// Writes an unsigned 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUnsignedInt(uint value)
    {
        var span = _writer.GetSpan(sizeof(uint));
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        _writer.Advance(sizeof(uint));
    }

    /// <summary>
    /// Writes a signed 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteSignedLong(long value)
    {
        var span = _writer.GetSpan(sizeof(long));
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        _writer.Advance(sizeof(long));
    }

    /// <summary>
    /// Writes an unsigned 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUnsignedLong(ulong value)
    {
        var span = _writer.GetSpan(sizeof(ulong));
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        _writer.Advance(sizeof(ulong));
    }

    /// <summary>
    /// Writes a single-precision floating-point number in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteFloat(float value)
    {
        var span = _writer.GetSpan(sizeof(float));
        BinaryPrimitives.WriteInt32BigEndian(span, BitConverter.SingleToInt32Bits(value));
        _writer.Advance(sizeof(float));
    }

    /// <summary>
    /// Writes a double-precision floating-point number in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteDouble(double value)
    {
        var span = _writer.GetSpan(sizeof(double));
        BinaryPrimitives.WriteInt64BigEndian(span, BitConverter.DoubleToInt64Bits(value));
        _writer.Advance(sizeof(double));
    }

    /// <summary>
    /// Writes a UUID as 16 bytes in big-endian order.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="InvalidOperationException">The value could not be written to the
    /// buffer.</exception>
    public void WriteUUID(Guid value)
    {
        var span = _writer.GetSpan(16);
        if (!value.TryWriteBytes(span, bigEndian: true, out _))
            throw new InvalidOperationException("Guid no write");
        _writer.Advance(16);
    }

    /// <summary>
    /// Writes the contents of a span, without a length prefix.
    /// </summary>
    /// <param name="value">The bytes to write.</param>
    public void WriteBuffer(ReadOnlySpan<byte> value)
    {
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a nullable value as a VarInt.
    /// </summary>
    /// <param name="value">The value to encode. It must have a value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> has no value.</exception>
    public void WriteVarInt(int? value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));
        WriteVarInt(value.Value);
    }

    /// <summary>
    /// Writes a value as a VarInt of 1 to 5 bytes.
    /// </summary>
    /// <param name="value">The value to encode. A negative value always occupies 5 bytes.</param>
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

    /// <summary>
    /// Writes a value as a VarLong of 1 to 10 bytes.
    /// </summary>
    /// <param name="value">The value to encode. A negative value always occupies 10 bytes.</param>
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

    /// <summary>
    /// Writes characters as a UTF-8 string prefixed with its length in bytes as a VarInt.
    /// </summary>
    /// <param name="chars">The characters to write.</param>
    /// <exception cref="ArgumentException">The encoded bytes could not be written to the
    /// buffer.</exception>
    public void WriteString(scoped ReadOnlySpan<char> chars)
    {
        int length = _utf8.GetByteCount(chars);
        WriteVarInt(length);
        var span = _writer.GetSpan(length);
        if (!_utf8.TryGetBytes(chars, span, out var written))
            throw new ArgumentException("Failed to write string to buffer", nameof(chars));
        _writer.Advance(written);
    }

    /// <summary>
    /// Writes a string as UTF-8, prefixed with its length in bytes as a VarInt.
    /// </summary>
    /// <param name="value">The string to write. <see langword="null"/> is written as an empty
    /// string.</param>
    /// <exception cref="ArgumentException">The encoded bytes could not be written to the
    /// buffer.</exception>
    public void WriteString(string value) => WriteString(value.AsSpan());

    /// <summary>
    /// Writes a presence flag followed by an NBT tag, or a lone unset flag when there is no tag.
    /// </summary>
    /// <param name="value">The tag to write, or <see langword="null"/> to write only an unset presence
    /// flag.</param>
    /// <param name="writeRootTag"><see langword="true"/> to write the name of the root tag, which is the
    /// pre-network NBT format; <see langword="false"/> to write the nameless network root. The default is
    /// <see langword="false"/>.</param>
    /// <seealso cref="MinecraftPrimitiveReader.ReadOptionalNbtTag"/>
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
    /// Writes an NBT tag in wire format.
    /// </summary>
    /// <param name="value">The tag to write.</param>
    /// <param name="writeRootTag"><see langword="true"/> to write the name of the root tag, which is the
    /// pre-network NBT format; <see langword="false"/> to write the nameless network root. The default is
    /// <see langword="false"/>.</param>
    /// <remarks>
    /// The wire format is the tag type byte, then the root name when <paramref name="writeRootTag"/> is
    /// <see langword="true"/>, then the payload.
    /// </remarks>
    /// <seealso cref="MinecraftPrimitiveReader.ReadNbtTag"/>
    public void WriteNbt(NbtTag value, bool writeRootTag = false)
    {
        NbtBufferWriter.WriteTag(_writer, value, writeRootTag);
    }

    /// <summary>
    /// Copies the bytes written so far into a buffer rented from the shared array pool.
    /// </summary>
    /// <returns>A <see cref="MemoryOwner{T}"/> that holds a copy of the written bytes. The caller owns it
    /// and must dispose it to return the buffer to the pool.</returns>
    public MemoryOwner<byte> GetWrittenMemory()
    {
        var written = _writer.WrittenSpan;
        var owner = MemoryOwner<byte>.Allocate(written.Length);
        written.CopyTo(owner.Span);
        return owner;
    }

    /// <summary>
    /// Releases all resources used by the current instance of the
    /// <see cref="MinecraftPrimitiveWriter"/> class.
    /// </summary>
    /// <remarks>
    /// This method does nothing. The writer holds no rented or unmanaged resources.
    /// </remarks>
    public void Dispose() { }
}
