using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Transport.Compression;

namespace McProtoNet.Benchmark;

/// <summary>
/// Memory + throughput probe for the two zlib strategies, run outside BenchmarkDotNet because the
/// question is about hundreds of live instances, not about a single call.
///
/// Phase 1 measures the private bytes a single instance costs (native state included), for the
/// compressor and the decompressor of each implementation.
/// Phase 2 simulates <c>connections</c> live sessions that decode for <c>seconds</c> seconds, with
/// the decoder either owned by the connection or rented from the per-thread cache, and reports the
/// throughput and the resident size of the whole process.
///
/// Run: <c>dotnet run -c Release --project benchmarks/McProtoNet.Benchmark -f net11.0 -- memory-bench 500 8</c>
/// </summary>
internal static class MemoryBench
{
    private const int PayloadSize = 8192;
    private const int CompressorLevel = 4;

    public static int Run(string[] args)
    {
        int connections = args.Length > 0 && int.TryParse(args[0], out int c) ? c : 500;
        int seconds = args.Length > 1 && int.TryParse(args[1], out int s) ? s : 8;

        byte[] plain = ZlibBenchData.Make(ZlibPayload.Text, PayloadSize, 12345);
        byte[] compressed = CompressWithLibDeflate(plain);

        Console.WriteLine($"# connections={connections} seconds={seconds} payload={PayloadSize} B " +
            $"zlib={compressed.Length} B level={CompressorLevel}");
        Console.WriteLine();

        Console.WriteLine("## Per-instance private bytes");
        Measure("libdeflate decompressor", connections, () => new LibDeflateDecompressor());
        Measure("zlib decoder (ZLibDecoder)", connections, () => new ZLibDecoder());
        Measure("libdeflate compressor lvl4", connections, () => new LibDeflateCompressor(CompressorLevel));
        Measure("zlib encoder lvl4 (ZLibEncoder)", connections, () => new ZLibEncoder(CompressorLevel));
        Console.WriteLine();

        Console.WriteLine("## 500-connection decode workload");
        RunLibDeflateWorkload(connections, seconds, compressed, plain.Length, Strategy.PerConnection);
        RunLibDeflateWorkload(connections, seconds, compressed, plain.Length, Strategy.ThreadStatic);
        RunLibDeflateWorkload(connections, seconds, compressed, plain.Length, Strategy.PerCall);
        RunZLibWorkload(connections, seconds, compressed, plain.Length, Strategy.PerConnection);
        RunZLibWorkload(connections, seconds, compressed, plain.Length, Strategy.ThreadStatic);
        RunZLibWorkload(connections, seconds, compressed, plain.Length, Strategy.PerCall);

        return 0;
    }

    private enum Strategy
    {
        ThreadStatic,
        PerConnection,
        PerCall,
    }

    private static string Describe(string codec, Strategy strategy) => strategy switch
    {
        Strategy.ThreadStatic => codec + " thread-static",
        Strategy.PerConnection => codec + " per-connection",
        _ => codec + " per-call",
    };

    private static byte[] CompressWithLibDeflate(byte[] plain)
    {
        var compressor = LibDeflateCache.RentCompressor();
        var compressed = new byte[compressor.GetBound(plain.Length)];
        int written = compressor.Compress(plain, compressed);
        return compressed.AsSpan(0, written).ToArray();
    }

    private static void Measure(string name, int count, Func<IDisposable> factory)
    {
        Collect();
        long before = PrivateBytes();

        var instances = new List<IDisposable>(count);
        for (int i = 0; i < count; i++)
        {
            instances.Add(factory());
        }

        Collect();
        long after = PrivateBytes();

        long perInstance = (after - before) / count;

        foreach (IDisposable instance in instances)
        {
            instance.Dispose();
        }

        instances.Clear();

        Collect();
        long released = PrivateBytes();

        Console.WriteLine($"| {name,-32} | {perInstance,10:N0} B | {perInstance * 500d / (1024 * 1024),8:F1} MiB @500 | " +
            $"kept after dispose: {Math.Max(0, released - before) / 1024d,6:F0} KiB |");
    }

