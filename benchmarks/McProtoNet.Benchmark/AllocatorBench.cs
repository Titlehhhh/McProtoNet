using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using McProtoNet.Transport.Compression;
using McProtoNet.Transport.Compression.Native;

namespace McProtoNet.Benchmark;

/// <summary>
/// Experiment: does replacing libdeflate's malloc/free with a pool make the "allocate a
/// (de)compressor per call" path cheaper?
///
/// The allocator is passed per instance through <c>libdeflate_options</c> to the <c>_ex</c>
/// allocation entry points, so nothing global is touched: the cached instances elsewhere in the
/// process keep libdeflate's default allocator.
///
/// Buffers come from <see cref="ArrayPool{T}"/> and are pinned; each allocation carries a 16-byte
/// header with the <see cref="GCHandle"/> of the pinned array, so free() is O(1) with no lookup table.
/// </summary>
internal static class AllocatorBench
{
    private const int HeaderSize = 16;
    private const int Iterations = 200_000;

    [StructLayout(LayoutKind.Sequential)]
    private unsafe struct LibDeflateOptions
    {
        public UIntPtr sizeof_options;
        public delegate* unmanaged[Cdecl]<nuint, IntPtr> malloc_func;
        public delegate* unmanaged[Cdecl]<IntPtr, void> free_func;
    }

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern IntPtr libdeflate_alloc_decompressor();

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern IntPtr libdeflate_alloc_decompressor_ex(in LibDeflateOptions options);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern void libdeflate_free_decompressor(IntPtr decompressor);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern Decompression.libdeflate_result libdeflate_zlib_decompress(IntPtr decompressor,
        in byte @in, nuint in_nbytes, ref byte @out, nuint out_nbytes_avail, out nuint actual_out_nbytes_ret);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern IntPtr libdeflate_alloc_compressor(int compression_level);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern IntPtr libdeflate_alloc_compressor_ex(int compression_level, in LibDeflateOptions options);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern void libdeflate_free_compressor(IntPtr compressor);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern UIntPtr libdeflate_zlib_compress_bound(IntPtr compressor, UIntPtr in_nbytes);

    [DllImport(Constants.DllName, CallingConvention = Constants.CallConv)]
    private static extern UIntPtr libdeflate_zlib_compress(IntPtr compressor, in byte @in, nuint in_nbytes,
        ref byte @out, nuint out_nbytes_avail);

    private static byte[] _plain = null!;
    private static byte[] _compressed = null!;

    public static int Run()
    {
        _plain = ZlibBenchData.Make(ZlibPayload.Text, 8192, 12345);
        _compressed = Compress(_plain);

        Console.WriteLine($"# per-call decompression, {Iterations:N0} iterations, payload {_plain.Length} B, " +
            $"zlib {_compressed.Length} B, allocator per instance via libdeflate_alloc_decompressor_ex");

        Measure("libdeflate default allocator", pooled: false, warmup: 10_000);
        Measure("libdeflate ArrayPool allocator", pooled: true, warmup: 10_000);

        Console.WriteLine();
        Console.WriteLine("# per-call compression (level 4), 20 000 iterations");
        MeasureCompress("libdeflate default allocator", pooled: false, warmup: 2_000);
        MeasureCompress("libdeflate ArrayPool allocator", pooled: true, warmup: 2_000);

        return 0;
    }

