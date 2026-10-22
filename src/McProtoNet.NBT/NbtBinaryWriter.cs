using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace McProtoNet.NBT;

/// <summary>Provides a stream writer for NBT primitives: numbers big-endian, strings in modified UTF-8.</summary>
internal sealed class NbtBinaryWriter
{
    /// <summary>The largest number of bytes written to the stream in one call.</summary>
    public const int MaxWriteChunk = 4 * 1024 * 1024;

    private const int BufferSize = 256;

    private readonly byte[] _buffer = new byte[BufferSize];
    private readonly Stream _stream;

    private int _depth;

    public NbtBinaryWriter(Stream input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (!input.CanWrite) throw new ArgumentException("Given stream must be writable", nameof(input));
        _stream = input;
    }

    public Stream BaseStream
    {
        get
        {
            _stream.Flush();
            return _stream;
        }
    }

    public void EnterLevel()
    {
        if (++_depth > NbtLimits.MaxDepth)
            throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {NbtLimits.MaxDepth}.");
    }

    public void ExitLevel()
    {
        _depth--;
    }

    public void Write(byte value)
    {
        _stream.WriteByte(value);
    }

    public void Write(NbtTagType value)
    {
        _stream.WriteByte((byte)value);
    }

    public void Write(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(_buffer, value);
        _stream.Write(_buffer, 0, sizeof(short));
    }

    public void Write(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(_buffer, value);
        _stream.Write(_buffer, 0, sizeof(ushort));
    }

    public void Write(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(_buffer, value);
        _stream.Write(_buffer, 0, sizeof(int));
    }

    public void Write(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(_buffer, value);
        _stream.Write(_buffer, 0, sizeof(long));
    }

    public void Write(float value)
    {
        Write(BitConverter.SingleToInt32Bits(value));
    }

    public void Write(double value)
    {
        Write(BitConverter.DoubleToInt64Bits(value));
    }

    /// <summary>Writes an NBT string: an unsigned big-endian byte count, then the modified UTF-8
    /// bytes.</summary>
    public void Write(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var byteCount = ModifiedUtf8.GetByteCount(value);
        if (byteCount > NbtLimits.MaxStringByteLength)
            throw new NbtFormatException(
                $"NBT string too long ({byteCount} bytes, max {NbtLimits.MaxStringByteLength}).");

        Write((ushort)byteCount);

        ReadOnlySpan<char> chars = value;
        while (!chars.IsEmpty)
        {
            var status = ModifiedUtf8.FromUtf16(chars, _buffer, out var charsRead, out var written);
            _stream.Write(_buffer, 0, written);
            chars = chars.Slice(charsRead);
            if (status == OperationStatus.Done) break;
        }
    }

    public void WriteBigEndian(ReadOnlySpan<int> values)
    {
        WriteBigEndian<int>(values);
    }

    public void WriteBigEndian(ReadOnlySpan<long> values)
    {
        WriteBigEndian<long>(values);
    }

    public void Write(byte[] data, int offset, int count)
    {
        var written = 0;
        while (written < count)
        {
            var toWrite = Math.Min(MaxWriteChunk, count - written);
            _stream.Write(data, offset + written, toWrite);
            written += toWrite;
        }
    }

    private void WriteBigEndian<T>(ReadOnlySpan<T> values) where T : unmanaged
    {
        var elementSize = Unsafe.SizeOf<T>();
        var perChunk = BufferSize / elementSize;
        while (!values.IsEmpty)
        {
            var take = Math.Min(perChunk, values.Length);
            var byteCount = take * elementSize;
            BigEndianArray.ToBigEndian(values.Slice(0, take), _buffer.AsSpan(0, byteCount));
            _stream.Write(_buffer, 0, byteCount);
            values = values.Slice(take);
        }
    }
}
