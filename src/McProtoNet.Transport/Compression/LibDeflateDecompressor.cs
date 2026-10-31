using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using McProtoNet.Transport.Compression.Native;
namespace McProtoNet.Transport.Compression;

/// <summary>
/// Provides zlib decompression through a native libdeflate decompressor handle owned by a
/// heap-allocated instance. Lifetime of the native handle belongs to
/// <see cref="LibDeflateDecompressorHandle"/>, so an instance that is dropped without Dispose is
/// still freed by the SafeHandle finalizer.
/// </summary>
internal class LibDeflateDecompressor : IDisposable
{
    private readonly LibDeflateDecompressorHandle decompressor;

    private bool disposedValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LibDeflateDecompressor()
    {
        decompressor = LibDeflateDecompressorHandle.Allocate();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static OperationStatus StatusFromResult(Decompression.libdeflate_result result)
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
        => result switch
        {
            Decompression.libdeflate_result.LIBDEFLATE_SUCCESS => OperationStatus.Done,
            Decompression.libdeflate_result.LIBDEFLATE_BAD_DATA => OperationStatus.InvalidData,
            Decompression.libdeflate_result.LIBDEFLATE_SHORT_OUTPUT => OperationStatus.NeedMoreData,
            Decompression.libdeflate_result.LIBDEFLATE_INSUFFICIENT_SPACE => OperationStatus.DestinationTooSmall,
        };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
    
    private OperationStatus DecompressCore(ReadOnlySpan<byte> input, Span<byte> output, out nuint bytesWritten)
    {
        return StatusFromResult(Decompression.libdeflate_zlib_decompress(decompressor,
            MemoryMarshal.GetReference(input),
            (nuint)input.Length, ref MemoryMarshal.GetReference(output), (nuint)output.Length, out bytesWritten));
    }

    public OperationStatus Decompress(ReadOnlySpan<byte> input, Span<byte> output, out int bytesWritten)
    {
        DisposedGuard();
        var status = DecompressCore(input, output, out nuint out_nbytes);
        switch (status)
        {
            case OperationStatus.Done:
                bytesWritten = (int)out_nbytes;
                return status;
            case OperationStatus.NeedMoreData:
            case OperationStatus.DestinationTooSmall:
            case OperationStatus.InvalidData:
            default:
                bytesWritten = default;
                return status;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DisposedGuard()
    {
        ObjectDisposedException.ThrowIf(disposedValue, this);
    }

    public void Dispose()
    {
        if (disposedValue)
            return;
        disposedValue = true;
        decompressor.Dispose();
    }
}
