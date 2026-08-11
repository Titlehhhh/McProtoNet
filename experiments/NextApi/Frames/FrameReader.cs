// McProtoNet.Next experiment — frame floor. Lives next to the old API, changes nothing in src/**.

using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Runtime.CompilerServices;

namespace McProtoNet.Next;

/// <summary>
///     Slices a byte stream into wire frames. This is internal machinery sitting on top of a
///     <see cref="PipeReader" />; the public door is the client's <c>ReadFramesAsync</c>, which
///     hands out the <see cref="RawFrame" /> values produced here.
/// </summary>
/// <remarks>
///     <para>
///         The reader works on plaintext bytes: decryption, when enabled, happens on the pipe below
///         this one, so this class never touches ciphers. It cuts frames by their VarInt length,
///         validates that length before slicing, and reads the compression size field when the
///         connection is compressed. Bodies are handed out raw — decompression is the caller's,
///         deferred to <see cref="RawFrame.Decompress" />.
///     </para>
///     <para>
///         Every length that comes off the wire is validated. A length that is not a well-formed
///         VarInt, is negative, or exceeds <see cref="RawFrame.MaxFrameSize" /> is a
///         <see cref="ProtocolViolationException" /> raised with a clear message, not an exception
///         thrown from deep inside a buffer slice or a pool rental. The claimed uncompressed size,
///         by contrast, is passed through untouched so a caller can inspect a malformed one; it is
///         validated only if and when the caller decompresses.
///     </para>
/// </remarks>
internal sealed class FrameReader
{
    private readonly PipeReader _reader;

    internal FrameReader(PipeReader reader)
    {
        _reader = reader;
    }

    /// <summary>
    ///     Gets or sets the compression threshold, in bytes; a negative value (the default, -1)
    ///     means compression is disabled and frames carry no size field.
    /// </summary>
    /// <remarks>
    ///     Only the sign matters to the reader: it decides whether to expect a compression size
    ///     field. The owner must set this before starting concurrent reads — changing it while a
    ///     read loop runs races that loop.
    /// </remarks>
    internal int CompressionThreshold { get; set; } = -1;

    /// <summary>
    ///     Reads frames until the connection closes, yielding each one as it is fully buffered.
    /// </summary>
    /// <param name="cancellationToken">A token that stops the loop.</param>
    /// <returns>An asynchronous stream of frames.</returns>
    /// <remarks>
    ///     A clean close after a whole frame ends the stream normally. A close in the middle of a
    ///     frame is a truncation and raises <see cref="ProtocolViolationException" />. Each yielded
    ///     <see cref="RawFrame.Payload" /> is valid only until the loop pulls the next frame.
    /// </remarks>
    /// <exception cref="ProtocolViolationException">A frame length is malformed, out of range, or the stream was truncated.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken" /> was cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The owning client was disposed while the enumeration was reading.</exception>
    internal async IAsyncEnumerable<RawFrame> ReadFramesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        while (true)
        {
            var result = await _reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var buffer = result.Buffer;

            try
            {
                while (TryReadFrame(ref buffer, out var frame))
                    yield return frame;

                if (result.IsCompleted)
                {
                    if (!buffer.IsEmpty)
                        throw new ProtocolViolationException(
                            "Connection closed in the middle of a frame: leftover bytes could not form a whole frame.");
                    yield break;
                }

                if (result.IsCanceled)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // The only caller of CancelPendingRead on this pipe is the client's
                    // shutdown path. A cancel that is not the consumer's own token means
                    // the client was disposed mid-read — say so, do not fake a clean end
                    // of stream (the consumer must be able to tell "server closed" from
                    // "somebody disposed the client under me").
                    throw new ObjectDisposedException(nameof(MinecraftClient));
                }
            }
            finally
            {
                _reader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
    }

