using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace McProtoNet.Benchmark.JitTest;

[MemoryDiagnoser]
[DisassemblyDiagnoser(exportHtml: true, printSource: true)]
public class ReadArrayGeneric
{
    private ArrayBufferWriter<byte> _writer = new();
    private int[] _intArray = new int[64];
    private long[] _longArray = new long[64];

    [Benchmark]
    public void Typeof_Int()
    {
        WriteBigEndianArray_Typeof<int>(_intArray);
    }

    [Benchmark]
    public void Typeof_Long()
    {
        WriteBigEndianArray_Typeof<long>(_longArray);
    }

    [Benchmark]
    public void Switch_Int()
    {
        WriteBigEndianArray_Switch<int>(_intArray);
    }

    [Benchmark]
    public void Switch_Long()
    {
        WriteBigEndianArray_Switch<long>(_longArray);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteBigEndianArray_Typeof<T>(ReadOnlySpan<T> val) where T : struct
    {
        int size = Unsafe.SizeOf<T>();
        int totalBytes = size * val.Length;
        var writer = _writer;

        if (typeof(T) == typeof(int))
        {
            var src = MemoryMarshal.Cast<T, int>(val);
            var dst = writer.GetSpan(totalBytes);
            var dstInts = MemoryMarshal.Cast<byte, int>(dst);
            for (int i = 0; i < src.Length; i++)
                dstInts[i] = BinaryPrimitives.ReverseEndianness(src[i]);
        }
        else if (typeof(T) == typeof(long))
        {
            var src = MemoryMarshal.Cast<T, long>(val);
            var dst = writer.GetSpan(totalBytes);
            var dstLongs = MemoryMarshal.Cast<byte, long>(dst);
            for (int i = 0; i < src.Length; i++)
                dstLongs[i] = BinaryPrimitives.ReverseEndianness(src[i]);
        }

        writer.Advance(totalBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteBigEndianArray_Switch<T>(ReadOnlySpan<T> val) where T : struct
    {
        int size = Unsafe.SizeOf<T>();
        int totalBytes = size * val.Length;
        var writer = _writer;

       

        writer.Advance(totalBytes);
    }
}

public class CustomClass
{
    public int A { get; }
}