using System;
using System.Buffers;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using McProtoNet.Transport.Compression;

namespace McProtoNet.Benchmark;

// Decompression side of the zlib comparison. The read path is the hot one on a live server
// (~57% of packets and ~93% of wire bytes pass through the inflater), so this measures
// libdeflate against the new .NET 11 ZLibDecoder on identical zlib streams.
//
// The x1000 variants do BatchSize decompressions per invocation, so BenchmarkDotNet's
// Allocated column reads directly as bytes per BatchSize packets.
[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ZlibInflateBench
{
    private const int BatchSize = 1000;

    private static readonly int VerifyIterations = 1000;

    private byte[] _plain;
    private byte[] _compressed;
    private int _compressedLength;
    private byte[] _output;

    [Params(ZlibPayload.Text, ZlibPayload.ChunkLike)]
    public ZlibPayload Payload { get; set; }

    [Params(8192)] public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _plain = ZlibBenchData.Make(Payload, DataLength, 12345);
        _output = new byte[DataLength];

        var compressor = LibDeflateCache.RentCompressor();
        _compressed = new byte[compressor.GetBound(DataLength)];
        _compressedLength = compressor.Compress(_plain, _compressed);

        // Every path must reproduce the plain payload before anything is measured, and the
        // reuse path must do it repeatedly: Reset() + Decompress has to produce a clean,
        // independent stream each time.
        CheckRoundTrip(LibDeflate());
        for (int i = 0; i < VerifyIterations; i++)
        {
            CheckRoundTrip(LibDeflate());
        }

#if NET11_0_OR_GREATER
        CheckRoundTrip(ZLibDecoder_ThreadStatic());
        CheckRoundTrip(ZLibDecoder_TryDecompress());
        for (int i = 0; i < VerifyIterations; i++)
        {
            CheckRoundTrip(ZLibDecoder_ThreadStatic());
        }

        Console.WriteLine($"// {Payload} {DataLength}: zlib {_compressedLength} B " +
            $"(x{(double)DataLength / _compressedLength:F2})");
#endif
    }

    private void CheckRoundTrip(int written)
    {
        if (written != _plain.Length)
            throw new InvalidOperationException($"decompressed {written} bytes, expected {_plain.Length}");
        if (!_output.AsSpan(0, written).SequenceEqual(_plain))
            throw new InvalidOperationException("zlib inflate mismatch");
    }

    [Benchmark(Baseline = true)]
    public int LibDeflate()
    {
        var decompressor = LibDeflateCache.RentDecompressor();
        return DecompressLibDeflate(decompressor, _compressed.AsSpan(0, _compressedLength), _output);
    }

    [Benchmark]
    public int LibDeflate_x1000()
    {
        var decompressor = LibDeflateCache.RentDecompressor();
        var source = _compressed.AsSpan(0, _compressedLength);
        int written = 0;
        for (int i = 0; i < BatchSize; i++)
        {
            written = DecompressLibDeflate(decompressor, source, _output);
        }

        return written;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DecompressLibDeflate(LibDeflateDecompressor decompressor, ReadOnlySpan<byte> source,
        Span<byte> output)
    {
        OperationStatus status = decompressor.Decompress(source, output, out int written);
        if (status != OperationStatus.Done)
            throw new InvalidOperationException($"libdeflate inflate failed: {status}");
        return written;
    }

#if NET11_0_OR_GREATER
    [Benchmark]
    public int ZLibDecoder_ThreadStatic()
    {
        ZLibDecoder decoder = ZLibDecoderCache.Rent();
        decoder.Reset();
        return DecompressZLibDecoder(decoder, _compressed.AsSpan(0, _compressedLength), _output);
    }

    [Benchmark]
    public int ZLibDecoder_ThreadStatic_x1000()
    {
        ZLibDecoder decoder = ZLibDecoderCache.Rent();
        decoder.Reset();
        var source = _compressed.AsSpan(0, _compressedLength);
        int written = 0;
        for (int i = 0; i < BatchSize; i++)
        {
            written = DecompressZLibDecoder(decoder, source, _output);
            if (i + 1 < BatchSize)
            {
                decoder.Reset();
            }
        }

        return written;
    }

    [Benchmark]
    public int ZLibDecoder_TryDecompress()
    {
        if (!ZLibDecoder.TryDecompress(_compressed.AsSpan(0, _compressedLength), _output, out int written))
            throw new InvalidOperationException("net11 ZLibDecoder TryDecompress failed");
        return written;
    }

    [Benchmark]
    public int ZLibDecoder_TryDecompress_x1000()
    {
        var source = _compressed.AsSpan(0, _compressedLength);
        int written = 0;
        for (int i = 0; i < BatchSize; i++)
        {
            if (!ZLibDecoder.TryDecompress(source, _output, out written))
                throw new InvalidOperationException("net11 ZLibDecoder TryDecompress failed");
        }

        return written;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DecompressZLibDecoder(ZLibDecoder decoder, ReadOnlySpan<byte> source, Span<byte> output)
    {
        OperationStatus status = decoder.Decompress(source, output, out _, out int written);
        if (status != OperationStatus.Done)
            throw new InvalidOperationException($"net11 ZLibDecoder inflate failed: {status}");
        return written;
    }
#endif
}

#if NET11_0_OR_GREATER
/// <summary>
/// Thread-local .NET 11 <see cref="ZLibDecoder"/> — the direct counterpart of
/// <see cref="McProtoNet.Transport.Compression.LibDeflateCache"/>.
/// </summary>
internal static class ZLibDecoderCache
{
    [ThreadStatic] private static ZLibDecoder? t_decoder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZLibDecoder Rent()
    {
        ZLibDecoder decoder = t_decoder;
        if (decoder is null)
        {
            decoder = new ZLibDecoder();
            t_decoder = decoder;
        }

        return decoder;
    }
}
#endif