    /// <summary>
    ///     Reads frames and yields each one's payload with the compression envelope opened and
    ///     verified: compressed bodies are decompressed and their claimed size checked, plain
    ///     bodies pass through as-is. This is the packet-floor feed.
    /// </summary>
    /// <param name="cancellationToken">A token that stops the loop.</param>
    /// <returns>An asynchronous stream of decompressed payloads.</returns>
    /// <remarks>
    ///     Each yielded sequence is a window — into the transport buffer for a plain body, into a
    ///     pooled buffer for a decompressed one — valid only until the next
    ///     <c>MoveNextAsync</c>. A clean close at a frame boundary ends the stream normally; a
    ///     close mid-frame raises <see cref="ProtocolViolationException" /> (truncation).
    /// </remarks>
    /// <exception cref="ProtocolViolationException">
    ///     A frame length is malformed or out of range, the stream was truncated, or a compressed
    ///     body lies about its size or is not valid zlib data (hardening plan К1/К2 — the packet
    ///     floor never sees an unverified payload).
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken" /> was cancelled.</exception>
    /// <exception cref="ObjectDisposedException">The owning client was disposed while the enumeration was reading.</exception>
    // Integrator note: added to match the client author's assumed FrameReader surface
    // (MinecraftClient.ReadPacketsCoreAsync consumes ReadPayloadsAsync).
    internal async IAsyncEnumerable<ReadOnlySequence<byte>> ReadPayloadsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var frame in ReadFramesAsync(cancellationToken).ConfigureAwait(false))
        {
            if (frame.IsCompressed)
            {
                // Decompress validates the claimed size range, the zlib stream, and
                // written == claimed (К2) before anything is handed out.
                using var owner = frame.Decompress();
                yield return new ReadOnlySequence<byte>(owner.Memory);
            }
            else
            {
                yield return frame.Payload;
            }
        }
    }

    /// <summary>
    ///     Tries to slice one whole frame off the front of <paramref name="buffer" />. On success the
    ///     buffer is advanced past that frame.
    /// </summary>
    /// <param name="buffer">The buffered bytes; advanced past the frame on success, left as-is otherwise.</param>
    /// <param name="frame">The sliced frame on success.</param>
    /// <returns><c>true</c> when a whole frame was available; <c>false</c> when more bytes are needed.</returns>
    /// <exception cref="ProtocolViolationException">A length prefix is malformed or out of range, or a compressed frame is too short for its size field.</exception>
    private bool TryReadFrame(ref ReadOnlySequence<byte> buffer, out RawFrame frame)
    {
        frame = default;

        if (buffer.IsEmpty)
            return false;

        // Outer length prefix: this drives buffer slicing, so it is validated strictly before any
        // slice or rental. A negative or oversized length terminates the connection instead of
        // reaching GetPosition or a pool rental from deep in the stack.
        var scan = TryScanVarInt(in buffer, out int length, out int headerLen);
        if (scan == VarIntScan.NeedMore)
            return false;
        if (scan == VarIntScan.TooLong)
            throw new ProtocolViolationException("Frame length prefix is not a valid VarInt (more than five bytes).");

        if (length < 0 || length > RawFrame.MaxFrameSize)
            throw new ProtocolViolationException(
                $"Frame declares a length of {length} bytes, outside the allowed range [0, {RawFrame.MaxFrameSize}].");

        if ((long)length + headerLen > buffer.Length)
            return false; // The whole frame has not arrived yet.

        var start = buffer.GetPosition(headerLen);
        var end = buffer.GetPosition(length, start);
        var body = buffer.Slice(start, end);
        buffer = buffer.Slice(end);

        FrameEnvelope envelope;
        ReadOnlySequence<byte> payload;

        if (CompressionThreshold < 0)
        {
            // No compression on the connection: the whole body is the plain payload.
            envelope = FrameEnvelope.Uncompressed;
            payload = body;
        }
        else
        {
            // The size field must be present inside the framed body. A body too short to hold it, or
            // an over-long VarInt, is a malformed frame — a structural error, unlike the size value
            // itself, which is reported verbatim and only range-checked at decompression time.
            var envScan = TryScanVarInt(in body, out int declaredUncompressedSize, out int envLen);
            if (envScan != VarIntScan.Ok)
                throw new ProtocolViolationException(
                    "Compressed frame is too short to contain its uncompressed-size field.");

            payload = body.Slice(envLen);
            envelope = declaredUncompressedSize == 0
                ? FrameEnvelope.Passthrough
                : FrameEnvelope.Compressed(declaredUncompressedSize);
        }

        frame = new RawFrame(length, envelope, payload);
        return true;
    }

    private enum VarIntScan
    {
        /// <summary>Not enough bytes buffered yet to hold a whole VarInt.</summary>
        NeedMore,

        /// <summary>A whole VarInt was read.</summary>
        Ok,

        /// <summary>The VarInt ran past five bytes without terminating — it cannot encode a 32-bit value.</summary>
        TooLong,
    }

    /// <summary>
    ///     Scans a VarInt from the front of a sequence without throwing. This is used instead of the
    ///     shared VarInt reader so that an over-long prefix becomes a clean status the caller turns
    ///     into a <see cref="ProtocolViolationException" />, rather than an arithmetic exception from
    ///     deep in the primitive reader.
    /// </summary>
    private static VarIntScan TryScanVarInt(in ReadOnlySequence<byte> buffer, out int value, out int bytesRead)
    {
        var reader = new SequenceReader<byte>(buffer);
        int result = 0;
        int shift = 0;

        for (int i = 0; i < 5; i++)
        {
            if (!reader.TryRead(out byte b))
            {
                value = 0;
                bytesRead = 0;
                return VarIntScan.NeedMore;
            }

            result |= (b & 0x7F) << shift;
            shift += 7;

            if ((b & 0x80) == 0)
            {
                value = result;
                bytesRead = i + 1;
                return VarIntScan.Ok;
            }
        }

        value = 0;
        bytesRead = 0;
        return VarIntScan.TooLong;
    }
}
