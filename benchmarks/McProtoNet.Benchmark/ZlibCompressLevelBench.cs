using System;
using System.IO;
using System.IO.Compression;
using BenchmarkDotNet.Attributes;
using McProtoNet.Net.Zlib;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace McProtoNet.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ZlibCompressLevelBench
{
    private byte[] _plain;
    private byte[] _output;
    private ZlibCompressorHeapAlloc _libdeflate;
    private RuntimeZlibDeflater _runtimeDeflater;

    [Params(ZlibPayload.Text, ZlibPayload.ChunkLike)]
    public ZlibPayload Payload { get; set; }

    [Params(8192)] public int DataLength { get; set; }

    [Params(1, 4, 6)] public int Level { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _plain = ZlibBenchData.Make(Payload, DataLength, 12345);
        _libdeflate = new ZlibCompressorHeapAlloc(Level);
        _runtimeDeflater = new RuntimeZlibDeflater(Level);
        _output = new byte[Math.Max(_libdeflate.GetBound(DataLength), DataLength + 1024)];

        int ld = _libdeflate.Compress(_plain, _output);
        int rt = _runtimeDeflater.Deflate(_plain, _output);
        Console.WriteLine($"// {Payload} {DataLength} level {Level}: libdeflate {ld} B (x{(double)DataLength / ld:F2}), runtime zlib {rt} B (x{(double)DataLength / rt:F2})");

        VerifyRoundTrip();
    }

    private void VerifyRoundTrip()
    {
        int n = _libdeflate.Compress(_plain, _output);
        using var ms = new MemoryStream(_output, 0, n);
        using var zs = new ZLibStream(ms, CompressionMode.Decompress);
        var back = new byte[DataLength];
        zs.ReadExactly(back);
        if (!back.AsSpan().SequenceEqual(_plain)) throw new InvalidOperationException("libdeflate roundtrip mismatch");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _runtimeDeflater.Dispose();
        _libdeflate.Dispose();
    }

    [Benchmark(Baseline = true)]
    public int LibDeflate() => _libdeflate.Compress(_plain, _output);

    [Benchmark]
    public int RuntimeZlib() => _runtimeDeflater.Deflate(_plain, _output);
}
