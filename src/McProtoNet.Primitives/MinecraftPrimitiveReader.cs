using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using McProtoNet.NBT;

namespace McProtoNet.Primitives;

/// <summary>
/// Provides a forward-only reader for the primitive types of the Minecraft protocol.
/// </summary>
/// <remarks>
/// The reader does not own the data it reads from and holds no resources.
/// </remarks>
[StructLayout(LayoutKind.Auto)]
public ref struct MinecraftPrimitiveReader
{
    private SequenceReader<byte> _reader;

    /// <summary>
    /// Gets the number of bytes consumed so far.
    /// </summary>
    public long ConsumedCount => _reader.Consumed;

    /// <summary>
    /// Gets the number of bytes left to read.
    /// </summary>
    public readonly long RemainingCount => _reader.Remaining;

    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftPrimitiveReader"/> structure that reads from
    /// the specified memory.
    /// </summary>
    /// <param name="data">The memory to read from. It is not copied and must stay valid for the lifetime of
    /// the reader.</param>
    public MinecraftPrimitiveReader(ReadOnlyMemory<byte> data) : this(new ReadOnlySequence<byte>(data))
    {

    }



    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftPrimitiveReader"/> structure that reads from
    /// the specified sequence.
    /// </summary>
    /// <param name="data">The sequence to read from. It is not copied and must stay valid for the lifetime
    /// of the reader.</param>
    public MinecraftPrimitiveReader(ReadOnlySequence<byte> data)
    {
        _reader = new SequenceReader<byte>(data);
       
    }

    /// <summary>
    /// Advances the reader by the specified number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to skip. This value must not be negative and must not
    /// exceed <see cref="RemainingCount"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative or greater than
    /// the number of bytes left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        _reader.Advance(count);
    }

    /// <summary>
    /// Moves the reader back by the specified number of bytes.
    /// </summary>
    /// <param name="count">The number of bytes to move back. This value must not be negative and must not
    /// exceed <see cref="ConsumedCount"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative or greater than
    /// the number of bytes consumed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Rewind(int count)
    {
        _reader.Rewind(count);
    }


    /// <summary>
    /// Reads the specified number of bytes as a sequence.
    /// </summary>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>A sequence over the bytes read. It is a window into the reader's own data and is not a
    /// copy.</returns>
    /// <exception cref="InvalidDataException">Fewer than <paramref name="count"/> bytes are left.</exception>
    public ReadOnlySequence<byte> Read(int count)
    {
        if (!_reader.TryReadExact(count, out var result))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return result;
    }

    /// <summary>
    /// Reads bytes into the specified span until it is full.
    /// </summary>
    /// <param name="output">The span to copy the bytes into. Its length decides how many bytes are
    /// read.</param>
    /// <returns>The number of bytes read, which is always the length of <paramref name="output"/>.</returns>
    /// <exception cref="InvalidDataException">Fewer bytes are left than <paramref name="output"/> can
    /// hold.</exception>
    /// <remarks>
    /// The reader is not advanced by this method.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Read(scoped Span<byte> output)
    {
        if (!_reader.TryCopyTo(output))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return output.Length;
    }


    /// <summary>
    /// Reads a VarInt.
    /// </summary>
    /// <returns>The decoded value.</returns>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes, or the data runs
    /// out.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadVarInt()
    {
        if (!_reader.TryReadVarInt(out int res, out _))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return res;
    }

    /// <summary>
    /// Reads a VarLong.
    /// </summary>
    /// <returns>The decoded value.</returns>
    /// <exception cref="InvalidDataException">The VarLong is longer than 10 bytes, or the data runs
    /// out.</exception>
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
                    ThrowHelper.ThrowInvalidData("VarLong is longer than 10 bytes");
                }
            }
            else
            {
                _reader.Rewind(numRead);
                ThrowHelper.ThrowNotEnoughData();
            }
        } while ((read & 0b10000000) != 0);

        return result;
    }

    /// <summary>
    /// Reads a single byte as a Boolean value.
    /// </summary>
    /// <returns><see langword="true"/> if the byte read is 1; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidDataException">No byte is left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        if (!_reader.TryRead(out var b))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return b == 1;
    }

    /// <summary>
    /// Reads an unsigned byte.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">No byte is left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadUnsignedByte()
    {
        if (!_reader.TryRead(out var b))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return b;
    }

    /// <summary>
    /// Reads a signed byte.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">No byte is left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSignedByte()
    {
        return (sbyte)ReadUnsignedByte();
    }

    /// <summary>
    /// Reads a big-endian unsigned 16-bit integer.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 2 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUnsignedShort()
    {
        if (!_reader.TryReadBigEndian(out short v))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return (ushort)v;
    }

    /// <summary>
    /// Reads a big-endian signed 16-bit integer.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 2 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadSignedShort()
    {
        if (!_reader.TryReadBigEndian(out short v))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return v;
    }

    /// <summary>
    /// Reads a big-endian signed 32-bit integer.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 4 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSignedInt()
    {
        if (!_reader.TryReadBigEndian(out int v))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return v;
    }

    /// <summary>
    /// Reads a big-endian unsigned 32-bit integer.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 4 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUnsignedInt()
    {
        return (uint)ReadSignedInt();
    }

    /// <summary>
    /// Reads a big-endian signed 64-bit integer.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 8 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadSignedLong()
    {
        if (!_reader.TryReadBigEndian(out long v))
        {
            ThrowHelper.ThrowNotEnoughData();
        }

        return v;
    }

    /// <summary>
    /// Reads a big-endian unsigned 64-bit integer.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 8 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUnsignedLong()
    {
        return (ulong)ReadSignedLong();
    }

    /// <summary>
    /// Reads a big-endian single-precision floating-point number.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 4 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        var value = ReadSignedInt();
        return Unsafe.BitCast<int, float>(value);
    }

    /// <summary>
    /// Reads a big-endian double-precision floating-point number.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 8 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        var value = ReadSignedLong();
        return Unsafe.BitCast<long, double>(value);
    }

    /// <summary>
    /// Reads a length-prefixed string using UTF-8.
    /// </summary>
    /// <param name="maxLength">The maximum number of characters the decoded string may contain. The default
    /// is <see cref="short.MaxValue"/>.</param>
    /// <returns>The decoded string.</returns>
    /// <exception cref="InvalidDataException">
    /// The length prefix is negative.
    /// -or-
    /// The byte count exceeds three times <paramref name="maxLength"/>.
    /// -or-
    /// The decoded string is longer than <paramref name="maxLength"/>.
    /// -or-
    /// The data runs out.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString(int maxLength = short.MaxValue)
    {
        return ReadString(Encoding.UTF8, maxLength);
    }


    /// <summary>
    /// Reads a length-prefixed string using the specified encoding.
    /// </summary>
    /// <param name="encoding">The encoding to decode the bytes with.</param>
    /// <param name="maxLength">The maximum number of characters the decoded string may contain. The default
    /// is <see cref="short.MaxValue"/>.</param>
    /// <returns>The decoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidDataException">
    /// The length prefix is negative.
    /// -or-
    /// The byte count exceeds three times <paramref name="maxLength"/>.
    /// -or-
    /// The decoded string is longer than <paramref name="maxLength"/>.
    /// -or-
    /// The data runs out.
    /// </exception>
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
            ThrowHelper.ThrowNotEnoughData();
        }

        string result = encoding.GetString(buff);
        if (result.Length > maxLength)
            ThrowHelper.ThrowInvalidData(
                $"Decoded string too long ({result.Length} chars, max {maxLength})");
        return result;
    }

   

    /// <summary>
    /// Reads a 16-byte big-endian UUID.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="InvalidDataException">Fewer than 16 bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Guid ReadUUID()
    {
        if (!_reader.TryReadExact(16, out var seq))
        {
            ThrowHelper.ThrowNotEnoughData();
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
    /// Reads every byte left and copies it into a new array.
    /// </summary>
    /// <returns>A new array that contains the remaining bytes.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadRestBuffer()
    {
        var arr = _reader.UnreadSequence.ToArray();
        _reader.Advance(arr.Length);
        return arr;
    }

    /// <summary>
    /// Reads the specified number of bytes and copies them into a new array.
    /// </summary>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>A new array of <paramref name="length"/> bytes.</returns>
    /// <exception cref="InvalidDataException">Fewer than <paramref name="length"/> bytes are left.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadBuffer(int length)
    {
        return Read(length).ToArray();
    }

    /// <summary>
    /// Reads a presence flag and, when it is set, the NBT tag that follows.
    /// </summary>
    /// <param name="readRootTag"><see langword="true"/> if the root tag carries a name, which is the
    /// pre-network NBT format; <see langword="false"/> for the nameless network root.</param>
    /// <returns>The tag read, or <see langword="null"/> if the presence flag was not set.</returns>
    /// <exception cref="InvalidDataException">The data runs out.</exception>
    /// <exception cref="NbtFormatException">The tag is not valid NBT.</exception>
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
    /// Reads an NBT tag without copying the underlying buffer.
    /// </summary>
    /// <param name="readRootTag"><see langword="true"/> if the root tag carries a name, which is the
    /// pre-network NBT format; <see langword="false"/> for the nameless network root.</param>
    /// <returns>The tag read, or <see langword="null"/> if the first byte is TAG_End.</returns>
    /// <exception cref="NbtFormatException">
    /// The data is malformed.
    /// -or-
    /// The data is truncated.
    /// -or-
    /// The tags are nested too deeply.
    /// </exception>
    public NbtTag? ReadNbtTag(bool readRootTag)
    {
        var unread = _reader.UnreadSequence;
        if (unread.IsSingleSegment)
        {
            // Fast path: parse straight from the contiguous buffer.
            var spanReader = new NbtSpanReader(unread.FirstSpan);
            NbtTag? result = spanReader.ReadAsTag<NbtTag>(readRootTag);
            _reader.Advance(spanReader.ConsumedCount);
            return result;
        }

        // Multi-segment path: parse straight from the sequence.
        return NbtSequenceReader.ReadTag(ref _reader, readRootTag);
    }
}