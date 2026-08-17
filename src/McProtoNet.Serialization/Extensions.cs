using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace McProtoNet.Serialization;

/// <summary>
/// Extension methods for working with Minecraft protocol data types and networking
/// </summary>
public static class Extensions
{
    private static int SEGMENT_BITS = 0x7F;
    private static int CONTINUE_BIT = 0x80;

    /// <summary>
    /// Reads a VarInt from a span of bytes
    /// </summary>
    /// <param name="data">The span of bytes to read from</param>
    /// <returns>The read VarInt</returns>
    public static int ReadVarInt(this ReadOnlySpan<byte> data)
    {
        return ReadVarInt(data, out _);
    }

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
    /// Reads a VarInt from a byte span
    /// </summary>
    /// <param name="data">The span to read from</param>
    /// <param name="len">The number of bytes read</param>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="ArithmeticException">Thrown when VarInt is too long</exception>
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
    /// Writes a VarInt to a buffer writer
    /// </summary>
    /// <param name="writer">The buffer writer to write to</param>
    /// <param name="value">The integer value to write as a VarInt</param>
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
    /// Gets the length in bytes needed to encode an integer as a VarInt using optimized bit operations
    /// </summary>
    /// <param name="value">The integer value to measure</param>
    /// <returns>The number of bytes needed to encode the value as VarInt</returns>
    /// <remarks>
    /// This implementation uses mathematical optimization for maximum performance.
    /// It has no branches or loops, making it ideal for high-performance scenarios.
    /// </remarks>
    public static int GetVarIntLengthFast(this int value)
    {
        return (BitOperations.LeadingZeroCount((uint)value | 1) - 38) * -1171 >> 13;
    }

    /// <summary>
    /// Gets the length in bytes needed to encode an integer as a VarInt
    /// </summary>
    /// <param name="value">The integer value to measure</param>
    /// <returns>The number of bytes needed to encode the value as VarInt</returns>
    /// <remarks>
    /// <para>VarInt encoding uses 1-5 bytes where each byte contains 7 bits of data:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>1 byte</term>
    /// <description>Values from 0 to 127 (0x00 to 0x7F)</description>
    /// </item>
    /// <item>
    /// <term>2 bytes</term>
    /// <description>Values from 128 to 16,383 (0x80 to 0x3FFF)</description>
    /// </item>
    /// <item>
    /// <term>3 bytes</term>
    /// <description>Values from 16,384 to 2,097,151 (0x4000 to 0x1FFFFF)</description>
    /// </item>
    /// <item>
    /// <term>4 bytes</term>
    /// <description>Values from 2,097,152 to 268,435,455 (0x200000 to 0xFFFFFFF)</description>
    /// </item>
    /// <item>
    /// <term>5 bytes</term>
    /// <description>Values from 268,435,456 to 4,294,967,295 (0x10000000 to 0xFFFFFFFF)</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static int GetVarIntLength(this int value)
    {
        var val = (uint)value;

        if (val == 0) return 1;
        return GetVarIntLengthFast((int)val);
    }
    
    public static byte[] VarIntToArray(this int value)
    {
        Span<byte> span = stackalloc byte[5];
        var len = value.GetVarIntLength(span);
        return span.Slice(0, len).ToArray();
    }


    /// <summary>
    /// Writes a VarInt to a byte span and returns its length
    /// </summary>
    /// <param name="value">The integer value to encode</param>
    /// <param name="data">The byte span to write to</param>
    /// <returns>The number of bytes written</returns>
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
    /// Writes a VarInt to a memory region and returns its length
    /// </summary>
    /// <param name="value">The integer value to encode</param>
    /// <param name="data">The memory region to write to</param>
    /// <returns>The number of bytes written</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetVarIntLength(this int value, Memory<byte> data)
    {
        return GetVarIntLength(value, data.Span);
    }

    /// <summary>
    /// Reads a VarInt from a stream
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="EndOfStreamException">Thrown when the stream ends unexpectedly</exception>
    /// <exception cref="InvalidOperationException">Thrown when VarInt is too big</exception>
    public static int ReadVarInt(this Stream stream)
    {
        return stream.ReadVarInt(out _);
    }

    /// <summary>
    /// Reads a VarInt from a stream asynchronously
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="InvalidOperationException">Thrown when VarInt is too big</exception>
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
    /// Reads a VarInt from a stream and returns the number of bytes read
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="len">The number of bytes read</param>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="EndOfStreamException">Thrown when the stream ends unexpectedly</exception>
    /// <exception cref="InvalidOperationException">Thrown when VarInt is too big</exception>
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
    /// Writes a VarInt to a stream
    /// </summary>
    /// <param name="stream">The stream to write to</param>
    /// <param name="value">The integer value to write as a VarInt</param>
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
    /// Writes a VarInt to a stream asynchronously
    /// </summary>
    /// <param name="stream">The stream to write to</param>
    /// <param name="value">The integer value to write as a VarInt</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
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