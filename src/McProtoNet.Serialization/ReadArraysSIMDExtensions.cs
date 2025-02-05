using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DotNext;
using DotNext.Buffers;

namespace McProtoNet.Serialization;

/// <summary>
/// Provides SIMD-optimized extensions for reading arrays of various data types
/// </summary>
public static class ReadArraysSIMDExtensions
{
    /// <summary>
    /// Reads a VarInt from a span of bytes
    /// </summary>
    /// <param name="data">The span of bytes to read from</param>
    /// <returns>The read VarInt</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadVarInt(this Span<byte> data)
    {
        var numRead = 0;
        var result = 0;
        byte read;

        do
        {
            read = data[numRead];
            var value = read & 127;
            result |= value << (7 * numRead);

            numRead++;
            if (numRead > 5) throw new ArithmeticException("VarInt too long");
        } while ((read & 0b10000000) != 0);

        return result;
    }

    /// <summary>
    /// Reads an array of 32-bit integers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of integers to read</param>
    /// <returns>An array of 32-bit integers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static int[] ReadArrayInt32BigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(int) * length);
        var ints = MemoryMarshal.Cast<byte, int>(bytes);
        if (!BitConverter.IsLittleEndian) return ints.ToArray();
        var result = new int[length];
        BinaryPrimitives.ReverseEndianness(ints, result);
        return result;
    }

    public static void ReadArrayInt32BigEndian(this ref MinecraftPrimitiveReader reader, scoped Span<int> destination)
    {
        var bytes = reader.Read(sizeof(int) * destination.Length);
        var ints = MemoryMarshal.Cast<byte, int>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(ints, destination);
        }
        else
        {
            ints.CopyTo(destination);
        }
    }


    /// <summary>
    /// Reads an array of 64-bit integers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of integers to read</param>
    /// <returns>An array of 64-bit integers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static long[] ReadArrayInt64BigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(long) * length);
        var ints = MemoryMarshal.Cast<byte, long>(bytes);
        if (!BitConverter.IsLittleEndian) return ints.ToArray();
        var result = new long[length];
        BinaryPrimitives.ReverseEndianness(ints, result);
        return result;
    }

    public static void ReadArrayInt64BigEndian(this ref MinecraftPrimitiveReader reader, scoped Span<long> destination)
    {
        var bytes = reader.Read(sizeof(long) * destination.Length);
        var ints = MemoryMarshal.Cast<byte, long>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(ints, destination);
        }
        else
        {
            ints.CopyTo(destination);
        }
    }

    /// <summary>
    /// Reads an array of 16-bit integers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of integers to read</param>
    /// <returns>An array of 16-bit integers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static short[] ReadArrayInt16BigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(short) * length);
        var ints = MemoryMarshal.Cast<byte, short>(bytes);
        if (!BitConverter.IsLittleEndian) return ints.ToArray();
        var result = new short[length];
        BinaryPrimitives.ReverseEndianness(ints, result);
        return result;
    }

    public static void ReadArrayInt16BigEndian(this ref MinecraftPrimitiveReader reader, scoped Span<short> destination)
    {
        var bytes = reader.Read(sizeof(short) * destination.Length);
        var ints = MemoryMarshal.Cast<byte, short>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(ints, destination);
        }
        else
        {
            ints.CopyTo(destination);
        }
    }

    /// <summary>
    /// Reads an array of unsigned 16-bit integers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of integers to read</param>
    /// <returns>An array of unsigned 16-bit integers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static ushort[] ReadArrayUnsignedInt16BigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(ushort) * length);
        var ints = MemoryMarshal.Cast<byte, ushort>(bytes);
        if (!BitConverter.IsLittleEndian) return ints.ToArray();
        ushort[] result = new ushort[length];
        BinaryPrimitives.ReverseEndianness(ints, result);
        return result;
    }

    public static void ReadArrayUnsignedInt16BigEndian(this ref MinecraftPrimitiveReader reader,
        scoped Span<ushort> destination)
    {
        var bytes = reader.Read(sizeof(ushort) * destination.Length);
        var ints = MemoryMarshal.Cast<byte, ushort>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(ints, destination);
        }
        else
        {
            ints.CopyTo(destination);
        }
    }

    /// <summary>
    /// Reads an array of unsigned 32-bit integers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of integers to read</param>
    /// <returns>An array of unsigned 32-bit integers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static uint[] ReadArrayUnsignedInt32BigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(uint) * length);
        var ints = MemoryMarshal.Cast<byte, uint>(bytes);
        if (!BitConverter.IsLittleEndian) return ints.ToArray();
        uint[] result = new uint[length];
        BinaryPrimitives.ReverseEndianness(ints, result);
        return result;
    }

    public static void ReadArrayUnsignedInt32BigEndian(this ref MinecraftPrimitiveReader reader,
        scoped Span<uint> destination)
    {
        var bytes = reader.Read(sizeof(uint) * destination.Length);
        var ints = MemoryMarshal.Cast<byte, uint>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(ints, destination);
        }
        else
        {
            ints.CopyTo(destination);
        }
    }

    /// <summary>
    /// Reads an array of unsigned 64-bit integers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of integers to read</param>
    /// <returns>An array of unsigned 64-bit integers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static ulong[] ReadArrayUnsignedInt64BigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(ulong) * length);
        var ints = MemoryMarshal.Cast<byte, ulong>(bytes);
        if (!BitConverter.IsLittleEndian) return ints.ToArray();
        var result = new ulong[length];
        BinaryPrimitives.ReverseEndianness(ints, result);
        return result;
    }

    public static void ReadArrayUnsignedInt64BigEndian(this ref MinecraftPrimitiveReader reader,
        scoped Span<ulong> destination)
    {
        var bytes = reader.Read(sizeof(ulong) * destination.Length);
        var ints = MemoryMarshal.Cast<byte, ulong>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(ints, destination);
        }
        else
        {
            ints.CopyTo(destination);
        }
    }

    /// <summary>
    /// Reads an array of single-precision floating-point numbers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of floats to read</param>
    /// <returns>An array of single-precision floating-point numbers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static float[] ReadArrayFloatBigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(int) * length);
        if (!BitConverter.IsLittleEndian) return MemoryMarshal.Cast<byte, float>(bytes).ToArray();
        var ints = MemoryMarshal.Cast<byte, int>(bytes);
        var result = new float[length];
        BinaryPrimitives.ReverseEndianness(ints, MemoryMarshal.Cast<float, int>(result));
        return result;
    }

    public static void ReadArrayFloatBigEndian(this ref MinecraftPrimitiveReader reader, scoped Span<float> destination)
    {
        var bytes = reader.Read(sizeof(int) * destination.Length);
        if (BitConverter.IsLittleEndian)
        {
            var ints = MemoryMarshal.Cast<byte, int>(bytes);
            Span<int> destinationInts = MemoryMarshal.Cast<float, int>(destination);
            BinaryPrimitives.ReverseEndianness(ints, destinationInts);
        }
        else
        {
            MemoryMarshal.Cast<byte, int>(bytes).CopyTo(MemoryMarshal.Cast<float, int>(destination));
        }
    }

    /// <summary>
    /// Reads an array of double-precision floating-point numbers in big-endian format
    /// </summary>
    /// <param name="reader">The primitive reader to read from</param>
    /// <param name="length">The number of doubles to read</param>
    /// <returns>An array of double-precision floating-point numbers</returns>
    /// <exception cref="InsufficientMemoryException">Thrown when there is not enough data to read</exception>
    public static double[] ReadArrayDoubleBigEndian(this ref MinecraftPrimitiveReader reader, int length)
    {
        var bytes = reader.Read(sizeof(long) * length);
        if (!BitConverter.IsLittleEndian) return MemoryMarshal.Cast<byte, double>(bytes).ToArray();
        var ints = MemoryMarshal.Cast<byte, long>(bytes);
        var result = new double[length];
        BinaryPrimitives.ReverseEndianness(ints, MemoryMarshal.Cast<double, long>(result));
        return result;
    }
    
    public static void ReadArrayDoubleBigEndian(this ref MinecraftPrimitiveReader reader, scoped Span<double> destination)
    {
        var bytes = reader.Read(sizeof(long) * destination.Length);
        if (BitConverter.IsLittleEndian)
        {
            var ints = MemoryMarshal.Cast<byte, long>(bytes);
            Span<long> destinationInts = MemoryMarshal.Cast<double, long>(destination);
            BinaryPrimitives.ReverseEndianness(ints, destinationInts);
        }
        else
        {
            MemoryMarshal.Cast<byte, long>(bytes)
                .CopyTo(MemoryMarshal.Cast<double, long>(destination));
        }
    }
}