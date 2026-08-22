using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace McProtoNet.NBT;

internal static class BigEndianArray
{
    public static void FromBigEndian<T>(ReadOnlySpan<byte> source, Span<T> destination) where T : unmanaged
    {
        var elementSize = Unsafe.SizeOf<T>();
        if (source.Length != (long)destination.Length * elementSize)
            throw new ArgumentException(
                $"Big-endian source of {source.Length} bytes does not match {destination.Length} elements of {elementSize} bytes.",
                nameof(source));

        if (elementSize == 1 || !BitConverter.IsLittleEndian)
        {
            EnsureSupported<T>(elementSize);
            source.CopyTo(MemoryMarshal.AsBytes(destination));
            return;
        }

        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            BinaryPrimitives.ReverseEndianness(
                MemoryMarshal.Cast<byte, ushort>(source), MemoryMarshal.Cast<T, ushort>(destination));
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float))
            BinaryPrimitives.ReverseEndianness(
                MemoryMarshal.Cast<byte, uint>(source), MemoryMarshal.Cast<T, uint>(destination));
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double))
            BinaryPrimitives.ReverseEndianness(
                MemoryMarshal.Cast<byte, ulong>(source), MemoryMarshal.Cast<T, ulong>(destination));
        else
            throw Unsupported<T>();
    }

    public static void ToBigEndian<T>(ReadOnlySpan<T> source, Span<byte> destination) where T : unmanaged
    {
        var elementSize = Unsafe.SizeOf<T>();
        if (destination.Length != (long)source.Length * elementSize)
            throw new ArgumentException(
                $"Big-endian destination of {destination.Length} bytes does not match {source.Length} elements of {elementSize} bytes.",
                nameof(destination));

        if (elementSize == 1 || !BitConverter.IsLittleEndian)
        {
            EnsureSupported<T>(elementSize);
            MemoryMarshal.AsBytes(source).CopyTo(destination);
            return;
        }

        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            BinaryPrimitives.ReverseEndianness(
                MemoryMarshal.Cast<T, ushort>(source), MemoryMarshal.Cast<byte, ushort>(destination));
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float))
            BinaryPrimitives.ReverseEndianness(
                MemoryMarshal.Cast<T, uint>(source), MemoryMarshal.Cast<byte, uint>(destination));
        else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double))
            BinaryPrimitives.ReverseEndianness(
                MemoryMarshal.Cast<T, ulong>(source), MemoryMarshal.Cast<byte, ulong>(destination));
        else
            throw Unsupported<T>();
    }

    private static void EnsureSupported<T>(int elementSize)
    {
        if (elementSize == 1) return;
        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort)) return;
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(float)) return;
        if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(double)) return;
        throw Unsupported<T>();
    }

    private static NotSupportedException Unsupported<T>()
    {
        return new NotSupportedException($"{typeof(T).Name} is not a supported big-endian NBT element type.");
    }
}
