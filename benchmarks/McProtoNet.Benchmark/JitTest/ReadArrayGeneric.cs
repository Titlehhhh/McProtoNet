using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace McProtoNet.Benchmark.JitTest;


[MemoryDiagnoser(true)]
[DisassemblyDiagnoser(exportHtml: true,printSource: true)]
public class ReadArrayGeneric
{
    private byte[] _array = new byte[64];
    
    [Benchmark()]
    public Guid[] Base1()
    {
        return ReadGuidArray();
    }
    
    [Benchmark()]
    public CustomClass[] Base2()
    {
        return ReadCustomClassArray();
    }
    
    [Benchmark()]
    public string[] Base3()
    {
        return ReadStringArray();
    }
    

    [Benchmark]
    public Guid[] If1()
    {
        return ReadArray<Guid>();
    }
    
    [Benchmark]
    public CustomClass[] If2()
    {
        return ReadArray<CustomClass>();
    }
    
    [Benchmark]
    public string[] If3()
    {
        return ReadArray<string>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T[] ReadArray<T>()
    {
        if (typeof(T) == typeof(int))
        {
            return (T[])(object)ReadIntArray();
        }

        if (typeof(T) == typeof(long))
        {
            return (T[])(object)ReadLongArray();
        }

        if (typeof(T) == typeof(string))
        {
            return (T[])(object)ReadStringArray();
        }

        if (typeof(T) == typeof(byte))
        {
            return (T[])(object)ReadByteArray();
        }

        if (typeof(T) == typeof(Guid))
        {
            return (T[])(object)ReadGuidArray();
        }

        if (typeof(T) == typeof(CustomClass))
        {
            return (T[])(object)ReadCustomClassArray();
        }

        throw new Exception();
    }

    private int[] _a = [];
    private long[] _b = [];
    private string[] _c = [];
    private byte[] _d = [];
    private Guid[] _e = [];
    private CustomClass[] _f = [];
    private int[] ReadIntArray()
    {
        return MemoryMarshal.Cast<byte, int>(_array).ToArray();
    }

    private long[] ReadLongArray()
    {
        return MemoryMarshal.Cast<byte, long>(_array).ToArray();
    }

    private string[] ReadStringArray()
    {
        return new string[5];
    }

    private byte[] ReadByteArray()
    {
        return _array.ToArray();
    }

    private Guid[] ReadGuidArray()
    {
        return MemoryMarshal.Cast<byte, Guid>(_array).ToArray();
    }

    private CustomClass[] ReadCustomClassArray()
    {
        return new CustomClass[1];
    }
    
}

public class CustomClass
{
    public int A { get; }
}