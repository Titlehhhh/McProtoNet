// McProtoNet.Next experiment — frame floor. Lives next to the old API, changes nothing in src/**.

using System;
using System.Buffers;
using System.Net;
using System.Threading;
using McProtoNet.Net.Zlib;

namespace McProtoNet.Next;

/// <summary>
///     One wire frame handed out exactly as it arrived: the length the sender declared, the
///     compression header, and the raw body. Decompression is deferred — the frame floor never
///     unpacks a body on your behalf, so a garbage payload cannot crash the receive loop.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="Payload" /> is a window into the transport buffer. It is valid only until the
///         next frame is read from the same reader; read it, copy it, or call
///         <see cref="Decompress" /> immediately, and never hold it across an <c>await</c> that pulls
///         the next frame. To keep the bytes, either copy them or take ownership through
///         <see cref="Decompress" />, which returns a pooled buffer you own.
///     </para>
///     <para>
///         This is the low-level door for protocol tooling. The reader validates the frame's outer
///         length before slicing (see <see cref="MaxFrameSize" />), but reports the compression
///         header verbatim so you can see a malformed one; the claimed uncompressed size is only
///         validated when you call <see cref="Decompress" />.
///     </para>
///     <para>
///         The type is a struct larger than the usual 16-byte guideline on purpose: it is a
///         short-lived window on the hot path, passed by value out of an enumeration — the same
///         trade the BCL makes for <c>ReadResult</c> and <c>ReadOnlySequence&lt;byte&gt;</c> —
///         so a frame costs zero allocations.
///     </para>
/// </remarks>
public readonly struct RawFrame
{
    /// <summary>
    ///     The largest frame length, in bytes, the reader accepts before treating the frame as a
    ///     protocol violation. A frame whose declared length is negative or above this ceiling is
    ///     rejected without allocating, which closes the denial-of-service vector of a hostile
    ///     length prefix.
    /// </summary>
    /// <remarks>
    ///     Set to 8 MiB as a deliberately generous guard. The exact ceiling is an open protocol
    ///     question (design question D3): vanilla's true maximum packet size and its "badly
    ///     compressed packet" rule are not yet pinned down, so this value only has to be safe, not
    ///     precise.
    /// </remarks>
    public const int MaxFrameSize = 8 * 1024 * 1024;

    // Decision (API review follow-up): the decompressor stays cached per thread and is never
    // disposed. Decompression runs on the read enumeration's task — in practice one pooled
    // thread at a time — so the population is bounded by the thread pool, and paying one cached
    // instance per pool thread is cheaper than allocating a decompressor per compressed frame.
    // A pooled-and-disposed decompressor scheme is deliberately out of scope for the experiment.
    [ThreadStatic] private static ZlibDecompressorHeapAlloc? t_decompressor;

    internal RawFrame(int declaredLength, FrameEnvelope envelope, ReadOnlySequence<byte> payload)
    {
        DeclaredLength = declaredLength;
        Envelope = envelope;
        Payload = payload;
    }

    /// <summary>Gets the frame length the sender declared in the outer length prefix, in bytes.</summary>
    /// <remarks>
    ///     This is the number of body bytes after the length prefix, including the compression size
    ///     field when one is present. The reader has already checked it against
    ///     <see cref="MaxFrameSize" />.
    /// </remarks>
    public int DeclaredLength { get; }

    /// <summary>Gets the compression header of the frame, read as it arrived.</summary>
    public FrameEnvelope Envelope { get; }

    /// <summary>
    ///     Gets the raw body: the zlib-compressed bytes when <see cref="FrameEnvelope.IsCompressed" />
    ///     is <c>true</c>, otherwise the plain body ready to use.
    /// </summary>
    /// <remarks>
    ///     This is a window into the transport buffer, valid only until the next frame is read. See
    ///     the remarks on <see cref="RawFrame" /> for the lifetime rule.
    /// </remarks>
    public ReadOnlySequence<byte> Payload { get; }

    /// <summary>
    ///     Gets a value indicating whether the body is zlib-compressed and must go through
    ///     <see cref="Decompress" /> before use.
    /// </summary>
    public bool IsCompressed => Envelope.IsCompressed;

    /// <summary>
    ///     Decompresses the body into a pooled buffer that the caller owns, validating the sender's
    ///     claimed size along the way.
    /// </summary>
    /// <returns>
    ///     An <see cref="IMemoryOwner{T}" /> holding the uncompressed body. The caller owns it and
    ///     must dispose it to return the memory to the pool; disposing more than once is safe
    ///     (idempotent). It outlives the transport window, so it is safe to keep past the next
    ///     frame read.
    /// </returns>
    /// <exception cref="InvalidOperationException">The frame is not compressed; use <see cref="Payload" /> directly.</exception>
    /// <exception cref="ProtocolViolationException">
    ///     The claimed uncompressed size is negative or above <see cref="MaxFrameSize" />, the body is
    ///     not valid zlib data, or the real decompressed length does not match the claimed size. Each
    ///     of these is a lie or corruption from the peer, caught here so it never reaches your buffer
    ///     as a silent tail of stale bytes.
    /// </exception>
    /// <remarks>
    ///     Validating the real output length against the claimed size is mandatory: without it a
    ///     forged size could hand back a buffer padded with leftover bytes from a reused arena.
    /// </remarks>
    public IMemoryOwner<byte> Decompress()
    {
        if (!Envelope.IsCompressed)
            throw new InvalidOperationException(
                "Frame is not compressed; read Payload directly instead of calling Decompress.");

        int claimed = Envelope.ClaimedUncompressedSize;
        if (claimed < 0 || claimed > MaxFrameSize)
            throw new ProtocolViolationException(
                $"Frame declares an uncompressed size of {claimed} bytes, outside the allowed range [0, {MaxFrameSize}].");

        // API-review follow-up: the buffer is returned behind the library's own sealed
        // IMemoryOwner with an idempotent Dispose — not the old Serialization MemoryOwner
        // struct, whose copyable-by-value shape made double-Dispose corrupt pooled data.
        byte[] output = ArrayPool<byte>.Shared.Rent(claimed);
        try
        {
            var decompressor = t_decompressor ??= new ZlibDecompressorHeapAlloc();

            OperationStatus status;
            int written;
            if (Payload.IsSingleSegment)
            {
                status = decompressor.Decompress(Payload.FirstSpan, output.AsSpan(0, claimed), out written);
            }
            else
            {
                int length = (int)Payload.Length;
                byte[] rented = ArrayPool<byte>.Shared.Rent(length);
                try
                {
                    Payload.CopyTo(rented);
                    status = decompressor.Decompress(rented.AsSpan(0, length), output.AsSpan(0, claimed), out written);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(rented);
                }
            }

            if (status != OperationStatus.Done)
                throw new ProtocolViolationException($"Frame body is not valid zlib data: {status}.");

            if (written != claimed)
                throw new ProtocolViolationException(
                    $"Frame claimed {claimed} uncompressed bytes but produced {written}.");

            var owner = new DecompressedBody(output, claimed);
            output = null!; // ownership handed off — the finally below must not return it
            return owner;
        }
        finally
        {
            if (output is not null)
                ArrayPool<byte>.Shared.Return(output);
        }
    }

    /// <summary>
    ///     The pooled buffer behind <see cref="Decompress" />: a class (single identity, no
    ///     copy-by-value trap) whose <see cref="Dispose" /> is idempotent — the buffer goes back
    ///     to the pool exactly once no matter how many times it is disposed.
    /// </summary>
    private sealed class DecompressedBody : IMemoryOwner<byte>
    {
        private readonly int _length;
        private byte[]? _array;

        internal DecompressedBody(byte[] array, int length)
        {
            _array = array;
            _length = length;
        }

        public Memory<byte> Memory
        {
            get
            {
                byte[]? array = _array;
                ObjectDisposedException.ThrowIf(array is null, this);
                return array.AsMemory(0, _length);
            }
        }

        public void Dispose()
        {
            byte[]? array = Interlocked.Exchange(ref _array, null);
            if (array is not null)
                ArrayPool<byte>.Shared.Return(array);
        }
    }
}
