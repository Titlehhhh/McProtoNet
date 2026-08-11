// McProtoNet.Next experiment — frame floor. Lives next to the old API, changes nothing in src/**.

namespace McProtoNet.Next;

/// <summary>
///     How <see cref="FrameWriter" /> decides whether to compress a frame body, relative to the
///     connection's compression threshold.
/// </summary>
/// <remarks>
///     The choice only has meaning while compression is enabled on the connection (a threshold
///     of zero or more). With compression disabled every frame goes out as a single length prefix
///     followed by the raw body, and this enum is ignored.
/// </remarks>
public enum FrameCompression
{
    /// <summary>
    ///     Follow the protocol honestly: compress the body when its length reaches the connection
    ///     threshold, send it uncompressed (with a zero size field) when it is below.
    /// </summary>
    Auto = 0,

    /// <summary>
    ///     Compress the body even when it is below the threshold. Useful for exercising a server's
    ///     decompression path with payloads a well-behaved client would never compress.
    /// </summary>
    Force = 1,

    /// <summary>
    ///     Never compress the body, even when it is above the threshold. The frame goes out with a
    ///     zero size field. Useful for testing how a server reacts to a "badly compressed packet".
    /// </summary>
    Never = 2,
}

/// <summary>
///     Per-frame overrides for <see cref="FrameWriter" />. The default value (<c>default</c>) means
///     honest behaviour: <see cref="FrameCompression.Auto" /> and no size lie.
/// </summary>
/// <remarks>
///     This is the low-level door for tools that probe foreign servers. It deliberately lets the
///     caller break the vanilla contract — compress below the threshold, refuse to compress above
///     it, or claim an uncompressed size that does not match the real body.
/// </remarks>
public readonly struct FrameOptions
{
    /// <summary>Gets the compression decision for this frame. Defaults to <see cref="FrameCompression.Auto" />.</summary>
    public FrameCompression Compression { get; init; }

    /// <summary>
    ///     Gets the uncompressed size to write into the frame's size field, or <c>null</c> to write
    ///     the real body length.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is a deliberate lie switch. In vanilla the size field must equal the real
    ///         uncompressed length of the body; a conforming server may reject or misbehave when it
    ///         does not. Setting this to a value that differs from the true length is exactly how you
    ///         test that behaviour on someone else's server.
    ///     </para>
    ///     <para>
    ///         The value is only written when the frame is actually compressed — that is, with
    ///         <see cref="FrameCompression.Force" />, or with <see cref="FrameCompression.Auto" /> on
    ///         a body at or above the threshold. On an uncompressed (passthrough) frame the size
    ///         field is always the mandatory zero, so to send a size lie you must make the frame
    ///         compress, typically with <see cref="FrameCompression.Force" />.
    ///     </para>
    /// </remarks>
    public int? ClaimedUncompressedSize { get; init; }
}
