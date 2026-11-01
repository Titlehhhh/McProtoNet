using System.Buffers;
using System.Runtime.CompilerServices;

namespace McProtoNet.Transport.Compression;

/// <summary>Provides zlib decompression through the thread-local libdeflate decompressor.</summary>
internal static class LibDeflateStatic
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperationStatus Decompress(ReadOnlySpan<byte> input, Span<byte> output, out int written)
    {
        return LibDeflateCache.RentDecompressor().Decompress(input, output, out written);
    }
}
