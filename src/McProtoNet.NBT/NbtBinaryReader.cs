using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace McProtoNet.NBT;

/// <summary>Provides a stream reader for NBT primitives: numbers big-endian, strings in modified UTF-8.</summary>
internal sealed class NbtBinaryReader : BinaryReader
{
    private const int SeekBufferSize = 8 * 1024;
    private const int MaxInitialArrayElements = 64 * 1024;

    private readonly byte[] _buffer = new byte[sizeof(double)];
    private readonly byte[] _stringConversionBuffer = new byte[128];

    private int _depth;
    private byte[]? _seekBuffer;

    public NbtBinaryReader(Stream input)
        : base(input, Encoding.UTF8, true)
    {
    }

    public NbtTagType ReadTagType()
    {
        int type = ReadByte();
        if (type > (int)NbtTagType.LongArray)
            throw new NbtFormatException("NBT tag type out of range: " + type);
        return (NbtTagType)type;
    }

    public override short ReadInt16()
    {
        FillBuffer(sizeof(short));
        return BinaryPrimitives.ReadInt16BigEndian(_buffer);
    }

    public override ushort ReadUInt16()
    {
        FillBuffer(sizeof(ushort));
        return BinaryPrimitives.ReadUInt16BigEndian(_buffer);
    }

    public override int ReadInt32()
    {
        FillBuffer(sizeof(int));
        return BinaryPrimitives.ReadInt32BigEndian(_buffer);
    }

    public override long ReadInt64()
    {
        FillBuffer(sizeof(long));
        return BinaryPrimitives.ReadInt64BigEndian(_buffer);
    }

    public override float ReadSingle()
    {
        return Unsafe.BitCast<int, float>(ReadInt32());
    }

    public override double ReadDouble()
    {
        return Unsafe.BitCast<long, double>(ReadInt64());
    }

    public override string ReadString()
    {
        int length = ReadUInt16();
        if (length == 0) return string.Empty;

        if (length <= _stringConversionBuffer.Length)
        {
            FillExactly(_stringConversionBuffer.AsSpan(0, length));
            return ModifiedUtf8.GetString(_stringConversionBuffer.AsSpan(0, length));
        }

        var rented = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            FillExactly(rented.AsSpan(0, length));
            return ModifiedUtf8.GetString(rented.AsSpan(0, length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public void SkipString()
    {
        Skip(ReadUInt16());
    }

    public void Skip(long bytesToSkip)
    {
        if (bytesToSkip < 0) throw new NbtFormatException("Negative NBT payload length: " + bytesToSkip);
        if (bytesToSkip == 0) return;
        EnsureAvailable(bytesToSkip);

        if (BaseStream.CanSeek)
        {
            BaseStream.Position += bytesToSkip;
            return;
        }

        _seekBuffer ??= new byte[SeekBufferSize];
        long skipped = 0;
        while (skipped < bytesToSkip)
        {
            var read = BaseStream.Read(_seekBuffer, 0, (int)Math.Min(SeekBufferSize, bytesToSkip - skipped));
            if (read == 0) throw new EndOfStreamException();
            skipped += read;
        }
    }

    /// <summary>Reads a big-endian array payload of the specified element count.</summary>
    public T[] ReadArrayBigEndian<T>(int count) where T : unmanaged
    {
        if (count < 0) throw new NbtFormatException("Negative array length given: " + count);
        if (count == 0) return [];

        var elementSize = Unsafe.SizeOf<T>();
        var totalBytes = (long)count * elementSize;
        if (totalBytes > int.MaxValue)
            throw new NbtFormatException($"NBT array of {count} elements is too large to allocate.");
        EnsureAvailable(totalBytes);

        var result = new T[Math.Min(count, MaxInitialArrayElements)];
        var filled = 0;
        while (filled < totalBytes)
        {
            if (filled == result.Length * elementSize)
                Array.Resize(ref result, (int)Math.Min(count, 2L * result.Length));

            var view = MemoryMarshal.AsBytes(result.AsSpan());
            var read = BaseStream.Read(view.Slice(filled, (int)Math.Min(view.Length - filled, totalBytes - filled)));
            if (read == 0) throw new EndOfStreamException();
            filled += read;
        }

        if (result.Length != count) Array.Resize(ref result, count);
        BigEndianArray.FromBigEndian<T>(MemoryMarshal.AsBytes(result.AsSpan()), result);
        return result;
    }

    public void EnterLevel()
    {
        if (++_depth > NbtLimits.MaxDepth) ThrowDepthExceeded();
    }

    public void ExitLevel()
    {
        _depth--;
    }

    private void EnsureAvailable(long byteCount)
    {
        if (!BaseStream.CanSeek) return;
        var remaining = BaseStream.Length - BaseStream.Position;
        if (byteCount > remaining)
            throw new NbtFormatException(
                $"NBT payload claims {byteCount} bytes but only {remaining} remain in the stream.");
    }

    private void FillExactly(Span<byte> destination)
    {
        var offset = 0;
        while (offset < destination.Length)
        {
            var read = BaseStream.Read(destination.Slice(offset));
            if (read == 0) throw new EndOfStreamException();
            offset += read;
        }
    }

    private new void FillBuffer(int numBytes)
    {
        FillExactly(_buffer.AsSpan(0, numBytes));
    }

    [DoesNotReturn]
    private static void ThrowDepthExceeded() =>
        throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {NbtLimits.MaxDepth}.");
}
