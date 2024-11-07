using System.Buffers;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using DotNext;
using DotNext.Buffers;

namespace McProtoNet.Net.Zlib;

internal static class Extensions
{
    private static MemoryAllocator<byte> s_allocator = ArrayPool<byte>.Shared.ToAllocator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemoryOwner<byte> Decompress(this ReadOnlySequence<byte> compressedSequence,
        int decompressSize)
    {
        MemoryOwner<byte> decompress = s_allocator.AllocateExactly(decompressSize);
        try
        {
            if (compressedSequence.IsSingleSegment)
            {
                var status = LibDeflateStatic.Decompress(compressedSequence.FirstSpan, decompress.Span, out _);

                if (status != OperationStatus.Done)
                {
                    throw new InvalidOperationException("Zlib decompress error: " + status);
                }
            }
            else
            {
                MemoryOwner<byte> tempBuffer = s_allocator.AllocateExactly((int)compressedSequence.Length);

                try
                {
                    compressedSequence.CopyTo(tempBuffer.Span);

                    var status = LibDeflateStatic.Decompress(tempBuffer.Span, decompress.Span, out _);

                    if (status != OperationStatus.Done)
                    {
                        throw new InvalidOperationException("Zlib decompress error: " + status);
                    }
                }
                finally
                {
                    tempBuffer.Dispose();
                }
            }

            return decompress;
        }
        catch
        {
            decompress.Dispose();
            throw;
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