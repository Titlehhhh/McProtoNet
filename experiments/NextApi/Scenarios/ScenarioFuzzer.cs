// McProtoNet.Next experiment — SCENARIO 4: the crooked-frame fuzzer.
// The door a Paper-hacker walks through: feed a foreign server malformed frames and
// watch how it reacts. Nothing here touches src/**; the file compiles standing next to
// the old code (verified member-by-member against the experiment's own sources, not from
// memory). Cracks are marked inline: "// CRACK:" where the API forces ugliness but works,
// "// HARD CRACK:" in an #if false block where a door is missing outright.
//
// Five probes, each one trick, straight off the task:
//   (a) OversizedLength         — VarInt(N) then a body shorter than N            (SendRawBytesAsync)
//   (b) CompressionEnvelopeLie  — real compressed frame, lying ClaimedUncompressedSize (SendFrameAsync)
//   (c) GarbageBytes            — arbitrary bytes straight onto the socket        (SendRawBytesAsync)
//   (d) TruncatedPacketBody     — a valid packet id over a cut-off body           (SendPacketAsync)
//   (e) ZeroLengthFrame         — a frame whose declared length is 0              (SendFrameAsync)
//
// Every packet id below is a protocol FACT pulled from the generated registry
// (SetProtocolPacket / PingRequestPacket .TryGetPacketId), never a literal in this file.
// The protocol version is a parameter — the caller passes the version it is probing.

#nullable enable

using System;
using System.Buffers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Serialization;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using StatusSb = McProtoNet.Protocol.Packets.Status.Serverbound;

namespace McProtoNet.Next.Scenarios;

/// <summary>
///     A fuzzer of crooked wire frames against someone else's server. Each probe is a single
///     malformed send; <see cref="ObserveFramesAsync" /> reads the reaction as raw frames
///     without the loop ever dying on garbage.
/// </summary>
public static class ScenarioFuzzer
{
    // ------------------------------------------------------------------ (a) oversized length

