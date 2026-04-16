using System;
using System.Collections.Generic;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    extension(MinecraftPrimitiveWriter writer)
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(ReadOnlySpan<T> arr)
            => writer.WriteArray(arr, LengthFormat.VarInt);

        public void WriteArray<T>(ReadOnlySpan<T> arr, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat)
        {
            WriteLength(writer, lengthFormat, arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                WriteTypeWithoutProtocolVersion(writer, arr[i]);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void WriteArray<T>(ReadOnlySpan<T> arr, int protocolVersion)
            => writer.WriteArray(arr, LengthFormat.VarInt, protocolVersion);

        public void WriteArray<T>(ReadOnlySpan<T> arr, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat, int protocolVersion)
        {
            WriteLength(writer, lengthFormat, arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                writer.WriteType(arr[i], protocolVersion);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void WriteVarIntArray(ReadOnlySpan<int> arr)
            => writer.WriteVarIntArray(arr, LengthFormat.VarInt);

        public void WriteVarIntArray(ReadOnlySpan<int> arr, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat)
        {
            WriteLength(writer, lengthFormat, arr.Length);
            for (int i = 0; i < arr.Length; i++)
                writer.WriteVarInt(arr[i]);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void WriteVarLongArray(ReadOnlySpan<long> arr)
            => writer.WriteVarLongArray(arr, LengthFormat.VarInt);

        public void WriteVarLongArray(ReadOnlySpan<long> arr, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat lengthFormat)
        {
            WriteLength(writer, lengthFormat, arr.Length);
            foreach (var t in arr)
                writer.WriteVarLong(t);
        }

        public void WriteArray2d<T>(ReadOnlySpan<T[]> arr, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat outerFormat, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat innerFormat)
        {
            WriteLength(writer, outerFormat, arr.Length);
            foreach (var t in arr)
                writer.WriteArray<T>(t, innerFormat);
        }

        public void WriteArray2d<T>(ReadOnlySpan<T[]> arr, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat outerFormat, [System.Diagnostics.CodeAnalysis.ConstantExpected] LengthFormat innerFormat, int protocolVersion)
        {
            WriteLength(writer, outerFormat, arr.Length);
            foreach (var t in arr)
                writer.WriteArray<T>(t, innerFormat, protocolVersion);
        }
    }

    extension(ref MinecraftPrimitiveReader reader)
    {
        public T[] ReadArray<T>(LengthFormat lengthFormat)
        {
            int length = ReadLength(ref reader, lengthFormat);
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = ReadTypeWithoutProtocolVersion<T>(ref reader);
            }

            return result;
        }

        public T[] ReadArray<T>(LengthFormat lengthFormat, int protocolVersion)
        {
            int length = ReadLength(ref reader, lengthFormat);
            return reader.ReadArray<T>(length, protocolVersion);
        }

        public T[] ReadArray<T>(int length, int protocolVersion)
        {
            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = reader.ReadType<T>(protocolVersion);
            }

            return result;
        }

        public int[] ReadVarIntArray(LengthFormat lengthFormat)
        {
            int length = ReadLength(ref reader, lengthFormat);
            if (length == 0) return Array.Empty<int>();
            var result = new int[length];
            for (int i = 0; i < length; i++)
                result[i] = reader.ReadVarInt();
            return result;
        }

        public long[] ReadVarLongArray(LengthFormat lengthFormat)
        {
            int length = ReadLength(ref reader, lengthFormat);
            if (length == 0) return Array.Empty<long>();
            var result = new long[length];
            for (int i = 0; i < length; i++)
                result[i] = reader.ReadVarLong();
            return result;
        }

        public T[][] ReadArray2d<T>(LengthFormat outerFormat, LengthFormat innerFormat)
        {
            int outerLen = ReadLength(ref reader, outerFormat);
            if (outerLen == 0) return Array.Empty<T[]>();
            var result = new T[outerLen][];
            for (int i = 0; i < outerLen; i++)
                result[i] = reader.ReadArray<T>(innerFormat);
            return result;
        }

        public T[][] ReadArray2d<T>(LengthFormat outerFormat, LengthFormat innerFormat, int protocolVersion)
        {
            int outerLen = ReadLength(ref reader, outerFormat);
            if (outerLen == 0) return Array.Empty<T[]>();
            var result = new T[outerLen][];
            for (int i = 0; i < outerLen; i++)
                result[i] = reader.ReadArray<T>(innerFormat, protocolVersion);
            return result;
        }
    }
}
