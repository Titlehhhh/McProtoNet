using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace McProtoNet.Transport.Compression.Native;

/// <summary>
/// Owns a native libdeflate compressor handle. The handle is released exactly once — by
/// <see cref="SafeHandle.Dispose()"/>, or by the SafeHandle finalizer when the owner forgets — and
/// never while a P/Invoke that takes the handle is in flight.
/// </summary>
internal sealed class LibDeflateCompressorHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    /// <summary>Wraps an already allocated native handle.</summary>
    /// <param name="preexistingHandle">The handle returned by libdeflate.</param>
    /// <param name="ownsHandle">Whether this instance must free the handle.</param>
    public LibDeflateCompressorHandle(IntPtr preexistingHandle, bool ownsHandle)
        : base(ownsHandle)
    {
        SetHandle(preexistingHandle);
    }

    /// <summary>Allocates a compressor at the given libdeflate compression level (0-12).</summary>
    /// <exception cref="InvalidOperationException">libdeflate refused to allocate.</exception>
    public static LibDeflateCompressorHandle Allocate(int compressionLevel)
    {
        var handle = Compression.libdeflate_alloc_compressor(compressionLevel);
        if (handle == IntPtr.Zero)
        {
            ThrowHelperFailedAllocCompressor();
        }

        return new LibDeflateCompressorHandle(handle, ownsHandle: true);
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        Compression.libdeflate_free_compressor(handle);
        return true;
    }

    [DoesNotReturn]
    private static void ThrowHelperFailedAllocCompressor() =>
        throw new InvalidOperationException("Failed to allocate compressor");
}

/// <summary>
/// Owns a native libdeflate decompressor handle. The handle is released exactly once — by
/// <see cref="SafeHandle.Dispose()"/>, or by the SafeHandle finalizer when the owner forgets — and
/// never while a P/Invoke that takes the handle is in flight.
/// </summary>
internal sealed class LibDeflateDecompressorHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    /// <summary>Wraps an already allocated native handle.</summary>
    /// <param name="preexistingHandle">The handle returned by libdeflate.</param>
    /// <param name="ownsHandle">Whether this instance must free the handle.</param>
    public LibDeflateDecompressorHandle(IntPtr preexistingHandle, bool ownsHandle)
        : base(ownsHandle)
    {
        SetHandle(preexistingHandle);
    }

    /// <summary>Allocates a decompressor.</summary>
    /// <exception cref="InvalidOperationException">libdeflate refused to allocate.</exception>
    public static LibDeflateDecompressorHandle Allocate()
    {
        var handle = Decompression.libdeflate_alloc_decompressor();
        if (handle == IntPtr.Zero)
        {
            ThrowHelperFailedAllocDecompressor();
        }

        return new LibDeflateDecompressorHandle(handle, ownsHandle: true);
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        Decompression.libdeflate_free_decompressor(handle);
        return true;
    }

    [DoesNotReturn]
    private static void ThrowHelperFailedAllocDecompressor() =>
        throw new InvalidOperationException("Failed to allocate decompressor");
}
