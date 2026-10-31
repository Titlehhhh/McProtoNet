using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using McProtoNet.Transport.Compression.Native;
namespace McProtoNet.Transport.Compression;

/// <summary>
/// Provides zlib compression through a native libdeflate compressor handle owned by a
/// heap-allocated instance. Lifetime of the native handle belongs to
/// <see cref="LibDeflateCompressorHandle"/>, so an instance that is dropped without Dispose is still
/// freed by the SafeHandle finalizer.
/// </summary>
internal class LibDeflateCompressor : IDisposable
{
    private readonly LibDeflateCompressorHandle compressor;

    // Set by Dispose and read by the guard; the SafeHandle itself refuses a second release.
    private bool disposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LibDeflateCompressor(int compressionLevel)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(compressionLevel);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(compressionLevel, 12);

        compressor = LibDeflateCompressorHandle.Allocate(compressionLevel);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private nuint CompressCore(ReadOnlySpan<byte> input, Span<byte> output)
    {
        return Native.Compression.libdeflate_zlib_compress(compressor, MemoryMarshal.GetReference(input), (nuint)input.Length,
            ref MemoryMarshal.GetReference(output), (nuint)output.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private nuint GetBoundCore(nuint inputLength)
    {
        return Native.Compression.libdeflate_zlib_compress_bound(compressor, inputLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compress(ReadOnlySpan<byte> input, Span<byte> output)
    {
        DisposedGuard();
        return (int)CompressCore(input, output);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetBound(int inputLength)
    {
        DisposedGuard();
        return (int)GetBoundCore((nuint)inputLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DisposedGuard()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
    }

    public void Dispose()
    {
        if (disposed)
            return;
        disposed = true;
        compressor.Dispose();
    }
}
