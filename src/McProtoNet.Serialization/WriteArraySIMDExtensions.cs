using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace McProtoNet.Serialization;

using MM = MemoryMarshal;

public static class WriteArraySIMDExtensions
{
    extension(ref MinecraftPrimitiveWriter writer)
    {
        private Span<T> GetSpan<T>(int size) where T : unmanaged
        {
            return MM.Cast<byte, T>(writer.GetSpan(size));
        }

        public void WriteVarIntArray(ReadOnlySpan<int> source)
        {
            // TODO: optimize with GetSpan / Advance / SIMD
            foreach (int i in source)
            {
                writer.WriteVarInt(i);
            }
        }

        public void WriteVarLongArray(ReadOnlySpan<long> source)
        {
            // TODO: optimize with GetSpan / Advance / SIMD
            foreach (long l in source)
            {
                writer.WriteVarLong(l);
            }
        }

        public void WriteBigEndianArray<T>(ReadOnlySpan<T> val) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();
            int totalBytes = size * val.Length;

            if (typeof(T) == typeof(short))
            {
                var src = MM.Cast<T, short>(val);
                var dst = writer.GetSpan<short>(totalBytes);
                BinaryPrimitives.ReverseEndianness(src, dst);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var src = MM.Cast<T, ushort>(val);
                var dst = writer.GetSpan<ushort>(totalBytes);
                BinaryPrimitives.ReverseEndianness(src, dst);
            }
            else if (typeof(T) == typeof(int))
            {
                var src = MM.Cast<T, int>(val);
                var dst = writer.GetSpan<int>(totalBytes);
                BinaryPrimitives.ReverseEndianness(src, dst);
            }
            else if (typeof(T) == typeof(uint))
            {
                var src = MM.Cast<T, uint>(val);
                var dst = writer.GetSpan<uint>(totalBytes);
                BinaryPrimitives.ReverseEndianness(src, dst);
            }
            else if (typeof(T) == typeof(long))
            {
                var src = MM.Cast<T, long>(val);
                var dst = writer.GetSpan<long>(totalBytes);
                BinaryPrimitives.ReverseEndianness(src, dst);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var src = MM.Cast<T, ulong>(val);
                var dst = writer.GetSpan<ulong>(totalBytes);
                BinaryPrimitives.ReverseEndianness(src, dst);
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T)} is not supported for WriteBigEndianArray");
            }


            writer.Advance(totalBytes);
        }
    }
}