using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using McProtoNet.Transport.Compression;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace McProtoNet.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ZlibInflateCompareBench
{
    private byte[] _plain;
    private byte[] _compressed;
    private byte[] _output;
    private ReadOnlySequence<byte> _single;
    private ReadOnlySequence<byte> _multi;
    private RuntimeZlibInflater _runtimeInflater;

    [Params(ZlibPayload.Text, ZlibPayload.ChunkLike)]
    public ZlibPayload Payload { get; set; }

    [Params(8192)] public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _plain = ZlibBenchData.Make(Payload, DataLength, 12345);

        var ms = new MemoryStream();
        using (var z = new ZLibStream(ms, CompressionLevel.Optimal, true)) z.Write(_plain);
        _compressed = ms.ToArray();
        _output = new byte[DataLength];
        Console.WriteLine($"// {Payload} {DataLength} -> {_compressed.Length} bytes (ratio {(double)DataLength / _compressed.Length:F2})");

        _single = new ReadOnlySequence<byte>(_compressed);
        _multi = Segmentize(_compressed, 4096);
        Console.WriteLine($"// multi-segment input: {CountSegments(_multi)} segments");
        _runtimeInflater = new RuntimeZlibInflater();

        Verify();
    }

    private void Verify()
    {
        foreach (var seq in new[] { _single, _multi })
        {
            Array.Clear(_output);
            int n = _runtimeInflater.Inflate(seq, _output);
            if (n != DataLength || !_output.AsSpan().SequenceEqual(_plain))
                throw new InvalidOperationException("runtime zlib inflate mismatch");
        }

        Array.Clear(_output);
        var status = LibDeflateCache.RentDecompressor().Decompress(_compressed, _output, out int written);
        if (status != OperationStatus.Done || written != DataLength || !_output.AsSpan().SequenceEqual(_plain))
            throw new InvalidOperationException("libdeflate inflate mismatch");
    }

    [GlobalCleanup]
    public void Cleanup() => _runtimeInflater.Dispose();

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Single")]
    public int LibDeflate_Single()
    {
        LibDeflateCache.RentDecompressor().Decompress(_compressed, _output, out int written);
        return written;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Multi")]
    public int LibDeflate_MultiSegment_CopyThenInflate()
    {
        byte[] rented = ArrayPool<byte>.Shared.Rent((int)_multi.Length);
        try
        {
            _multi.CopyTo(rented);
            LibDeflateCache.RentDecompressor().Decompress(rented.AsSpan(0, (int)_multi.Length), _output, out int written);
            return written;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Single")]
    public int RuntimeZlib_Single() => _runtimeInflater.Inflate(_single, _output);

    [Benchmark]
    [BenchmarkCategory("Multi")]
    public int RuntimeZlib_MultiSegment_Streaming() => _runtimeInflater.Inflate(_multi, _output);

    [Benchmark]
    [BenchmarkCategory("Single")]
    public int ZLibStream_Single()
    {
        using var ms = new MemoryStream(_compressed);
        using var zs = new ZLibStream(ms, CompressionMode.Decompress);
        zs.ReadExactly(_output);
        if (zs.ReadByte() != -1) throw new InvalidOperationException("trailing bytes after zlib stream");
        return _output.Length;
    }

    private static int CountSegments(in ReadOnlySequence<byte> seq)
    {
        int n = 0;
        foreach (var _ in seq) n++;
        return n;
    }

    private static ReadOnlySequence<byte> Segmentize(byte[] data, int segmentSize)
    {
        int size = Math.Min(segmentSize, Math.Max(1, (data.Length + 1) / 2));
        Segment first = null, last = null;
        int pos = 0;
        while (pos < data.Length)
        {
            int len = Math.Min(data.Length - pos, size);
            var s = new Segment(data.AsMemory(pos, len));
            if (first is null) first = last = s;
            else last = last.Append(s);
            pos += len;
        }

        return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
    }

    private sealed class Segment : ReadOnlySequenceSegment<byte>
    {
        public Segment(ReadOnlyMemory<byte> memory) => Memory = memory;

        public Segment Append(Segment next)
        {
            next.RunningIndex = RunningIndex + Memory.Length;
            Next = next;
            return next;
        }
    }
}
