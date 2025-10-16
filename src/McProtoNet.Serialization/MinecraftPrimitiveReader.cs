using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DotNext.Buffers;
using McProtoNet.NBT;

namespace McProtoNet.Serialization;

/// <summary>
/// Represents stack-allocated reader for primitive types of Minecraft
/// </summary>
[StructLayout(LayoutKind.Auto)]
public ref struct MinecraftPrimitiveReader
{
    private SequenceReader<byte> _reader;

    /// <summary>
    /// Gets the number of bytes consumed from the reader
    /// </summary>
    public long ConsumedCount => _reader.Consumed;

    /// <summary>
    /// Gets the number of remaining bytes to be read
    /// </summary>
    public readonly long RemainingCount => _reader.Remaining;

    

    

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowBufferOverflow() =>
        throw new BufferOverflowException();

    /// <summary>
    /// Initializes a new instance of the MinecraftPrimitiveReader with a memory of bytes
    /// </summary>
    /// <param name="data">The memory of bytes to read from</param>
    public MinecraftPrimitiveReader(ReadOnlyMemory<byte> data) : this(new ReadOnlySequence<byte>(data))
    {
        
    }

    

    /// <summary>
    /// Initializes a new instance of the MinecraftPrimitiveReader with a sequence of bytes
    /// </summary>
    /// <param name="data">The sequence of bytes to read from</param>
    public MinecraftPrimitiveReader(ReadOnlySequence<byte> data)
    {
        _reader = new SequenceReader<byte>(data);
       
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Rewind(int count)
    {
        _reader.Rewind(count);
    }


    public ReadOnlySequence<byte> Read(int count)
    {
        if (!_reader.TryReadExact(count, out var result))
        {
            ThrowBufferOverflow();
        }

        return result;
    }

    /// <summary>
    /// Reads bytes into the provided output span
    /// </summary>
    /// <param name="output">The span to read bytes into</param>
    /// <returns>The number of bytes read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(scoped Span<byte> output)
    {
        if (!_reader.TryCopyTo(output))
        {
            ThrowBufferOverflow();
        }

        return output.Length;
    }


    /// <summary>
    /// Reads a VarInt from the reader
    /// </summary>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="ArithmeticException">Thrown when the VarInt is too long</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadVarInt()
    {
        if (!_reader.TryReadVarInt(out int res, out _))
        {
            ThrowBufferOverflow();
        }

        return res;
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
            if (_reader.TryRead(out read))
            {
                var value = read & 127;
                result |= (long)value << (7 * numRead);
                numRead++;
                if (numRead > 10)
                {
                    _reader.Rewind(numRead);
                    throw new ArithmeticException("VarLong too long");
                }
            }
            else
            {
                _reader.Rewind(numRead);
                ThrowBufferOverflow();
            }
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
        if (!_reader.TryRead(out var b))
        {
            ThrowBufferOverflow();
        }

        return b == 1;
    }

    /// <summary>
    /// Reads an unsigned byte from the reader
    /// </summary>
    /// <returns>The unsigned byte value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadUnsignedByte()
    {
        if (!_reader.TryRead(out var b))
        {
            ThrowBufferOverflow();
        }

        return b;
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
        if (!_reader.TryReadBigEndian(out short v))
        {
            ThrowBufferOverflow();
        }

        return (ushort)v;
    }

    /// <summary>
    /// Reads a signed short in big-endian format from the reader
    /// </summary>
    /// <returns>The signed short value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadSignedShort()
    {
        if (!_reader.TryReadBigEndian(out short v))
        {
            ThrowBufferOverflow();
        }

        return v;
    }

    /// <summary>
    /// Reads a signed integer in big-endian format from the reader
    /// </summary>
    /// <returns>The signed integer value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSignedInt()
    {
        if (!_reader.TryReadBigEndian(out int v))
        {
            ThrowBufferOverflow();
        }

        return v;
    }

    /// <summary>
    /// Reads an unsigned integer in big-endian format from the reader
    /// </summary>
    /// <returns>The unsigned integer value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUnsignedInt()
    {
        return (uint)ReadSignedInt();
    }

    /// <summary>
    /// Reads a signed long in big-endian format from the reader
    /// </summary>
    /// <returns>The signed long value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadSignedLong()
    {
        if (!_reader.TryReadBigEndian(out long v))
        {
            ThrowBufferOverflow();
        }

        return v;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString(int maxLength = short.MaxValue)
    {
        return ReadString(Encoding.UTF8, maxLength);
    }


    public string ReadString(Encoding encoding, int maxLength = short.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(encoding);
        int len = ReadVarInt();

        if (len < 0)
        {
            ThrowHelper.ThrowInvalidData($"Negative string length: {len}");
        }

        if (len > maxLength * 3)
        {
            ThrowHelper.ThrowInvalidData($"String buffer too long ({len} bytes, max {maxLength * 3}).");
        }

        if (!_reader.TryReadExact(len, out var buff))
        {
            ThrowBufferOverflow();
        }

        string result = encoding.GetString(buff);
        if (result.Length > maxLength)
            ThrowHelper.ThrowInvalidData(
                $"Decoded string too long ({result.Length} chars, max {maxLength})");
        return result;
    }

   

    /// <summary>
    /// Reads a UUID from the reader
    /// </summary>
    /// <returns>The UUID value read</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Guid ReadUUID()
    {
        if (!_reader.TryReadExact(16, out var seq))
        {
            ThrowBufferOverflow();
        }

        if (seq.IsSingleSegment)
        {
            return new Guid(seq.FirstSpan, bigEndian: true);
        }

        Span<byte> bytes = stackalloc byte[16];
        seq.CopyTo(bytes);
        return new Guid(bytes, bigEndian: true);
    }

    /// <summary>
    /// Reads all remaining bytes from the reader
    /// </summary>
    /// <returns>An array containing the remaining bytes</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadRestBuffer()
    {
        var arr = _reader.UnreadSequence.ToArray();
        _reader.Advance(arr.Length);
        return arr;
    }

    /// <summary>
    /// Reads a specified number of bytes from the reader
    /// </summary>
    /// <param name="length">The number of bytes to read</param>
    /// <returns>An array containing the read bytes</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadBuffer(int length)
    {
        return Read(length).ToArray();
    }

    /// <summary>
    /// Reads an optional NBT tag from the reader
    /// </summary>
    /// <param name="readRootTag">Whether to read the root tag</param>
    /// <returns>The NBT tag if present, null otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NbtTag? ReadNbtTag(bool readRootTag)
    {
        throw new NotImplementedException();
        // NbtSpanReader nbtSpanReader = new NbtSpanReader(_reader.RemainingSpan);
        // NbtTag? result = nbtSpanReader.ReadAsTag<NbtTag>(readRootTag);
        //
        // _reader.Advance(nbtSpanReader.ConsumedCount);
        // return result;
    }
}