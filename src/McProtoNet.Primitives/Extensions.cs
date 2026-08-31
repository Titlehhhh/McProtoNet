using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace McProtoNet.Primitives;

/// <summary>
/// Provides extension methods that read, write and measure Minecraft protocol VarInt values.
/// </summary>
public static class Extensions
{
    private static int SEGMENT_BITS = 0x7F;
    private static int CONTINUE_BIT = 0x80;

    /// <summary>
    /// Reads a VarInt from the start of a span.
    /// </summary>
    /// <param name="data">The span to read from. It must contain a complete VarInt.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="data"/> ends before the VarInt does.</exception>
    public static int ReadVarInt(this ReadOnlySpan<byte> data)
    {
        return ReadVarInt(data, out _);
    }

    /// <summary>
    /// Attempts to read a VarInt from the current position of a sequence reader.
    /// </summary>
    /// <param name="reader">The reader to read from. It is advanced past the VarInt on success and left
    /// where it was on failure.</param>
    /// <param name="res">When this method returns, contains the decoded value, or 0 if the read
    /// failed.</param>
    /// <param name="length">When this method returns, contains the number of bytes consumed, or -1 if the
    /// read failed.</param>
    /// <returns><see langword="true"/> if the value was read; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    public static bool TryReadVarInt(this ref SequenceReader<byte> reader, out int res, out int length)
    {
        var numRead = 0;
        var result = 0;
        byte read;

        do
        {
            if (reader.TryRead(out read))
            {
                var value = read & 127;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                    ThrowHelper.ThrowVarIntTooLong();
            }
            else
            {
                reader.Rewind(numRead);
                res = 0;
                length = -1;
                return false;
            }
        } while ((read & 0b10000000) != 0);


        res = result;
        length = numRead;
        return true;
    }


    /// <summary>
    /// Attempts to read a VarInt from the start of a sequence.
    /// </summary>
    /// <param name="sequence">The sequence to read from.</param>
    /// <param name="result">When this method returns, contains the decoded value, or 0 if the read
    /// failed.</param>
    /// <param name="length">When this method returns, contains the number of bytes the VarInt occupies, or
    /// 0 if the read failed.</param>
    /// <returns><see langword="true"/> if the value was read; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    /// <remarks>
    /// The sequence is not advanced.
    /// </remarks>
    public static bool TryReadVarInt(
        this in ReadOnlySequence<byte> sequence,
        out int result,
        out int length)
    {
        if (sequence.FirstSpan.Length >= 5 || sequence.IsSingleSegment)
        {
            return sequence.FirstSpan.TryReadVarInt(out result, out length);
        }

        return TryReadVarIntMultisegment(in sequence, out result, out length);
    }

    private static bool TryReadVarIntMultisegment(in ReadOnlySequence<byte> sequence,
        out int result,
        out int length)
    {
        var res1 = 0;
        var numRead = 0;

        var position = sequence.Start;
        while (sequence.TryGet(
                   ref position,
                   out var segment))
        {
            int i = 0;
            do
            {
                var read = segment.Span[i];

                var value = read & 127;
                res1 |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                    ThrowHelper.ThrowVarIntTooLong();

                i++;
                if ((read & 0b10000000) != 0) continue;

                length = numRead;
                result = res1;
                return true;
            } while (i < segment.Length);
        }

        result = 0;
        length = 0;
        return false;
    }

