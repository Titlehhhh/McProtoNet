using System.Runtime.CompilerServices;

namespace McProtoNet.Transport.Compression;

/// <summary>
/// Provides the thread-local libdeflate compressor and decompressor instances.
/// </summary>
/// <remarks>
/// Compressors are kept per level: a level costs 200 KiB (level 1), 660 KiB (levels 2-9) or
/// 8.6 MiB (levels 10-12) of native memory per thread, so the array is filled lazily and only the
/// levels a thread actually asked for are ever allocated.
/// </remarks>
internal static class LibDeflateCache
{
    /// <summary>The compression level the library uses when the caller does not ask for one.</summary>
    public const int DefaultLevel = 4;

    private const int MaxLevel = 12;

    [ThreadStatic] private static LibDeflateCompressor?[]? t_compressors;

    [ThreadStatic] private static LibDeflateDecompressor? t_decompressor;

    /// <summary>Rents the thread-local compressor at <see cref="DefaultLevel"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LibDeflateCompressor RentCompressor() => RentCompressor(DefaultLevel);

    /// <summary>Rents the thread-local compressor for the given libdeflate level (0-12).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LibDeflateCompressor RentCompressor(int level)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(level);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, MaxLevel);

        LibDeflateCompressor?[] compressors = t_compressors ??= new LibDeflateCompressor?[MaxLevel + 1];
        return compressors[level] ??= new LibDeflateCompressor(level);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LibDeflateDecompressor RentDecompressor()
    {
        LibDeflateDecompressor decompressor = t_decompressor ??= new();
        return decompressor;
    }
}
