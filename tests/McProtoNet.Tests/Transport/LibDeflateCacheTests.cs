using System.Buffers;
using McProtoNet.Transport.Compression;

namespace McProtoNet.Tests.Transport;

/// <summary>
///     The thread-local compressor cache hands out one compressor per level, keeps the instance for
///     the thread, and refuses levels libdeflate does not support.
/// </summary>
public class LibDeflateCacheTests
{
    private static byte[] Payload()
    {
        var payload = new byte[4096];
        for (var i = 0; i < payload.Length; i++) payload[i] = (byte)(i * 7 % 251);
        return payload;
    }

    [Fact]
    public void RentCompressor_ReturnsSameInstanceForSameLevelAndDifferentForOtherLevels()
    {
        LibDeflateCompressor level1 = LibDeflateCache.RentCompressor(1);
        LibDeflateCompressor level1Again = LibDeflateCache.RentCompressor(1);
        LibDeflateCompressor level9 = LibDeflateCache.RentCompressor(9);
        LibDeflateCompressor defaultLevel = LibDeflateCache.RentCompressor();

        Assert.Same(level1, level1Again);
        Assert.NotSame(level1, level9);
        Assert.NotSame(level9, defaultLevel);
        Assert.Same(defaultLevel, LibDeflateCache.RentCompressor(LibDeflateCache.DefaultLevel));
    }

    [Fact]
    public void EveryRentedLevel_ProducesAValidZlibStream()
    {
        var payload = Payload();
        var decompressor = LibDeflateCache.RentDecompressor();

        foreach (var level in new[] { 0, 1, 4, 9 })
        {
            var compressor = LibDeflateCache.RentCompressor(level);
            var compressed = new byte[compressor.GetBound(payload.Length)];
            var written = compressor.Compress(payload, compressed);

            Assert.True(written > 0, $"level {level} produced no output");

            var restored = new byte[payload.Length];
            Assert.Equal(OperationStatus.Done,
                decompressor.Decompress(compressed.AsSpan(0, written), restored, out var read));
            Assert.Equal(payload.Length, read);
            Assert.Equal(payload, restored);
        }
    }

    [Fact]
    public void RentCompressor_RejectsLevelsOutsideLibDeflateRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => LibDeflateCache.RentCompressor(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => LibDeflateCache.RentCompressor(13));
    }
}