    /// <summary>
    /// Reads a VarInt from the start of a span and reports how many bytes it occupied.
    /// </summary>
    /// <param name="data">The span to read from. It must contain a complete VarInt.</param>
    /// <param name="len">When this method returns, contains the number of bytes read.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    /// <exception cref="IndexOutOfRangeException"><paramref name="data"/> ends before the VarInt does.</exception>
    public static int ReadVarInt(this in ReadOnlySpan<byte> data, out int len)
    {
        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            read = data[numRead];


            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) ThrowHelper.ThrowVarIntTooLong();
        } while ((read & 0b10000000) != 0);

        len = numRead;
        return result;
    }


    /// <summary>
    /// Attempts to read a VarInt from the start of a span.
    /// </summary>
    /// <param name="span">The span to read from.</param>
    /// <param name="result">When this method returns, contains the decoded value, or 0 if the read
    /// failed.</param>
    /// <param name="len">When this method returns, contains the number of bytes the VarInt occupies, or 0
    /// if the read failed.</param>
    /// <returns><see langword="true"/> if the value was read; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    public static bool TryReadVarInt(this in ReadOnlySpan<byte> span, out int result, out int len)
    {
        var numRead = 0;
        result = 0;
        byte read = 0;
        do
        {
            if (numRead >= span.Length)
            {
                len = 0;
                result = 0;
                return false;
            }

            read = span[numRead];
            var value = read & 0b01111111;
            result |= value << (7 * numRead);
            numRead++;
            if (numRead > 5)
                ThrowHelper.ThrowVarIntTooLong();
        } while ((read & 0b10000000) != 0);

        len = numRead;
        return true;
    }

    /// <summary>
    /// Writes a value as a VarInt to a buffer writer.
    /// </summary>
    /// <param name="writer">The buffer writer to write to.</param>
    /// <param name="value">The value to encode.</param>
    public static void WriteVarInt(this IBufferWriter<byte> writer, int value)
    {
        if (value == 0)
        {
            writer.GetSpan(1)[0] = 0;
            writer.Advance(1);
            return;
        }


        var span = writer.GetSpan(5);
        var len = value.GetVarIntLength(span);
        writer.Advance(len);
    }


    /// <summary>
    /// Returns the number of bytes needed to encode a value as a VarInt, without branches or loops.
    /// </summary>
    /// <param name="value">The value to measure.</param>
    /// <returns>The number of bytes the VarInt encoding of the value occupies.</returns>
    public static int GetVarIntLengthFast(this int value)
    {
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
    }

    /// <summary>
    /// Returns the number of bytes needed to encode a value as a VarInt.
    /// </summary>
    /// <param name="value">The value to measure.</param>
    /// <returns>The number of bytes the VarInt encoding of the value occupies, from 1 to 5.</returns>
    /// <remarks>
    /// <para>A VarInt carries 7 bits of the value per byte, so the length depends on the value read as
    /// unsigned:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>1 byte</term>
    /// <description>0 to 127.</description>
    /// </item>
    /// <item>
    /// <term>2 bytes</term>
    /// <description>128 to 16,383.</description>
    /// </item>
    /// <item>
    /// <term>3 bytes</term>
    /// <description>16,384 to 2,097,151.</description>
    /// </item>
    /// <item>
    /// <term>4 bytes</term>
    /// <description>2,097,152 to 268,435,455.</description>
    /// </item>
    /// <item>
    /// <term>5 bytes</term>
    /// <description>268,435,456 to 4,294,967,295, which covers every negative value.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static int GetVarIntLength(this int value)
    {
        var val = (uint)value;

        if (val == 0) return 1;
        return GetVarIntLengthFast((int)val);
    }
    
    /// <summary>
    /// Returns a new array that holds the VarInt encoding of a value.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>An array of 1 to 5 bytes that contains the VarInt encoding of the value.</returns>
    public static byte[] VarIntToArray(this int value)
    {
        Span<byte> span = stackalloc byte[5];
        var len = value.GetVarIntLength(span);
        return span.Slice(0, len).ToArray();
    }


    /// <summary>
    /// Writes a value as a VarInt into a span and returns the number of bytes written.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <param name="data">The span to write to. It must hold at least 5 bytes to accept every value.</param>
    /// <returns>The number of bytes written, from 1 to 5.</returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="data"/> is shorter than the encoding of
    /// <paramref name="value"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetVarIntLength(this int value, Span<byte> data)
    {
        var unsigned = (uint)value;

        byte len = 0;
        do
        {
            
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            data[len++] = temp;
        } while (unsigned != 0);

        return len;
    }

    /// <summary>
    /// Writes a value as a VarInt into a memory region and returns the number of bytes written.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <param name="data">The memory region to write to. It must hold at least 5 bytes to accept every
    /// value.</param>
    /// <returns>The number of bytes written, from 1 to 5.</returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="data"/> is shorter than the encoding of
    /// <paramref name="value"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetVarIntLength(this int value, Memory<byte> data)
    {
        return GetVarIntLength(value, data.Span);
    }

    /// <summary>
    /// Reads a VarInt from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="EndOfStreamException">The stream ended before the VarInt did.</exception>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    public static int ReadVarInt(this Stream stream)
    {
        return stream.ReadVarInt(out _);
    }

    /// <summary>
    /// Asynchronously reads a VarInt from a stream, using a buffer rented from the shared array pool.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the decoded
    /// value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="EndOfStreamException">The stream ended before the VarInt did.</exception>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    //[AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public static async ValueTask<int> ReadVarIntAsync(this Stream stream, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        var buff = ArrayPool<byte>.Shared.Rent(1);

        try
        {
            var numRead = 0;
            var result = 0;
            byte read;
            do
            {
                await stream.ReadExactlyAsync(buff, 0, 1, token)
                    .ConfigureAwait(false);


                read = buff[0];
                var value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5) ThrowHelper.ThrowVarIntTooLong();
            } while ((read & 0b10000000) != 0);

            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buff);
        }
    }

    /// <summary>
    /// Asynchronously reads a VarInt from a stream, using a caller-supplied buffer.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="buff">The buffer each byte of the VarInt is read into. Its length must be at least
    /// 1 byte, and the whole buffer is filled on every step of the read.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the decoded
    /// value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="buff"/> is empty.</exception>
    /// <exception cref="EndOfStreamException">The stream ended before the VarInt did.</exception>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static async ValueTask<int> ReadVarIntAsync(this Stream stream, Memory<byte> buff,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(buff);

        if (buff.Length == 0)
            throw new ArgumentException("Buffer must be at least 1 byte long", nameof(buff));

        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            await stream.ReadExactlyAsync(buff, token)
                .ConfigureAwait(false);


            read = buff.Span[0];
            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) ThrowHelper.ThrowVarIntTooLong();
        } while ((read & 0b10000000) != 0);

        return result;
    }

    /// <summary>
    /// Reads a VarInt from a stream and reports how many bytes it occupied.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="len">When this method returns, contains the number of bytes read.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="EndOfStreamException">The stream ended before the VarInt did.</exception>
    /// <exception cref="InvalidDataException">The VarInt is longer than 5 bytes.</exception>
    public static int ReadVarInt(this Stream stream, out int len)
    {
        ArgumentNullException.ThrowIfNull(stream);
        Span<byte> buff = stackalloc byte[1];

        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            if (stream.Read(buff) <= 0) throw new EndOfStreamException();

            read = buff[0];


            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) ThrowHelper.ThrowVarIntTooLong();
        } while ((read & 0b10000000) != 0);

        len = numRead;
        return result;
    }

    /// <summary>
    /// Writes a value as a VarInt to a stream, one byte at a time.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The value to encode.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public static void WriteVarInt(this Stream stream, int value)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var unsigned = (uint)value;
        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            stream.WriteByte(temp);
        } while (unsigned != 0);
    }

    /// <summary>
    /// Asynchronously writes a value as a VarInt to a stream, using a buffer rented from the shared array
    /// pool.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The value to encode.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static async ValueTask WriteVarIntAsync(this Stream stream, int value, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        var data = ArrayPool<byte>.Shared.Rent(5);
        try
        {
            await WriteVarIntAsync(stream, value, data, token).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(data);
        }
    }

    /// <summary>
    /// Asynchronously writes a value as a VarInt to a stream, using a caller-supplied buffer.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The value to encode.</param>
    /// <param name="buffer">The scratch buffer the encoding is built in. Its length must be at least
    /// 5 bytes.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="buffer"/> is shorter than 5 bytes.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static async ValueTask WriteVarIntAsync(this Stream stream, int value, Memory<byte> buffer,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (buffer.Length < 5)
        {
            throw new ArgumentException("Buffer must be at least 5 bytes long", nameof(buffer));
        }

        int len = value.GetVarIntLength(buffer.Span);

        await stream.WriteAsync(buffer[..len], token)
            .ConfigureAwait(false);
    }
}