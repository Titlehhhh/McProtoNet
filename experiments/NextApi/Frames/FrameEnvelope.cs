// McProtoNet.Next experiment — frame floor. Lives next to the old API, changes nothing in src/**.

namespace McProtoNet.Next;

/// <summary>
///     The compression header of one wire frame, read exactly as it arrived. It answers three
///     questions without touching the body: was a compression size field present at all, is the
///     body actually compressed, and what uncompressed size did the sender claim.
/// </summary>
/// <remarks>
///     <para>
///         Minecraft frames come in three shapes. When compression is disabled on the connection a
///         frame has no size field at all (<see cref="HasSizeField" /> is <c>false</c>).
///         When compression is enabled the frame carries a size field: zero means the body is below
///         the threshold and was sent uncompressed (a "passthrough" frame), and a non-zero value is
///         the sender's claimed uncompressed size for a zlib-compressed body.
///     </para>
///     <para>
///         The claimed size is reported verbatim and is <b>not</b> range-checked here — a hostile or
///         buggy sender can put anything in it. The reader keeps this frame alive so you can inspect
///         the lie; the size is only validated against the allowed range and the real output length
///         when you actually decompress, in <see cref="RawFrame.Decompress" />.
///     </para>
/// </remarks>
public readonly struct FrameEnvelope
{
    private readonly Kind _kind;
    private readonly int _claimedUncompressedSize;

    private FrameEnvelope(Kind kind, int claimedUncompressedSize)
    {
        _kind = kind;
        _claimedUncompressedSize = claimedUncompressedSize;
    }

    private enum Kind : byte
    {
        // No size field on the wire: compression is disabled on the connection.
        None = 0,

        // Size field present and zero: compression enabled, this body sent uncompressed.
        Passthrough = 1,

        // Size field present and non-zero: body is zlib-compressed, size is the claimed length.
        Compressed = 2,
    }

    /// <summary>An envelope for a frame from a connection with compression disabled: no size field.</summary>
    internal static FrameEnvelope Uncompressed => new(Kind.None, 0);

    /// <summary>An envelope for a compression-enabled frame whose body was sent uncompressed (size field zero).</summary>
    internal static FrameEnvelope Passthrough => new(Kind.Passthrough, 0);

    /// <summary>An envelope for a compressed frame carrying the claimed uncompressed size read from the wire.</summary>
    /// <param name="claimedUncompressedSize">The uncompressed size the sender declared, reported as read.</param>
    internal static FrameEnvelope Compressed(int claimedUncompressedSize) => new(Kind.Compressed, claimedUncompressedSize);

    /// <summary>
    ///     Gets a value indicating whether the frame carried a compression size field on the wire.
    ///     This is <c>false</c> only when compression is disabled on the connection.
    /// </summary>
    public bool HasSizeField => _kind != Kind.None;

    /// <summary>
    ///     Gets a value indicating whether the body is zlib-compressed and must be passed through
    ///     <see cref="RawFrame.Decompress" /> before use. When <c>false</c>, <see cref="RawFrame.Payload" />
    ///     already holds the plain body.
    /// </summary>
    public bool IsCompressed => _kind == Kind.Compressed;

    /// <summary>
    ///     Gets the uncompressed size the sender declared. It is the size field read from the wire
    ///     for a compressed frame, and zero for a passthrough or compression-disabled frame.
    /// </summary>
    /// <remarks>
    ///     This is the sender's claim, reported as read and not validated. For a compressed frame it
    ///     is checked against the allowed range and the real decompressed length only inside
    ///     <see cref="RawFrame.Decompress" />.
    /// </remarks>
    public int ClaimedUncompressedSize => _claimedUncompressedSize;
}
