using System.Buffers;
using McProtoNet.Transport.Compression;

namespace McProtoNet.Tests.Transport;

/// <summary>
///     Every libdeflate wrapper frees its native handle once and refuses to work afterwards. A call
///     that gets past the guard hands a freed pointer to native code, and a second free of the same
///     handle is a double free.
/// </summary>
public class CompressionDisposeTests
{
    private static byte[] Payload()
    {
        var payload = new byte[512];
        for (var i = 0; i < payload.Length; i++) payload[i] = (byte)(i * 7);
        return payload;
    }

    private static byte[] Compress(byte[] payload, out int written)
    {
        using var compressor = new LibDeflateCompressor(4);
        var compressed = new byte[compressor.GetBound(payload.Length)];
        written = compressor.Compress(payload, compressed);
        return compressed;
    }

    /// <summary>The instance LibDeflateCache hands out: it works, then refuses every call after Dispose.</summary>
    [Fact]
    public void Decompressor_RefusesEveryCallAfterDispose()
    {
        var payload = Payload();
        var compressed = Compress(payload, out var written);

        var decompressor = new LibDeflateDecompressor();
        var restored = new byte[payload.Length];

        Assert.Equal(OperationStatus.Done,
            decompressor.Decompress(compressed.AsSpan(0, written), restored, out var read));
        Assert.Equal(payload.Length, read);
        Assert.Equal(payload, restored);

        decompressor.Dispose();
        decompressor.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            decompressor.Decompress(compressed.AsSpan(0, written), restored, out _));
    }

    /// <summary>The compressor guard: same story, frees once and refuses afterwards.</summary>
    [Fact]
    public void Compressor_RefusesEveryCallAfterDispose()
    {
        var payload = Payload();
        var compressor = new LibDeflateCompressor(4);
        var compressed = new byte[compressor.GetBound(payload.Length)];

        Assert.True(compressor.Compress(payload, compressed) > 0);

        compressor.Dispose();
        compressor.Dispose();

        Assert.Throws<ObjectDisposedException>(() => compressor.GetBound(payload.Length));
        Assert.Throws<ObjectDisposedException>(() => compressor.Compress(payload, compressed));
    }
}