    private static void RunLibDeflateWorkload(int connections, int seconds, byte[] compressed, int plainLength,
        Strategy strategy)
    {
        // The per-connection pool exists only to be measured: it is what an implementation that
        // owns one native handle per session would hold.
        var owned = strategy == Strategy.PerConnection ? new LibDeflateDecompressor[connections] : null;
        if (owned is not null)
        {
            for (int i = 0; i < connections; i++)
            {
                owned[i] = new LibDeflateDecompressor();
            }
        }

        Collect();
        long baseline = PrivateBytes();
        var counter = new OperationCounter();

        var deadline = Stopwatch.GetTimestamp() + (long)(seconds * Stopwatch.Frequency);
        var threads = new Task[connections];

        for (int i = 0; i < connections; i++)
        {
            int index = i;
            threads[i] = Task.Factory.StartNew(() =>
            {
                var shared = strategy == Strategy.ThreadStatic ? LibDeflateCache.RentDecompressor() : null;
                var source = compressed.AsSpan();
                var output = new byte[plainLength];

                while (Stopwatch.GetTimestamp() < deadline)
                {
                    for (int n = 0; n < 64; n++)
                    {
                        var status = strategy == Strategy.PerCall
                            ? DecompressOnce(source, output)
                            : (shared ?? owned![index]).Decompress(source, output, out _);
                        if (status != System.Buffers.OperationStatus.Done)
                            throw new InvalidOperationException($"libdeflate inflate failed: {status} ({strategy})");
                    }

                    counter.Add(64);
                }
            }, TaskCreationOptions.LongRunning);
        }

        Task.WaitAll(threads);
        Report(Describe("libdeflate", strategy), counter, baseline);

        if (owned is not null)
        {
            foreach (LibDeflateDecompressor decompressor in owned)
            {
                decompressor.Dispose();
            }
        }
    }

    private static void RunZLibWorkload(int connections, int seconds, byte[] compressed, int plainLength,
        Strategy strategy)
    {
        ZLibDecoder[]? owned = strategy == Strategy.PerConnection ? new ZLibDecoder[connections] : null;
        if (owned is not null)
        {
            for (int i = 0; i < connections; i++)
            {
                owned[i] = new ZLibDecoder();
            }
        }

        Collect();
        long baseline = PrivateBytes();
        var counter = new OperationCounter();

        var deadline = Stopwatch.GetTimestamp() + (long)(seconds * Stopwatch.Frequency);
        var threads = new Task[connections];

        for (int i = 0; i < connections; i++)
        {
            int index = i;
            threads[i] = Task.Factory.StartNew(() =>
            {
                ZLibDecoder? shared = strategy == Strategy.ThreadStatic ? ZLibDecoderCache.Rent() : null;
                var source = compressed.AsSpan();
                var output = new byte[plainLength];
                bool first = true;

                while (Stopwatch.GetTimestamp() < deadline)
                {
                    for (int n = 0; n < 64; n++)
                    {
                        bool ok;
                        if (strategy == Strategy.PerCall)
                        {
                            ok = ZLibDecoder.TryDecompress(source, output, out _);
                        }
                        else
                        {
                            ZLibDecoder decoder = shared ?? owned![index];
                            if (!first)
                            {
                                decoder.Reset();
                            }

                            ok = decoder.Decompress(source, output, out _, out _) ==
                                System.Buffers.OperationStatus.Done;
                        }

                        first = false;

                        if (!ok)
                            throw new InvalidOperationException($"ZLibDecoder inflate failed ({strategy})");
                    }

                    counter.Add(64);
                }
            }, TaskCreationOptions.LongRunning);
        }

        Task.WaitAll(threads);
        Report(Describe("ZLibDecoder", strategy), counter, baseline);

        if (owned is not null)
        {
            foreach (ZLibDecoder decoder in owned)
            {
                decoder.Dispose();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static System.Buffers.OperationStatus DecompressOnce(ReadOnlySpan<byte> source, Span<byte> output)
    {
        // What a static one-shot libdeflate API would do if it existed: allocate the state,
        // decompress, free the state.
        using var decompressor = new LibDeflateDecompressor();
        return decompressor.Decompress(source, output, out _);
    }

    private static void Report(string name, OperationCounter counter, long baseline)    {
        Collect();
        long after = PrivateBytes();
        long ops = counter.Value;
        double perOpUs = counter.ElapsedSeconds * 1_000_000 / Math.Max(1, ops);

        Console.WriteLine($"| {name,-32} | {ops,12:N0} ops | {counter.ElapsedSeconds,6:F2} s | " +
            $"{perOpUs,6:F2} us/op | {ops / Math.Max(0.001, counter.ElapsedSeconds),10:N0} op/s | " +
            $"private +{Math.Max(0, after - baseline) / (double)(1024 * 1024),6:F1} MiB |");
    }

    private static void Collect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private static long PrivateBytes()
    {
        using var process = Process.GetCurrentProcess();
        process.Refresh();
        return process.PrivateMemorySize64;
    }

    private sealed class OperationCounter
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private long _value;

        public long Value => Interlocked.Read(ref _value);

        public double ElapsedSeconds => _stopwatch.Elapsed.TotalSeconds;

        public void Add(long count) => Interlocked.Add(ref _value, count);
    }
}
