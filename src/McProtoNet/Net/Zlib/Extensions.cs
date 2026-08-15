using System.Buffers;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using McProtoNet.Serialization;

namespace McProtoNet.Net.Zlib;

internal static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemoryOwner<byte> Decompress(this in ReadOnlySequence<byte> compressedSequence,
        int decompressSize)
    {
        var decompress = MemoryOwner<byte>.Allocate(decompressSize);
        try
        {
            byte[] rented = ArrayPool<byte>.Shared.Rent((int)compressedSequence.Length);
            try
            {
                compressedSequence.CopyTo(rented);
                using var ms = new MemoryStream(rented, 0, (int)compressedSequence.Length, writable: false);
                using var zLibStream = new ZLibStream(ms, CompressionMode.Decompress);
                var read = zLibStream.ReadAtLeast(decompress.Span, decompressSize);
                if (read != decompressSize)
                    throw new InvalidOperationException("Zlib decompress error: " + read);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }

            return decompress;
        }
        catch
        {
            decompress.Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Decompress(this in ReadOnlySequence<byte> compressedSequence,
        ref MemoryOwner<byte> owner)
    {
        if (compressedSequence.IsSingleSegment)
        {
            var status = LibDeflateStatic.Decompress(compressedSequence.FirstSpan, owner.Span, out _);

            if (status != OperationStatus.Done)
            {
                throw new InvalidOperationException("Zlib decompress error: " + status);
            }
        }
        else
        {
            byte[] rented = ArrayPool<byte>.Shared.Rent((int)compressedSequence.Length);
            try
            {
                compressedSequence.CopyTo(rented);
                var status = LibDeflateStatic.Decompress(
                    rented.AsSpan(0, (int)compressedSequence.Length),
                    owner.Span, out _);

                if (status != OperationStatus.Done)
                    throw new InvalidOperationException("Zlib decompress error: " + status);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }
}

internal static class LibDeflateStatic
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperationStatus Decompress(ReadOnlySpan<byte> input, Span<byte> output, out int written)
    {
        return LibDeflateCache.RentDecompressor().Decompress(input, output, out written);
    }
}
