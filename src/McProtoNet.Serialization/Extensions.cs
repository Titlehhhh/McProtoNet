using System.Buffers;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadVarInt(this ReadOnlySpan<byte> data)
    {
        return ReadVarInt(data, out _);
    }
    /// <summary>
    /// Reads a VarInt from a byte span
    /// </summary>
    /// <param name="data">The span to read from</param>
    /// <param name="len">The number of bytes read</param>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="ArithmeticException">Thrown when VarInt is too long</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadVarInt(this ReadOnlySpan<byte> data, out int len)
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
            if (numRead > 5) throw new ArithmeticException("VarInt too long");
        } while ((read & 0b10000000) != 0);

        //data = data.Slice(numRead);


        len = numRead;
        return result;
    }

    /// <summary>
    /// Writes a VarInt to a buffer writer
    /// </summary>
    /// <param name="writer">The buffer writer to write to</param>
    /// <param name="value">The integer value to write as a VarInt</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteVarInt(this IBufferWriter<byte> writer, int value)
    {
        if (value == 0)
        {
            writer.GetSpan(1)[0] = 0;
            writer.Advance(1);
            return;
        }


        Span<byte> data = stackalloc byte[5];

        var len = value.GetVarIntLength(data);

        writer.Write(data.Slice(0, len));
    }

   

    /// <summary>
    /// Gets the length in bytes needed to encode an integer as a VarInt
    /// </summary>
    /// <param name="val">The integer value</param>
    /// <returns>The number of bytes needed</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte GetVarIntLength(this int val)
    {
        byte amount = 0;
        do
        {
            val >>= 7;
            amount++;
        } while (val != 0);

        return amount;
    }

    /// <summary>
    /// Writes a VarInt to a byte array and returns its length
    /// </summary>
    /// <param name="value">The integer value to encode</param>
    /// <param name="data">The byte array to write to</param>
    /// <returns>The number of bytes written</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte GetVarIntLength(this int value, byte[] data)
    {
        return GetVarIntLength(value, data, 0);
    }

    /// <summary>
    /// Writes a VarInt to a byte array at the specified offset and returns its length
    /// </summary>
    /// <param name="value">The integer value to encode</param>
    /// <param name="data">The byte array to write to</param>
    /// <param name="offset">The offset in the array to start writing</param>
    /// <returns>The number of bytes written</returns>
    /// <exception cref="ArithmeticException">Thrown when VarInt is too big</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte GetVarIntLength(this int value, byte[] data, int offset)
    {
        var unsigned = (uint)value;

        byte len = 0;
        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            data[offset + len++] = temp;
        } while (unsigned != 0);

        if (len > 5)
            throw new ArithmeticException("Var int is too big");
        return len;
    }

    /// <summary>
    /// Writes a VarInt to a byte span at the specified offset and returns its length
    /// </summary>
    /// <param name="value">The integer value to encode</param>
    /// <param name="data">The byte span to write to</param>
    /// <param name="offset">The offset in the span to start writing</param>
    /// <returns>The number of bytes written</returns>
    /// <exception cref="ArithmeticException">Thrown when VarInt is too big</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte GetVarIntLength(this int value, Span<byte> data, int offset)
    {
        var unsigned = (uint)value;

        byte len = 0;
        do
        {
            var temp = (byte)(unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
                temp |= 128;

            data[offset + len++] = temp;
        } while (unsigned != 0);

        if (len > 5)
            throw new ArithmeticException("Var int is too big");
        return len;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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
            if (numRead > 5) throw new InvalidOperationException("VarInt is too big");
        } while ((read & 0b10000000) != 0);

        return result;
    }

    /// <summary>
    /// Reads a VarInt from a stream asynchronously
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>The decoded VarInt value</returns>
    /// <exception cref="InvalidOperationException">Thrown when VarInt is too big</exception>
    public static async ValueTask<int> ReadVarIntAsync(this Stream stream, CancellationToken token = default)
    {
        var buff = ArrayPool<byte>.Shared.Rent(1);
       
        try
        {
            var numRead = 0;
            var result = 0;
            byte read;
            do
            {
                await stream.ReadExactlyAsync(buff,0,1, token);

                read = buff[0];
                var value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5) throw new InvalidOperationException("VarInt is too big");
            } while ((read & 0b10000000) != 0);

            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buff);
        }
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
        var buff = new byte[1];

        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            if (stream.Read(buff, 0, 1) <= 0)
                throw new EndOfStreamException();
            read = buff[0];


            var value = read & 0b01111111;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) throw new InvalidOperationException("VarInt is too big");
        } while ((read & 0b10000000) != 0);

        len = (byte)numRead;
        return result;
    }

    /// <summary>
    /// Writes a VarInt to a stream
    /// </summary>
    /// <param name="stream">The stream to write to</param>
    /// <param name="value">The integer value to write as a VarInt</param>
    public static void WriteVarInt(this Stream stream, int value)
    {
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
    public static ValueTask WriteVarIntAsync(this Stream stream, int value, CancellationToken token = default)
    {
        var unsigned = (uint)value;


        var data = ArrayPool<byte>.Shared.Rent(5);
        try
        {
            var len = 0;
            do
            {
                token.ThrowIfCancellationRequested();
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;
                data[len++] = temp;
            } while (unsigned != 0);

            return stream.WriteAsync(data.AsMemory(0, len), token);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(data);
        }
    }

    
}