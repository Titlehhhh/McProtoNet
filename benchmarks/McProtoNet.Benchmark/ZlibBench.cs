using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using McProtoNet.Transport.Compression;

namespace McProtoNet.Benchmark;

// One-shot compress comparison for the Minecraft packet path: each outgoing packet payload is
// compressed by a single call into a buffer. libdeflate is what the library ships with today
// (level 4 via LibDeflateCache); ZLibEncoder is the .NET 11 candidate behind #if NET11_0_OR_GREATER.
// Decompression is out of scope here.

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

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ZlibBench
{
    /// <summary>The compression level the library ships with (LibDeflateCache.RentCompressor).</summary>
    internal const int LibraryLevel = 4;

    private static readonly int VerifyIterations = 1000;

    private byte[] _plain;
    private byte[] _output;

    [Params(ZlibPayload.Text, ZlibPayload.ChunkLike)]
    public ZlibPayload Payload { get; set; }

    [Params(8192)] public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _plain = ZlibBenchData.Make(Payload, DataLength, 12345);
        int bound = Math.Max(LibDeflateCache.RentCompressor().GetBound(DataLength), DataLength + 1024);
#if NET11_0_OR_GREATER
        bound = Math.Max(bound, (int)ZLibEncoder.GetMaxCompressedLength(DataLength));
#endif
        _output = new byte[bound];

        int ld = LibDeflateCache.RentCompressor().Compress(_plain, _output);
        CheckRoundTrip(ld);

#if NET11_0_OR_GREATER
        // The reuse path in a loop: every Reset + Compress must produce a clean, independent
        // zlib stream that decodes back to the plain input.
        for (int i = 0; i < VerifyIterations; i++)
        {
            CheckRoundTrip(CompressWithCachedEncoder());
        }

        if (!ZLibEncoder.TryCompress(_plain, _output, out int nStatic, LibraryLevel))
            throw new InvalidOperationException("net11 ZLibEncoder TryCompress failed");
        CheckRoundTrip(nStatic);

        int n11 = CompressWithCachedEncoder();
        Console.WriteLine($"// {Payload} {DataLength} level {LibraryLevel}: " +
            $"libdeflate {ld} B (x{(double)DataLength / ld:F2}), " +
            $"net11 ZLibEncoder {n11} B (x{(double)DataLength / n11:F2})");
#else
        Console.WriteLine($"// {Payload} {DataLength} level {LibraryLevel}: libdeflate {ld} B (x{(double)DataLength / ld:F2})");
#endif
    }

    private void CheckRoundTrip(int compressedLength)
    {
        using var ms = new MemoryStream(_output, 0, compressedLength);
        using var zs = new ZLibStream(ms, CompressionMode.Decompress);
        var back = new byte[DataLength];
        zs.ReadExactly(back);
        if (!back.AsSpan().SequenceEqual(_plain)) throw new InvalidOperationException("zlib roundtrip mismatch");
    }

#if NET11_0_OR_GREATER
    private int CompressWithCachedEncoder()
    {
        ZLibEncoder encoder = ZlibEncoderCache.Rent();
        encoder.Reset();
        OperationStatus status = encoder.Compress(_plain, _output, out int consumed, out int written, isFinalBlock: true);
        if (status != OperationStatus.Done || consumed != _plain.Length)
            throw new InvalidOperationException("net11 ZLibEncoder compress failed");
        return written;
    }
#endif

    [Benchmark(Baseline = true)]
    public int LibDeflate() => LibDeflateCache.RentCompressor().Compress(_plain, _output);

    [Benchmark]
    public int LibDeflate_x1000()
    {
        var compressor = LibDeflateCache.RentCompressor();
        int written = 0;
        for (int i = 0; i < BatchSize; i++)
        {
            written = compressor.Compress(_plain, _output);
        }

        return written;
    }

#if NET11_0_OR_GREATER
    [Benchmark]
    public int ZLibEncoder_ThreadStatic()
    {
        ZLibEncoder encoder = ZlibEncoderCache.Rent();
        encoder.Reset();
        OperationStatus status = encoder.Compress(_plain, _output, out int consumed, out int written, isFinalBlock: true);
        if (status != OperationStatus.Done || consumed != _plain.Length)
            throw new InvalidOperationException("net11 ZLibEncoder compress failed");
        return written;
    }

    [Benchmark]
    public int ZLibEncoder_TryCompress()
    {
        if (!ZLibEncoder.TryCompress(_plain, _output, out int written, LibraryLevel))
            throw new InvalidOperationException("net11 ZLibEncoder TryCompress failed");
        return written;
    }

    // The x1000 variants (this one and the two below) do BatchSize compressions per invocation, so
    // BenchmarkDotNet's Allocated column reads directly as bytes per BatchSize compressions ("how
    // much does a thousand packets cost"), instead of bytes per single compression.
    [Benchmark]
    public int ZLibEncoder_ThreadStatic_x1000()
    {
        ZLibEncoder encoder = ZlibEncoderCache.Rent();
        int written = 0;
        for (int i = 0; i < BatchSize; i++)
        {
            encoder.Reset();
            OperationStatus status = encoder.Compress(_plain, _output, out int consumed, out written, isFinalBlock: true);
            if (status != OperationStatus.Done || consumed != _plain.Length)
                throw new InvalidOperationException("net11 ZLibEncoder compress failed");
        }

        return written;
    }

    [Benchmark]
    public int ZLibEncoder_TryCompress_x1000()
    {
        int written = 0;
        for (int i = 0; i < BatchSize; i++)
        {
            if (!ZLibEncoder.TryCompress(_plain, _output, out written, LibraryLevel))
                throw new InvalidOperationException("net11 ZLibEncoder TryCompress failed");
        }

        return written;
    }
#endif

    private const int BatchSize = 1000;
}

#if NET11_0_OR_GREATER
/// <summary>
/// Thread-local .NET 11 <see cref="ZLibEncoder"/> at the library compression level —
/// the direct counterpart of <see cref="McProtoNet.Transport.Compression.LibDeflateCache"/>.
/// </summary>
internal static class ZlibEncoderCache
{
    [ThreadStatic] private static ZLibEncoder? t_encoder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZLibEncoder Rent()
    {
        ZLibEncoder encoder = t_encoder;
        if (encoder is null)
        {
            encoder = new ZLibEncoder(ZlibBench.LibraryLevel);
            t_encoder = encoder;
        }

        return encoder;
    }
}
#endif