    /// <summary>
    ///     Probe (a): declare more body than you send. Writes VarInt(<paramref name="declaredLength" />)
    ///     as the outer frame-length prefix, then only <paramref name="sentBytes" /> bytes of body, so
    ///     the peer waits for a tail that never comes.
    /// </summary>
    /// <remarks>
    ///     Precondition: compression OFF (<see cref="MinecraftClient.CompressionThreshold" /> == -1).
    ///     With compression on, the first body byte is read as the envelope's size field and the probe
    ///     tests the wrong path.
    /// </remarks>
    public static async ValueTask OversizedLengthAsync(
        MinecraftClient client, int declaredLength = 100, int sentBytes = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        // CRACK: to lie about the OUTER frame length there is no framing door — SendFrameAsync
        //        always writes an honest length prefix (== real payload length), and FrameOptions
        //        exposes only the INNER compression size (ClaimedUncompressedSize), never the outer
        //        one. So the whole frame is hand-assembled and pushed through the header-less raw
        //        door. Works, but the caller re-implements framing by hand.
        // IDEAL: a door that lets the outer length be chosen, e.g. FrameOptions
        //        { DeclaredLengthOverride = N } or SendFrameAsync(payload, declaredLength: N, ...).
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(declaredLength);            // the lie: the length prefix
        byte[] body = new byte[sentBytes < 0 ? 0 : sentBytes];
        FillPattern(body, start: 0);
        writer.WriteBuffer(body);                      // fewer bytes than declaredLength
        // await inside the using: SendRawBytesAsync must finish reading the pooled buffer before
        // GetWrittenMemory's MemoryOwner returns it to the pool.
        using MemoryOwner<byte> wire = writer.GetWrittenMemory();
        await client.SendRawBytesAsync(wire.Memory, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Finishes the frame that <see cref="OversizedLengthAsync" /> left dangling, so the peer can
    ///     complete it instead of timing out — useful for measuring the server's patience window.
    /// </summary>
    public static ValueTask OversizedLengthTailAsync(
        MinecraftClient client, int declaredLength = 100, int sentBytes = 10,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        int missing = declaredLength - sentBytes;
        byte[] tail = new byte[missing < 0 ? 0 : missing];
        FillPattern(tail, start: sentBytes);
        return client.SendRawBytesAsync(tail, cancellationToken);
    }

    // ------------------------------------------------------------------ (b) compression-envelope lie

    /// <summary>
    ///     Probe (b): a genuinely compressed frame whose envelope claims
    ///     <paramref name="claimedUncompressedSize" /> uncompressed bytes for a tiny real body — the
    ///     "badly compressed packet" a conforming server must reject.
    /// </summary>
    /// <remarks>
    ///     Precondition: compression MUST be enabled first
    ///     (<see cref="MinecraftClient.CompressionThreshold" /> &gt;= 0). With it off the frame carries
    ///     no size field at all and <see cref="FrameCompression.Force" /> plus
    ///     <see cref="FrameOptions.ClaimedUncompressedSize" /> are ignored — the lie silently
    ///     evaporates. This method sets the threshold to 0 to make the lie land.
    /// </remarks>
    public static ValueTask CompressionEnvelopeLieAsync(
        MinecraftClient client, int claimedUncompressedSize = 1_000_000, int realBodyBytes = 24,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        // Not a crack: enabling compression is a real protocol precondition, not an API defect —
        // but it is the one silent trap of this probe, so it is set explicitly rather than assumed.
        if (client.CompressionThreshold < 0)
            client.CompressionThreshold = 0;

        byte[] payload = new byte[realBodyBytes];
        FillPattern(payload, start: 0);

        // First-class: the envelope lie is exactly what FrameOptions was built for.
        //   Force               -> compress a body a well-behaved client would send plain
        //   ClaimedUncompressedSize -> write a size field that does not match the real length
        var options = new FrameOptions
        {
            Compression = FrameCompression.Force,
            ClaimedUncompressedSize = claimedUncompressedSize,
        };
        return client.SendFrameAsync(payload, options, cancellationToken);
    }

    // ------------------------------------------------------------------ (c) arbitrary bytes

    /// <summary>
    ///     Probe (c): dump arbitrary bytes onto the socket with no framing of any kind — no length
    ///     prefix, no envelope, whatever <paramref name="bytes" /> holds goes out verbatim.
    /// </summary>
    /// <remarks>
    ///     <see cref="MinecraftClient.SendRawBytesAsync" /> is the "past all framing" door and is the
    ///     right tool here. One caveat worth knowing while fuzzing: if
    ///     <see cref="MinecraftClient.EnableEncryption(ReadOnlySpan{byte})" /> has been called, these
    ///     bytes are still run through the outbound cipher — see the CRACK note on
    ///     <see cref="GarbageBytesRandomAsync" />. Fuzz before enabling encryption to put truly raw
    ///     bytes on the wire.
    /// </remarks>
    public static ValueTask GarbageBytesAsync(
        MinecraftClient client, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        return client.SendRawBytesAsync(bytes, cancellationToken);
    }

    /// <summary>Convenience form of probe (c): a block of pseudo-random garbage.</summary>
    public static ValueTask GarbageBytesRandomAsync(
        MinecraftClient client, int count = 512, int seed = 1337, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        byte[] junk = new byte[count];
        new Random(seed).NextBytes(junk);

        // CRACK: there is no door that puts bytes on the wire AFTER the framing layer but BEFORE
        //        (i.e. bypassing) the encryption pump. SendRawBytesAsync skips varint framing but
        //        still encrypts once EnableEncryption ran, so "inject raw plaintext onto an already
        //        encrypted channel" is not expressible. Harmless for the usual fuzz (do it on a
        //        fresh, unencrypted connection), hence a workaround exists: just don't enable
        //        encryption before this probe.
        // IDEAL: an overload / flag such as SendRawBytesAsync(bytes, bypassCipher: true) for the
        //        rare post-encryption raw-injection case.
        return client.SendRawBytesAsync(junk, cancellationToken);
    }

    // ------------------------------------------------------------------ (d) valid id, truncated body

    /// <summary>
    ///     Probe (d): a well-formed frame carrying a VALID packet id but a body cut short of what the
    ///     packet's decoder expects. Uses the Handshake <c>SetProtocol</c> packet (four fields:
    ///     ProtocolVersion, ServerHost, ServerPort, NextState) and sends only the first field, so the
    ///     server's decoder runs off the end of the body.
    /// </summary>
    /// <remarks>
    ///     The frame itself is honest — <see cref="MinecraftClient.SendPacketAsync" /> writes a
    ///     correct outer length and the real VarInt id in front. Only the packet CONTENT is
    ///     truncated. The id is resolved from the generated registry, never hardcoded.
    /// </remarks>
    /// <exception cref="NotSupportedException">The handshake packet has no id on <paramref name="protocolVersion" />.</exception>
    public static async ValueTask TruncatedHandshakeBodyAsync(
        MinecraftClient client, int protocolVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (!HandshakeSb.SetProtocolPacket.TryGetPacketId(protocolVersion, out int id))
            throw new NotSupportedException($"No SetProtocol packet id on protocol version {protocolVersion}.");

        // Write ONLY ProtocolVersion, then stop — ServerHost / ServerPort / NextState are missing.
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(protocolVersion);
        // await inside the using: keep the pooled body alive until SendPacketAsync has copied it.
        using MemoryOwner<byte> truncatedBody = writer.GetWrittenMemory();
        await client.SendPacketAsync(id, truncatedBody.Memory, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Probe (d), second flavour: the Status <c>PingRequest</c> packet expects an 8-byte long;
    ///     this sends a valid id over only <paramref name="bodyBytes" /> bytes. Reach the status phase
    ///     first (handshake with next-state 1, then a status request).
    /// </summary>
    /// <exception cref="NotSupportedException">The ping packet has no id on <paramref name="protocolVersion" />.</exception>
    public static ValueTask TruncatedPingBodyAsync(
        MinecraftClient client, int protocolVersion, int bodyBytes = 3,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (!StatusSb.PingRequestPacket.TryGetPacketId(protocolVersion, out int id))
            throw new NotSupportedException($"No PingRequest packet id on protocol version {protocolVersion}.");

        byte[] stub = new byte[bodyBytes];   // fewer than the 8 bytes ReadSignedLong will demand
        FillPattern(stub, start: 0);
        return client.SendPacketAsync(id, stub, cancellationToken);
    }

    // ------------------------------------------------------------------ (e) zero-length frame

    /// <summary>
    ///     Probe (e): a frame whose declared length is 0 — a single <c>0x00</c> byte on the wire.
    /// </summary>
    /// <remarks>
    ///     Sent with compression OFF so the frame is a bare length prefix of zero and nothing else.
    ///     The method forces <see cref="MinecraftClient.CompressionThreshold" /> to -1: with
    ///     compression enabled an empty payload still carries a size field, so the frame would be
    ///     <c>VarInt(len) VarInt(0)</c> (a passthrough), not the bare zero this probe wants.
    /// </remarks>
    public static ValueTask ZeroLengthFrameAsync(
        MinecraftClient client, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (client.CompressionThreshold >= 0)
            client.CompressionThreshold = -1;

        // SendFrameAsync accepts an empty payload; with compression off it writes VarInt(0) and no
        // body — the bare zero-length frame. (Equivalent raw form: SendRawBytesAsync(new byte[]{0}).)
        return client.SendFrameAsync(ReadOnlyMemory<byte>.Empty, default, cancellationToken);
    }

    // ------------------------------------------------------------------ observe the reaction

    /// <summary>
    ///     Reads up to <paramref name="maxFrames" /> frames of the server's reaction as raw wire
    ///     frames. Garbage is data, not a fault: a lying envelope fails only where its claim is
    ///     checked (inside <see cref="RawFrame.Decompress" />), and the loop reads on.
    /// </summary>
    /// <returns>The number of frames observed before the cap or a clean close.</returns>
    public static async Task<int> ObserveFramesAsync(
        MinecraftClient client, int maxFrames = 16, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        int seen = 0;
        await foreach (RawFrame frame in client.ReadFramesAsync(cancellationToken).ConfigureAwait(false))
        {
            Console.WriteLine(
                $"reply #{seen}: declared {frame.DeclaredLength} B, payload {frame.Payload.Length} B, " +
                $"sizeField {frame.Envelope.HasSizeField}, compressed {frame.IsCompressed}, " +
                $"claimed {frame.Envelope.ClaimedUncompressedSize}");

            if (frame.IsCompressed)
            {
                try
                {
                    using IMemoryOwner<byte> body = frame.Decompress();
                    Console.WriteLine($"  body: {body.Memory.Length} B");
                }
                catch (ProtocolViolationException ex)
                {
                    Console.WriteLine($"  bad envelope, loop survives: {ex.Message}");
                }
            }

            if (++seen >= maxFrames)
                break;
        }

        return seen;
    }

    // ------------------------------------------------------------------ orchestration

    /// <summary>
    ///     Runs the whole battery against a live target: connect, start reading replies, fire each
    ///     probe with a small pause between them. Nothing is committed and nothing runs unless a
    ///     caller invokes it — long-running against a real server, so drive it from a visible console.
    /// </summary>
    public static async Task RunAsync(
        string host, int port, int protocolVersion, CancellationToken cancellationToken = default)
    {
        await using MinecraftClient client =
            await MinecraftClient.ConnectAsync(host, port, options: null, cancellationToken: cancellationToken).ConfigureAwait(false);

        Task<int> reader = ObserveFramesAsync(client, maxFrames: 32, cancellationToken);

        // (a) oversized length — compression is off on a fresh connection, the probe's precondition.
        await OversizedLengthAsync(client, cancellationToken: cancellationToken).ConfigureAwait(false);
        await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken).ConfigureAwait(false);
        await OversizedLengthTailAsync(client, cancellationToken: cancellationToken).ConfigureAwait(false);

        // (c) raw garbage.
        await GarbageBytesRandomAsync(client, cancellationToken: cancellationToken).ConfigureAwait(false);

        // (d) valid id, truncated body.
        await TruncatedHandshakeBodyAsync(client, protocolVersion, cancellationToken).ConfigureAwait(false);

        // (e) zero-length frame (forces compression off).
        await ZeroLengthFrameAsync(client, cancellationToken).ConfigureAwait(false);

        // (b) envelope lie — enables compression, so it runs last: it changes the connection's
        //     compression state and the raw-frame probes above assume it is off.
        await CompressionEnvelopeLieAsync(client, cancellationToken: cancellationToken).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        int replies = await reader.ConfigureAwait(false);
        Console.WriteLine($"observed {replies} reply frame(s)");
    }

#if false
    // HARD CRACK: a genuinely zlib-compressed frame whose OUTER length prefix lies.
    // FrameOptions can lie about the INNER envelope (ClaimedUncompressedSize), but the OUTER
    // frame length is always computed honestly by FrameWriter. To combine a real compression
    // envelope with a forged outer length there is no door: SendFrameAsync writes the honest
    // outer length, and SendRawBytesAsync has no compression helper (the zlib compressor is not
    // on the NextApi surface — only ZlibDecompressorHeapAlloc is, via RawFrame.Decompress). The
    // only escape is to compress the body by hand and re-emit the whole framing, which defeats
    // the point of having doors. The call one WANTS:
    await client.SendFrameAsync(
        payload,
        new FrameOptions
        {
            Compression = FrameCompression.Force,
            ClaimedUncompressedSize = 1_000_000,
            DeclaredLengthOverride = 5,   // <-- does not exist: no outer-length lie for a compressed frame
        },
        cancellationToken);
#endif

    // ------------------------------------------------------------------ helpers

    // A recognisable byte pattern so a receiver can tell which slice of a split frame it holds.
    private static void FillPattern(Span<byte> destination, int start)
    {
        for (int i = 0; i < destination.Length; i++)
            destination[i] = (byte)((start + i) * 31 + 7);
    }
}
