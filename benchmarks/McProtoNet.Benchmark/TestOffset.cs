using System;
using BenchmarkDotNet.Attributes;
using DotNext;
using DotNext.Collections.Generic;

namespace McProtoNet.Benchmark;

public class TestOffset
{
    private readonly byte[] _buffer = new byte[4096];
    private readonly byte[] _bufferBackup = new byte[4096]; 
    private readonly byte[] _data = new byte[515];
    private byte[] destinationArray;

    private readonly Random _random = new(50);
    
    [Params(1, 10,64)] 
    public int Offset { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        destinationArray = new byte[_data.Length];
        _random.NextBytes(_data);
        _random.NextBytes(_buffer); 

        
        _data.AsSpan().CopyTo(_buffer.AsSpan(Offset));
        
       
        _buffer.AsSpan().CopyTo(_bufferBackup);
    }

    [IterationSetup]
    public void IterationSetup()
    {
       
        _bufferBackup.AsSpan().CopyTo(_buffer);
    }

    [Benchmark(Baseline = true)]
    public void CopyToSameArray()
    {
        Span<byte> src = _buffer.AsSpan(Offset, _data.Length);
        Span<byte> dsc = _buffer.AsSpan(0, _data.Length); 
        src.CopyTo(dsc);
    }

    [Benchmark]
    public void CopyToDifferentArray()
    {
        Span<byte> src = _buffer.AsSpan(Offset, _data.Length);
        Span<byte> dsc = destinationArray.AsSpan(0, _data.Length);
        src.CopyTo(dsc);
    }

    [Benchmark]
    public void CopyToDifferentArrayWithClear()
    {
     
        destinationArray.AsSpan().Clear();
        Span<byte> src = _buffer.AsSpan(Offset, _data.Length);
        Span<byte> dsc = destinationArray.AsSpan(0, _data.Length);
        src.CopyTo(dsc);
    }

    [Benchmark]
    public void CopyWithBufferBlockCopy()
    {
        Buffer.BlockCopy(_buffer, Offset, destinationArray, 0, _data.Length);
    }
}