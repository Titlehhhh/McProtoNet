// McProtoNet.Next experiment — frame floor. Lives next to the old API, changes nothing in src/**.

using System;
using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Next;

/// <summary>
///     Assembles wire frames onto a <see cref="PipeWriter" />. This is internal machinery; the
///     public door is the client's <c>SendFrameAsync</c>, which calls <see cref="WriteFrame" /> and
///     then flushes.
/// </summary>
/// <remarks>
///     The writer works on plaintext bytes: encryption, when enabled, happens on the pipe below
///     this one. It honours the connection's compression threshold by default, but
///     <see cref="FrameOptions" /> lets a caller override the compression decision and even lie
///     about the uncompressed size — the low-level controls a tool needs to probe foreign servers.
/// </remarks>
internal sealed class FrameWriter : IDisposable
{
    // Matches the transport's compression level so frames written here are byte-comparable with the
    // rest of the library.
    private const int CompressionLevel = 4;

    private readonly PipeWriter _writer;
    private ZlibCompressorHeapAlloc? _compressor;

    internal FrameWriter(PipeWriter writer)
    {
        _writer = writer;
    }

    /// <summary>
    ///     Gets or sets the compression threshold, in bytes; a negative value (the default, -1)
    ///     means compression is disabled and frames carry a single length prefix with a raw body.
    /// </summary>
    internal int CompressionThreshold { get; set; } = -1;

    /// <summary>
    ///     Writes one frame from <paramref name="payload" /> into the pipe, following
    ///     <paramref name="options" />. Does not flush; the caller flushes the pipe afterwards.
    /// </summary>
    /// <param name="payload">The frame body. The window is consumed within the call and not held.</param>
    /// <param name="options">Per-frame compression overrides.</param>
    /// <remarks>
    ///     <para>
    ///         With compression disabled the frame is a length prefix followed by the raw body, and
    ///         <paramref name="options" /> is ignored. With compression enabled the body is compressed
    ///         or sent through per <see cref="FrameOptions.Compression" />, and a compressed frame
    ///         carries the size field — the real body length, or
    ///         <see cref="FrameOptions.ClaimedUncompressedSize" /> when set, so a caller can send a
    ///         size that does not match the body on purpose.
    ///     </para>
    ///     <para>
    ///         The writer does not cap the frame size: an oversized frame is a legitimate probe, and
    ///         capping it here would defeat the point of this door.
    ///     </para>
    /// </remarks>
    internal void WriteFrame(ReadOnlySpan<byte> payload, FrameOptions options)
    {
        int threshold = CompressionThreshold;
        if (threshold < 0)
        {
            _writer.WriteVarInt(payload.Length);
            _writer.Write(payload);
            return;
        }

        bool compress = options.Compression switch
        {
            FrameCompression.Force => true,
            FrameCompression.Never => false,
            _ => payload.Length >= threshold, // Auto: honest threshold.
        };

        if (!compress)
        {
            // Passthrough frame: the size field is the mandatory zero (one VarInt byte).
            _writer.WriteVarInt(payload.Length + 1);
            _writer.WriteVarInt(0);
            _writer.Write(payload);
            return;
        }

        int claimed = options.ClaimedUncompressedSize ?? payload.Length;
        var compressor = _compressor ??= new ZlibCompressorHeapAlloc(CompressionLevel);

        int bound = compressor.GetBound(payload.Length);
        byte[] rented = ArrayPool<byte>.Shared.Rent(bound);
        try
        {
            int compressedLength = compressor.Compress(payload, rented.AsSpan(0, bound));

            // The outer length covers the size field plus the compressed bytes. The size field
            // encodes 'claimed', so its byte count is measured from 'claimed', not the real length.
            int packetLength = claimed.GetVarIntLength() + compressedLength;
            _writer.WriteVarInt(packetLength);
            _writer.WriteVarInt(claimed);
            _writer.Write(rented.AsSpan(0, compressedLength));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    /// <summary>
    ///     Writes one frame per <paramref name="options" /> and flushes the pipe: the async door
    ///     the client's <c>SendFrameAsync</c>/<c>SendPacketAsync</c> sit on. The payload is
    ///     consumed within the call and not retained after the returned task completes.
    /// </summary>
    /// <param name="payload">The frame body. Not held after completion; a rented buffer may be reused.</param>
    /// <param name="options">Per-frame compression overrides.</param>
    /// <param name="cancellationToken">A token that cancels the flush.</param>
    /// <returns>The flush result of the underlying pipe.</returns>
    // Integrator note: added to match the client author's assumed FrameWriter surface
    // (ValueTask SendFrameAsync(ReadOnlyMemory, FrameOptions, ct) backed by WriteFrame + FlushAsync).
    internal ValueTask<FlushResult> SendFrameAsync(
        ReadOnlyMemory<byte> payload, FrameOptions options = default, CancellationToken cancellationToken = default)
    {
        WriteFrame(payload.Span, options);
        return FlushAsync(cancellationToken);
    }

    /// <summary>Flushes the underlying pipe.</summary>
    /// <param name="cancellationToken">A token that cancels the flush.</param>
    /// <returns>The flush result of the underlying pipe.</returns>
    internal ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
        => _writer.FlushAsync(cancellationToken);

    /// <summary>Releases the cached compressor.</summary>
    public void Dispose()
    {
        _compressor?.Dispose();
        _compressor = null;
    }
}
