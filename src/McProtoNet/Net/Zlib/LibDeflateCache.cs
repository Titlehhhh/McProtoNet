using System.Runtime.CompilerServices;

namespace McProtoNet.Net.Zlib;

internal static class LibDeflateCache
{
    [ThreadStatic] private static ZlibCompressorHeapAlloc? t_compressor;

    [ThreadStatic] private static ZlibDecompressorHeapAlloc? t_decompressor;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZlibCompressorHeapAlloc RentCompressor()
    {
        ZlibCompressorHeapAlloc compressor = t_compressor ??= new(4);
        return compressor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZlibDecompressorHeapAlloc RentDecompressor()
    {
        ZlibDecompressorHeapAlloc decompressor = t_decompressor ??= new();
        return decompressor;
    }
}