    private static unsafe void MeasureCompress(string name, bool pooled, int warmup)
    {
        const int iterations = 20_000;
        var output = new byte[_plain.Length + 1024];

        var options = default(LibDeflateOptions);
        if (pooled)
        {
            options.sizeof_options = (nuint)sizeof(LibDeflateOptions);
            options.malloc_func = &PooledMalloc;
            options.free_func = &PooledFree;
        }

        IntPtr Allocate() => pooled
            ? libdeflate_alloc_compressor_ex(4, in options)
            : libdeflate_alloc_compressor(4);

        int CompressOnce(IntPtr handle)
        {
            nuint bound = libdeflate_zlib_compress_bound(handle, (nuint)_plain.Length);
            if ((int)bound > output.Length)
                Array.Resize(ref output, (int)bound);

            return (int)libdeflate_zlib_compress(handle, in _plain[0], (nuint)_plain.Length,
                ref output[0], (nuint)output.Length);
        }

        for (int i = 0; i < warmup; i++)
        {
            var handle = Allocate();
            CompressOnce(handle);
            libdeflate_free_compressor(handle);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var handle = Allocate();
            CompressOnce(handle);
            libdeflate_free_compressor(handle);
        }

        stopwatch.Stop();

        double microseconds = stopwatch.Elapsed.TotalMilliseconds * 1000 / iterations;
        Console.WriteLine($"| {name,-32} | {stopwatch.Elapsed.TotalMilliseconds,8:F1} ms | " +
            $"{microseconds,6:F2} us/op |");
    }

    private static unsafe void Measure(string name, bool pooled, int warmup)
    {
        var output = new byte[_plain.Length];

        // Function pointers to managed [UnmanagedCallersOnly] methods are static and live for the
        // whole process, so libdeflate can keep calling them until the last free.
        var options = default(LibDeflateOptions);
        if (pooled)
        {
            options.sizeof_options = (nuint)sizeof(LibDeflateOptions);
            options.malloc_func = &PooledMalloc;
            options.free_func = &PooledFree;
        }

        IntPtr Allocate() => pooled
            ? libdeflate_alloc_decompressor_ex(in options)
            : libdeflate_alloc_decompressor();

        int Decompress(IntPtr handle)
        {
            var status = libdeflate_zlib_decompress(handle, in _compressed[0], (nuint)_compressed.Length,
                ref output[0], (nuint)output.Length, out nuint written);
            if (status != Decompression.libdeflate_result.LIBDEFLATE_SUCCESS)
                throw new InvalidOperationException($"inflate failed: {status}");
            return (int)written;
        }

        for (int i = 0; i < warmup; i++)
        {
            var handle = Allocate();
            Decompress(handle);
            libdeflate_free_decompressor(handle);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long allocatedBefore = GC.GetTotalAllocatedBytes(precise: true);
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            var handle = Allocate();
            int written = Decompress(handle);
            libdeflate_free_decompressor(handle);

            if (written != _plain.Length)
                throw new InvalidOperationException("size mismatch");
        }

        stopwatch.Stop();
        long allocated = GC.GetTotalAllocatedBytes(precise: true) - allocatedBefore;

        if (!output.AsSpan().SequenceEqual(_plain))
            throw new InvalidOperationException("roundtrip mismatch");

        double microseconds = stopwatch.Elapsed.TotalMilliseconds * 1000 / Iterations;
        Console.WriteLine($"| {name,-32} | {stopwatch.Elapsed.TotalMilliseconds,8:F1} ms | " +
            $"{microseconds,6:F2} us/op | {allocated / (double)Iterations,5:F0} B/op managed |");
    }

    private static byte[] Compress(byte[] plain)
    {
        var compressor = LibDeflateCache.RentCompressor();
        var compressed = new byte[compressor.GetBound(plain.Length)];
        int written = compressor.Compress(plain, compressed);
        return compressed.AsSpan(0, written).ToArray();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe IntPtr PooledMalloc(nuint size)
    {
        int total = checked((int)size) + HeaderSize;
        byte[] array = ArrayPool<byte>.Shared.Rent(total);
        GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
        IntPtr basePtr = handle.AddrOfPinnedObject();

        // The header keeps the GCHandle so free() can unpin and return the buffer without a map.
        Unsafe.Write((void*)basePtr, (long)GCHandle.ToIntPtr(handle));
        return basePtr + HeaderSize;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static unsafe void PooledFree(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            return;
        }

        IntPtr basePtr = ptr - HeaderSize;
        var handle = GCHandle.FromIntPtr((IntPtr)Unsafe.Read<long>((void*)basePtr));
        var array = (byte[])handle.Target!;
        handle.Free();
        ArrayPool<byte>.Shared.Return(array);
    }
}
