using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using DotNext.Buffers;
using DotNext.Collections.Generic;
using DotNext.IO;
using McProtoNet.Net.Zlib;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace McProtoNet.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ZlibBench
{
    private byte[] compressed;
    private byte[] outTest;

    private ReadOnlySequence<byte> sec;
    [Params(100, 500)] public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        outTest = new byte[DataLength];
        Random r = new Random(27);
        byte[] testData = new byte[DataLength];
        r.NextBytes(testData);

        MemoryStream ms = new MemoryStream();
        using (var zlib = new ZLibStream(ms, CompressionLevel.Optimal, true))
        {
            zlib.Write(testData);
        }

        compressed = ms.ToArray();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
    }

    [Benchmark]
    public void LibDeflateAlloc()
    {
        for (int i = 0; i < 500; i++)
        {
            scoped ZlibDecompressor decompressor = new ZlibDecompressor();
            decompressor.Decompress(compressed, outTest, out _);

            decompressor.Dispose();
        }
    }

    [Benchmark]
    public void LibDeflateThreadStatic()
    {
        var decompressor = LibDeflateCache.RentDecompressor();
        for (int i = 0; i < 500; i++)
        {
            decompressor.Decompress(compressed, outTest, out _);
        }
    }
}