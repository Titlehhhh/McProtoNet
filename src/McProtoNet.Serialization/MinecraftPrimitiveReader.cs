using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Unicode;
using DotNext;
using DotNext.Buffers;
using McProtoNet.NBT;
using DotNext.IO;

namespace McProtoNet.Serialization;

/// <summary>
/// Represents stack-allocated reader for primitive types of Minecraft
/// </summary>
[StructLayout(LayoutKind.Auto)]
public ref partial struct MinecraftPrimitiveReader
{
    private SpanReader<byte> _reader;
    private bool disposed;

    /// <summary>
    /// Gets the underlying span being read from
    /// </summary>
    public ReadOnlySpan<byte> Span => _reader.Span;

    /// <summary>
    /// Gets a reference to the current byte being read
    /// </summary>
    public ref byte Current => ref Unsafe.AsRef(in _reader.Current);

    /// <summary>
    /// Gets the number of bytes consumed from the reader
    /// </summary>
    public int ConsumedCount => _reader.ConsumedCount;

    /// <summary>
    /// Gets the number of remaining bytes to be read
    /// </summary>
    public readonly int RemainingCount => _reader.RemainingCount;

    /// <summary>
    /// Gets the span of bytes that have been consumed
    /// </summary>
    public readonly ReadOnlySpan<byte> ConsumedSpan => _reader.ConsumedSpan;

    /// <summary>
    /// Gets the span of remaining bytes to be read
    /// </summary>
    public readonly ReadOnlySpan<byte> RemainingSpan => _reader.RemainingSpan;

    /// <summary>
    /// Initializes a new instance of the MinecraftPrimitiveReader with a span of bytes
    /// </summary>
    /// <param name="data">The span of bytes to read from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MinecraftPrimitiveReader(ReadOnlySpan<byte> data)
    {
        _reader = new SpanReader<byte>(data);
    }

    /// <summary>
    /// Initializes a new instance of the MinecraftPrimitiveReader with a memory of bytes
    /// </summary>
    /// <param name="data">The memory of bytes to read from</param>
    public MinecraftPrimitiveReader(ReadOnlyMemory<byte> data) : this(data.Span)
    {
    }

    /// <summary>
    /// Initializes a new instance of the MinecraftPrimitiveReader with a sequence of bytes
    /// </summary>
    /// <param name="data">The sequence of bytes to read from</param>
    internal MinecraftPrimitiveReader(ReadOnlySequence<byte> data)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Advances the reader by the specified number of bytes
    /// </summary>
    /// <param name="count">The number of bytes to advance</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        _reader.Advance(count);
    }

    /// <summary>
    /// Reads a specified number of bytes from the reader
    /// </summary>
    /// <param name="count">The number of bytes to read</param>
    /// <returns>A span containing the read bytes</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> Read(int count) => _reader.Read(count);

    /// <summary>
    /// Reads bytes into the provided output span
    /// </summary>
    /// <param name="output">The span to read bytes into</param>
    /// <returns>The number of bytes read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(scoped Span<byte> output) => _reader.Read(output);

    /// <summary>
    /// Checks if the reader has been disposed
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown when the reader is disposed</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckDisposed()
    {
        if (disposed)
            throw new ObjectDisposedException(nameof(MinecraftPrimitiveWriter));
    }

    /// <summary>
    /// Reads a VarInt from the reader
    /// </summary>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="ArithmeticException">Thrown when the VarInt is too long</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadVarInt()
    {
        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            read = _reader.Read();
            var value = read & 127;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) throw new ArithmeticException("VarInt too long");
        } while ((read & 0b10000000) != 0);

        return result;
    }

    /// <summary>
    /// Reads a VarLong from the reader
    /// </summary>
    /// <returns>The decoded VarLong value</returns>
    /// <exception cref="ArithmeticException">Thrown when the VarLong is too long</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadVarLong()
    {
        var numRead = 0;
        long result = 0;
        byte read;
        do
        {
            read = _reader.Read();
            var value = read & 127;
            result |= (long)value << (7 * numRead);

            numRead++;
            if (numRead > 10) throw new ArithmeticException("VarLong too long");
        } while ((read & 0b10000000) != 0);

        return result;
    }

    /// <summary>
    /// Reads a boolean value from the reader
    /// </summary>
    /// <returns>The boolean value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        return _reader.Read() == 1;
    }

    /// <summary>
    /// Reads an unsigned byte from the reader
    /// </summary>
    /// <returns>The unsigned byte value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadUnsignedByte()
    {
        return _reader.Read();
    }

    /// <summary>
    /// Reads a signed byte from the reader
    /// </summary>
    /// <returns>The signed byte value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSignedByte()
    {
        return (sbyte)ReadUnsignedByte();
    }

    /// <summary>
    /// Reads an unsigned short in big-endian format from the reader
    /// </summary>
    /// <returns>The unsigned short value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUnsignedShort()
    {
        return _reader.ReadBigEndian<ushort>();
    }

    /// <summary>
    /// Reads a signed short in big-endian format from the reader
    /// </summary>
    /// <returns>The signed short value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadSignedShort()
    {
        return (short)ReadUnsignedShort();
    }

    /// <summary>
    /// Reads a signed integer in big-endian format from the reader
    /// </summary>
    /// <returns>The signed integer value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSignedInt()
    {
        return _reader.ReadBigEndian<int>();
    }

    /// <summary>
    /// Reads an unsigned integer in big-endian format from the reader
    /// </summary>
    /// <returns>The unsigned integer value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUnsignedInt()
    {
        return _reader.ReadBigEndian<uint>();
    }

    /// <summary>
    /// Reads a signed long in big-endian format from the reader
    /// </summary>
    /// <returns>The signed long value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadSignedLong()
    {
        return _reader.ReadBigEndian<long>();
    }

    /// <summary>
    /// Reads an unsigned long in big-endian format from the reader
    /// </summary>
    /// <returns>The unsigned long value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUnsignedLong()
    {
        return (ulong)ReadSignedLong();
    }

    /// <summary>
    /// Reads a float in big-endian format from the reader
    /// </summary>
    /// <returns>The float value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        var value = ReadSignedInt();
        return Unsafe.BitCast<int, float>(value);
    }

    /// <summary>
    /// Reads a double in big-endian format from the reader
    /// </summary>
    /// <returns>The double value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        var value = ReadSignedLong();
        return Unsafe.BitCast<long, double>(value);
    }

    /// <summary>
    /// Reads a UTF-8 encoded string from the reader
    /// </summary>
    /// <returns>The decoded string</returns>
    public string ReadString()
    {
        int len = ReadVarInt();

        if (len > 0)
        {
            return Encoding.UTF8.GetString(_reader.Read(len));
        }

        return "";
    }

    /// <summary>
    /// Reads a UUID from the reader
    /// </summary>
    /// <returns>The UUID value read</returns>
    public unsafe Guid ReadUUID()
    {
        long x = ReadSignedLong();
        long y = ReadSignedLong();

        long* ptr = stackalloc long[2];
        ptr[0] = x;
        ptr[1] = y;
        return *(Guid*)ptr;
    }

    /// <summary>
    /// Reads all remaining bytes from the reader
    /// </summary>
    /// <returns>An array containing the remaining bytes</returns>
    public byte[] ReadRestBuffer()
    {
        return _reader.ReadToEnd().ToArray();
    }

    /// <summary>
    /// Reads a specified number of bytes from the reader
    /// </summary>
    /// <param name="length">The number of bytes to read</param>
    /// <returns>An array containing the read bytes</returns>
    public byte[] ReadBuffer(int length)
    {
        return _reader.Read(length).ToArray();
    }

    /// <summary>
    /// Reads an optional NBT tag from the reader
    /// </summary>
    /// <param name="readRootTag">Whether to read the root tag</param>
    /// <returns>The NBT tag if present, null otherwise</returns>
    public NbtTag? ReadOptionalNbtTag(bool readRootTag)
    {
        if (ReadBoolean())
        {
            return ReadNbtTag(readRootTag);
        }

        return null;
    }

    /// <summary>
    /// Reads an NBT tag from the reader
    /// </summary>
    /// <param name="readRootTag">Whether to read the root tag</param>
    /// <returns>The NBT tag read</returns>
    public NbtTag? ReadNbtTag(bool readRootTag)
    {
        NbtSpanReader nbtSpanReader = new NbtSpanReader(_reader.RemainingSpan);
        NbtTag? result = nbtSpanReader.ReadAsTag<NbtTag>(readRootTag);

        _reader.Advance(nbtSpanReader.ConsumedCount);
        return result;
    }

    
}