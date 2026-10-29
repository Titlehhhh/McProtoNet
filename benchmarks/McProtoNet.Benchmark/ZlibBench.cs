using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using McProtoNet.Transport.Compression;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace McProtoNet.Benchmark;

// The single zlib bench file: payload data generation, the raw runtime-zlib FFI wrappers,
// the .NET 11 ZLibEncoder thread-static cache, and the benchmark classes themselves.

public enum ZlibPayload
{
    Text,
    ChunkLike,
    Mixed
}

internal static class ZlibBenchData
{
    public static byte[] Make(ZlibPayload kind, int length, int seed)
    {
        var rng = new Random(seed);
        return kind switch
        {
            ZlibPayload.Text => MakeText(rng, length),
            ZlibPayload.ChunkLike => MakeChunkLike(rng, length),
            ZlibPayload.Mixed => MakeMixed(rng, length),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    private static byte[] MakeMixed(Random rng, int length)
    {
        var b = new byte[length];
        for (int i = 0; i < length; i++)
            b[i] = (byte)(i % 7 == 0 ? rng.Next(256) : 97 + i % 13);
        return b;
    }

    private static readonly string[] Names =
    {
        "minecraft:stone", "minecraft:dirt", "minecraft:grass_block", "minecraft:oak_log", "minecraft:water",
        "minecraft:deepslate", "minecraft:iron_ore", "minecraft:gravel", "minecraft:sand", "minecraft:oak_leaves"
    };

    private static byte[] MakeText(Random rng, int length)
    {
        var sb = new StringBuilder(length + 128);
        int id = 0;
        while (sb.Length < length)
        {
            sb.Append("{\"id\":").Append(id++)
                .Append(",\"type\":\"").Append(Names[rng.Next(Names.Length)])
                .Append("\",\"pos\":[").Append(rng.Next(-3000, 3000)).Append(',').Append(rng.Next(-64, 320))
                .Append(',').Append(rng.Next(-3000, 3000))
                .Append("],\"text\":{\"translate\":\"chat.type.text\",\"with\":[\"Player")
                .Append(rng.Next(100)).Append("\",\"hello world ").Append(rng.Next(1000)).Append("\"]}}\n");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        Array.Resize(ref bytes, length);
        return bytes;
    }

    private static byte[] MakeChunkLike(Random rng, int length)
    {
        var b = new byte[length];
        int pos = 0;
        while (pos < length)
        {
            pos = WriteSection(b, pos, rng);
        }

        return b;
    }

    private static int WriteSection(byte[] b, int pos, Random rng)
    {
        int end = b.Length;
        if (pos + 2 > end) return end;
        BinaryPrimitives.WriteInt16BigEndian(b.AsSpan(pos), (short)rng.Next(0, 4096));
        pos += 2;

        int paletteSize = rng.Next(2, 16);
        if (pos + 2 > end) return end;
        b[pos++] = 4;
        b[pos++] = (byte)paletteSize;
        for (int i = 0; i < paletteSize && pos + 3 <= end; i++)
        {
            pos = WriteVarInt(b, pos, rng.Next(1, 30000));
        }

        int longs = 4096 * 4 / 64;
        if (pos + 2 > end) return end;
        BinaryPrimitives.WriteInt16BigEndian(b.AsSpan(pos), (short)longs);
        pos += 2;

        int current = rng.Next(paletteSize);
        int run = 0;
        for (int l = 0; l < longs && pos + 8 <= end; l++)
        {
            ulong packed = 0;
            for (int k = 0; k < 16; k++)
            {
                if (run-- <= 0)
                {
                    if (rng.Next(100) < 85) current = rng.Next(2);
                    else current = rng.Next(paletteSize);
                    run = rng.Next(4, 200);
                }

                packed |= (ulong)(uint)current << (k * 4);
            }

            BinaryPrimitives.WriteUInt64BigEndian(b.AsSpan(pos), packed);
            pos += 8;
        }

        pos = WriteRuns(b, pos, end, 2048, 0xFF, rng);
        pos = WriteRuns(b, pos, end, 2048, 0x00, rng);
        return pos;
    }

    private static int WriteRuns(byte[] b, int pos, int end, int count, byte fill, Random rng)
    {
        int i = 0;
        while (i < count && pos < end)
        {
            int run = Math.Min(count - i, rng.Next(16, 512));
            byte value = rng.Next(100) < 85 ? fill : (byte)rng.Next(256);
            for (int k = 0; k < run && pos < end; k++, i++) b[pos++] = value;
        }

        return pos;
    }

    private static int WriteVarInt(byte[] b, int pos, int value)
    {
        uint v = (uint)value;
        while (v >= 0x80)
        {
            b[pos++] = (byte)(v | 0x80);
            v >>= 7;
        }

        b[pos++] = (byte)v;
        return pos;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct RuntimeZStream
{
    public byte* NextIn;
    public byte* NextOut;
    public nint Msg;
    public nint InternalState;
    public uint AvailIn;
    public uint AvailOut;
}

internal enum RuntimeZlibError
{
    Ok = 0,
    StreamEnd = 1,
    StreamError = -2,
    DataError = -3,
    MemError = -4,
    BufError = -5,
    VersionError = -6
}

internal static unsafe class RuntimeZlibNative
{
    private const string Lib = "System.IO.Compression.Native";

    [DllImport(Lib, EntryPoint = "CompressionNative_InflateInit2_")]
    public static extern RuntimeZlibError InflateInit2_(RuntimeZStream* s, int windowBits);

    [DllImport(Lib, EntryPoint = "CompressionNative_InflateReset2_")]
    public static extern RuntimeZlibError InflateReset2_(RuntimeZStream* s, int windowBits);

    [DllImport(Lib, EntryPoint = "CompressionNative_Inflate")]
    public static extern RuntimeZlibError Inflate(RuntimeZStream* s, int flush);

    [DllImport(Lib, EntryPoint = "CompressionNative_InflateEnd")]
    public static extern RuntimeZlibError InflateEnd(RuntimeZStream* s);

    [DllImport(Lib, EntryPoint = "CompressionNative_DeflateInit2_")]
    public static extern RuntimeZlibError DeflateInit2_(RuntimeZStream* s, int level, int method, int windowBits,
        int memLevel, int strategy);

    [DllImport(Lib, EntryPoint = "CompressionNative_Deflate")]
    public static extern RuntimeZlibError Deflate(RuntimeZStream* s, int flush);

    [DllImport(Lib, EntryPoint = "CompressionNative_DeflateEnd")]
    public static extern RuntimeZlibError DeflateEnd(RuntimeZStream* s);
}

internal sealed unsafe class RuntimeZlibInflater : IDisposable
{
    private const int WindowBits = 15;
    private const int NoFlush = 0;
    private const int Finish = 4;
    private static bool s_hasReset = true;

    private RuntimeZStream* _z;
    private bool _dirty;

    public RuntimeZlibInflater()
    {
        _z = (RuntimeZStream*)NativeMemory.AllocZeroed((nuint)sizeof(RuntimeZStream));
        Check(RuntimeZlibNative.InflateInit2_(_z, WindowBits));
    }

    public int Inflate(in ReadOnlySequence<byte> input, Span<byte> output)
    {
        if (_dirty) Reset();
        _dirty = true;
        fixed (byte* outPtr = output)
        {
            _z->NextOut = outPtr;
            _z->AvailOut = (uint)output.Length;
            bool ended = false;
            foreach (var segment in input)
            {
                if (ended)
                {
                    if (segment.Length > 0) ThrowTrailing();
                    continue;
                }

                fixed (byte* inPtr = segment.Span)
                {
                    _z->NextIn = inPtr;
                    _z->AvailIn = (uint)segment.Length;
                    while (_z->AvailIn > 0)
                    {
                        var r = RuntimeZlibNative.Inflate(_z, NoFlush);
                        if (r == RuntimeZlibError.StreamEnd)
                        {
                            if (_z->AvailIn != 0) ThrowTrailing();
                            ended = true;
                            break;
                        }

                        if (r == RuntimeZlibError.BufError && _z->AvailOut == 0)
                            throw new InvalidOperationException("output too small");
                        Check(r);
                    }

                    _z->NextIn = null;
                }
            }

            if (!ended)
            {
                var fin = RuntimeZlibNative.Inflate(_z, Finish);
                if (fin != RuntimeZlibError.StreamEnd) throw new InvalidOperationException("truncated: " + fin);
            }

            int written = output.Length - (int)_z->AvailOut;
            _z->NextOut = null;
            _z->AvailOut = 0;
            return written;
        }

        static void ThrowTrailing() => throw new InvalidOperationException("trailing bytes after zlib stream");
    }

    private void Reset()
    {
        if (s_hasReset)
        {
            try
            {
                Check(RuntimeZlibNative.InflateReset2_(_z, WindowBits));
                return;
            }
            catch (EntryPointNotFoundException)
            {
                s_hasReset = false;
            }
        }

        RuntimeZlibNative.InflateEnd(_z);
        NativeMemory.Clear(_z, (nuint)sizeof(RuntimeZStream));
        Check(RuntimeZlibNative.InflateInit2_(_z, WindowBits));
    }

    private static void Check(RuntimeZlibError e)
    {
        if (e != RuntimeZlibError.Ok) throw new InvalidOperationException(e.ToString());
    }

    public void Dispose()
    {
        if (_z == null) return;
        RuntimeZlibNative.InflateEnd(_z);
        NativeMemory.Free(_z);
        _z = null;
    }
}

internal sealed unsafe class RuntimeZlibDeflater : IDisposable
{
    private RuntimeZStream* _z;
    private readonly int _level;
    private bool _dirty;

    public RuntimeZlibDeflater(int level)
    {
        _level = level;
        _z = (RuntimeZStream*)NativeMemory.AllocZeroed((nuint)sizeof(RuntimeZStream));
        Init();
    }

    private void Init()
    {
        var r = RuntimeZlibNative.DeflateInit2_(_z, _level, 8, 15, 8, 0);
        if (r != RuntimeZlibError.Ok) throw new InvalidOperationException(r.ToString());
    }

    public int Deflate(ReadOnlySpan<byte> input, Span<byte> output)
    {
        if (_dirty)
        {
            RuntimeZlibNative.DeflateEnd(_z);
            NativeMemory.Clear(_z, (nuint)sizeof(RuntimeZStream));
            Init();
        }

        _dirty = true;
        fixed (byte* i = input)
        fixed (byte* o = output)
        {
            _z->NextIn = i;
            _z->AvailIn = (uint)input.Length;
            _z->NextOut = o;
            _z->AvailOut = (uint)output.Length;
            var r = RuntimeZlibNative.Deflate(_z, 4);
            if (r != RuntimeZlibError.StreamEnd) throw new InvalidOperationException("deflate: " + r);
            return output.Length - (int)_z->AvailOut;
        }
    }

    public void Dispose()
    {
        if (_z == null) return;
        RuntimeZlibNative.DeflateEnd(_z);
        NativeMemory.Free(_z);
        _z = null;
    }
}

/// <summary>
/// Thread-local .NET 11 <see cref="ZLibEncoder"/> instances —
/// mirrors <see cref="McProtoNet.Transport.Compression.LibDeflateCache"/>. The encoder is rebuilt when the
/// requested level differs from the cached one, so a single per-thread slot stays correct across
/// {@literal @}Params levels.
/// </summary>
internal static class RuntimeZlibEncoderCache
{
    [ThreadStatic] private static ZLibEncoder? t_encoder;

    [ThreadStatic] private static int t_level;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZLibEncoder Rent(int level)
    {
        ZLibEncoder encoder = t_encoder;
        if (encoder is null || t_level != level)
        {
            encoder = new ZLibEncoder(level);
            t_encoder = encoder;
            t_level = level;
        }

        return encoder;
    }
}

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
        _output = new byte[Math.Max(Math.Max(_libdeflate.GetBound(DataLength), (int)ZLibEncoder.GetMaxCompressedLength(DataLength)), DataLength + 1024)];

        int ld = _libdeflate.Compress(_plain, _output);
        int rt = _runtimeDeflater.Deflate(_plain, _output);
        int n11 = CompressNet11();
        Console.WriteLine($"// {Payload} {DataLength} level {Level}: " +
            $"libdeflate {ld} B (x{(double)DataLength / ld:F2}), " +
            $"runtime zlib {rt} B (x{(double)DataLength / rt:F2}), " +
            $"net11 ZLibEncoder {n11} B (x{(double)DataLength / n11:F2})");

        VerifyRoundTrip();
    }

    private static readonly int VerifyIterations = 1000;

    private void VerifyRoundTrip()
    {
        // libdeflate: single roundtrip — our library contract is covered elsewhere.
        int n = _libdeflate.Compress(_plain, _output);
        CheckRoundTrip(n);

        // net11 ZLibEncoder: exercise the reuse path in a loop — every Reset + Compress must
        // produce a clean, independent zlib stream that decodes back to the plain input.
        for (int i = 0; i < VerifyIterations; i++)
        {
            CheckRoundTrip(CompressNet11());
        }

        for (int i = 0; i < VerifyIterations; i++)
        {
            if (!ZLibEncoder.TryCompress(_plain, _output, out n, Level))
                throw new InvalidOperationException("net11 ZLibEncoder TryCompress failed");
            CheckRoundTrip(n);
        }
    }

    private void CheckRoundTrip(int compressedLength)
    {
        using var ms = new MemoryStream(_output, 0, compressedLength);
        using var zs = new ZLibStream(ms, CompressionMode.Decompress);
        var back = new byte[DataLength];
        zs.ReadExactly(back);
        if (!back.AsSpan().SequenceEqual(_plain)) throw new InvalidOperationException("zlib roundtrip mismatch");
    }

    private int CompressNet11()
    {
        ZLibEncoder encoder = RuntimeZlibEncoderCache.Rent(Level);
        encoder.Reset();
        OperationStatus status = encoder.Compress(_plain, _output, out int consumed, out int written, isFinalBlock: true);
        if (status != OperationStatus.Done || consumed != _plain.Length)
            throw new InvalidOperationException("net11 ZLibEncoder compress failed");
        return written;
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

    [Benchmark]
    public int ZLibEncoder_ThreadStatic()
    {
        ZLibEncoder encoder = RuntimeZlibEncoderCache.Rent(Level);
        encoder.Reset();
        OperationStatus status = encoder.Compress(_plain, _output, out int consumed, out int written, isFinalBlock: true);
        if (status != OperationStatus.Done || consumed != _plain.Length)
            throw new InvalidOperationException("net11 ZLibEncoder compress failed");
        return written;
    }

    [Benchmark]
    public int ZLibEncoder_TryCompress()
    {
        if (!ZLibEncoder.TryCompress(_plain, _output, out int written, Level))
            throw new InvalidOperationException("net11 ZLibEncoder TryCompress failed");
        return written;
    }
}

/// <summary>
/// Allocation cost of the .NET 11 <see cref="ZLibEncoder"/>: every instance carries both managed
/// state (GC heap) and a native zlib stream (process private memory). The memory diagnoser reports
/// the managed per-op cost; the GlobalSetup probe prints the managed/native split per encoder —
/// the ETW-based native profiler needs admin rights, so it is not used here.
/// </summary>
[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ZLibEncoderAllocBench
{
    private static readonly int Count = 100;

    [Params(1, 4, 6)] public int Level { get; set; }

    [GlobalSetup]
    public void ProbeAllocations()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long managedBefore = GC.GetTotalAllocatedBytes(precise: true);
        long nativeBefore = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;

        var encoders = new ZLibEncoder[Count];
        for (int i = 0; i < Count; i++)
        {
            encoders[i] = new ZLibEncoder(Level);
        }

        long managedAfter = GC.GetTotalAllocatedBytes(precise: true);
        long nativeAfter = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;

        long managed = managedAfter - managedBefore;
        long native = nativeAfter - nativeBefore;
        Console.WriteLine($"// net11 ZLibEncoder x{Count} level {Level}: " +
            $"managed {managed} B (~{managed / Count} B/encoder), " +
            $"private {native / 1024} KiB (~{native / Count / 1024} KiB/encoder)");

        for (int i = 0; i < Count; i++)
        {
            encoders[i].Dispose();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [Benchmark]
    public int ZLibEncoder_Alloc100_Dispose()
    {
        var encoders = new ZLibEncoder[Count];
        for (int i = 0; i < Count; i++)
        {
            encoders[i] = new ZLibEncoder(Level);
        }

        for (int i = 0; i < Count; i++)
        {
            encoders[i].Dispose();
        }

        return Count;
    }
}

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