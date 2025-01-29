using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNext;
using DotNext.Buffers;

namespace McProtoNet.Serialization;

public static class ReadArraysSIMDExtensions
{
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