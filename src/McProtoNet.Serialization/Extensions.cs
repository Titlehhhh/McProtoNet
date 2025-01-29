﻿using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        scoped SpanReader<byte> reader = new SpanReader<byte>(data);
        do
        {
            read = reader.Read();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(int) * length);
        ReadOnlySpan<int> ints = MemoryMarshal.Cast<byte, int>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            int[] result = new int[length];
            BinaryPrimitives.ReverseEndianness(ints, result);
            return result;
        }

        return ints.ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(long) * length);
        ReadOnlySpan<long> ints = MemoryMarshal.Cast<byte, long>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            long[] result = new long[length];
            BinaryPrimitives.ReverseEndianness(ints, result);
            return result;
        }

        return ints.ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(short) * length);
        ReadOnlySpan<short> ints = MemoryMarshal.Cast<byte, short>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            short[] result = new short[length];
            BinaryPrimitives.ReverseEndianness(ints, result);
            return result;
        }

        return ints.ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(ushort) * length);
        ReadOnlySpan<ushort> ints = MemoryMarshal.Cast<byte, ushort>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            ushort[] result = new ushort[length];
            BinaryPrimitives.ReverseEndianness(ints, result);
            return result;
        }

        return ints.ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(uint) * length);
        ReadOnlySpan<uint> ints = MemoryMarshal.Cast<byte, uint>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            uint[] result = new uint[length];
            BinaryPrimitives.ReverseEndianness(ints, result);
            return result;
        }

        return ints.ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(ulong) * length);
        ReadOnlySpan<ulong> ints = MemoryMarshal.Cast<byte, ulong>(bytes);
        if (BitConverter.IsLittleEndian)
        {
            ulong[] result = new ulong[length];
            BinaryPrimitives.ReverseEndianness(ints, result);
            return result;
        }

        return ints.ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(int) * length);
        if (BitConverter.IsLittleEndian)
        {
            ReadOnlySpan<int> ints = MemoryMarshal.Cast<byte, int>(bytes);
            float[] result = new float[length];
            BinaryPrimitives.ReverseEndianness(ints, MemoryMarshal.Cast<float, int>(result));
            return result;
        }

        return MemoryMarshal.Cast<byte, float>(bytes).ToArray();
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
        if (reader.RemainingCount < length)
        {
            throw new InsufficientMemoryException();
        }

        ReadOnlySpan<byte> bytes = reader.Read(sizeof(long) * length);
        if (BitConverter.IsLittleEndian)
        {
            ReadOnlySpan<long> ints = MemoryMarshal.Cast<byte, long>(bytes);
            double[] result = new double[length];
            BinaryPrimitives.ReverseEndianness(ints, MemoryMarshal.Cast<double, long>(result));
            return result;
        }

        return MemoryMarshal.Cast<byte, double>(bytes).ToArray();
    }
}