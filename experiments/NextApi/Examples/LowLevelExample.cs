// McProtoNet.Next experiment — examples, BOTTOM floor: the doors a protocol-core
// developer needs when probing someone else's server. Nothing here touches src/**.
//
// Every method below is one trick, standing on its own:
//   ConnectRawAsync                    — bring your own TCP connection
//   SendHandshakeByHandAsync           — build a packet body by hand, send it by id
//   SendOversizedLengthAsync           — declare more bytes than you send
//   SendLyingCompressionEnvelopeAsync  — lie about the uncompressed size
//   InspectFramesAsync                 — read frames exactly as they arrive
//
// The typed floor (SendAsync/ReadTypedAsync) is deliberately absent: this floor exists
// for the cases where the protocol is what you are attacking, not what you are obeying.

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using McProtoNet.Serialization;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;

namespace McProtoNet.Next.Examples;

/// <summary>
///     Low-level recipes on <see cref="MinecraftClient" />: raw connection, hand-built
///     packets, malformed frames, and frame-level inspection.
/// </summary>
public static class LowLevelExample
{
    /// <summary>Frame length declared by <see cref="SendOversizedLengthAsync" />.</summary>
    public const int OversizedDeclaredLength = 100;

    /// <summary>Body bytes actually sent by <see cref="SendOversizedLengthAsync" />.</summary>
    public const int OversizedSentBytes = 10;

    /// <summary>Uncompressed size claimed by <see cref="SendLyingCompressionEnvelopeAsync" />.</summary>
    public const int LyingClaimedSize = 1_000_000;

    // The handshake "next state" the old sample uses for a login connection
    // (examples/MinimalBot/Program.cs).
    private const int LoginIntent = 2;

    // ------------------------------------------------------------------ (a) raw connect

    /// <summary>
    ///     Opens the TCP connection by hand and puts a client on top of the stream. The
    ///     transport does not insist on its own connect path: anything that is a readable and
    ///     writable <see cref="Stream" /> works — a socket, a proxy tunnel, a capture replay.
    /// </summary>
    /// <param name="host">Server host name or address.</param>
    /// <param name="port">Server port.</param>
    /// <param name="cancellationToken">Token that cancels the connect attempt.</param>
    /// <returns>
    ///     The TCP connection and the client on top of it. One owner, and it is the client:
    ///     <see cref="TcpClient.GetStream" /> hands out a stream that owns the socket, and the
    ///     client closes that stream on disposal. Dispose the client and the connection is gone.
    ///     The <see cref="TcpClient" /> comes back only so you can read its endpoints and options;
    ///     disposing it too is harmless AFTER the client, and wrong before it — killing the socket
    ///     under a live client faults <see cref="MinecraftClient.Completion" /> with an I/O error
    ///     that has nothing to do with the peer.
    /// </returns>
    public static async Task<(TcpClient Tcp, MinecraftClient Client)> ConnectRawAsync(
        string host, int port, CancellationToken cancellationToken = default)
    {
        var tcp = new TcpClient { NoDelay = true };
        try
        {
            await tcp.ConnectAsync(host, port, cancellationToken).ConfigureAwait(false);
            // Create, not ConnectAsync: the connection is already ours. Compression stays off
            // (threshold -1) until the server announces a threshold.
            MinecraftClient client = MinecraftClient.Create(tcp.GetStream());
            return (tcp, client);
        }
        catch
        {
            tcp.Dispose();
            throw;
        }
    }

    // ------------------------------------------------------------------ (b) packet by hand

    /// <summary>
    ///     Sends a handshake without the typed layer: the body is written field by field with
    ///     <see cref="MinecraftPrimitiveWriter" /> and handed to
    ///     <see cref="MinecraftClient.SendPacketAsync" />, which only prepends the VarInt id and
    ///     wraps the result in a frame.
    /// </summary>
    /// <param name="client">The client to send through.</param>
    /// <param name="host">Host string to put into the handshake.</param>
    /// <param name="port">Port to put into the handshake.</param>
    /// <param name="protocolVersion">Protocol version to announce, and the version whose wire id is used.</param>
    /// <param name="cancellationToken">Token that cancels the send.</param>
    /// <exception cref="NotSupportedException">The handshake packet does not exist on <paramref name="protocolVersion" />.</exception>
    public static async ValueTask SendHandshakeByHandAsync(
        MinecraftClient client, string host, int port, int protocolVersion,
        CancellationToken cancellationToken = default)
    {
        // The id still comes from the generated registry rather than from a magic number in
        // this file — the wire id is a protocol fact, the hand-built body is the point here.
        if (!HandshakeSb.SetProtocolPacket.TryGetPacketId(protocolVersion, out int id))
            throw new NotSupportedException($"No handshake packet id on protocol version {protocolVersion}.");

        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(protocolVersion);
        writer.WriteString(host);
        writer.WriteUnsignedShort((ushort)port);
        writer.WriteVarInt(LoginIntent);

        using MemoryOwner<byte> body = writer.GetWrittenMemory();
        await client.SendPacketAsync(id, body.Memory, cancellationToken).ConfigureAwait(false);
    }

    // ------------------------------------------------------------------ (c) oversized length

    /// <summary>
    ///     Sends a frame whose declared length is far larger than the body that follows it:
    ///     VarInt(<see cref="OversizedDeclaredLength" />) and only
    ///     <see cref="OversizedSentBytes" /> bytes of body.
    /// </summary>
    /// <param name="client">The client to send through.</param>
    /// <param name="cancellationToken">Token that cancels the send.</param>
    /// <remarks>
    ///     <para>
    ///         The bytes go out through <see cref="MinecraftClient.SendRawBytesAsync" />, which adds
    ///         no header of its own — that is exactly why the door exists. Any framing door would
    ///         write an honest length prefix and make this test impossible.
    ///     </para>
    ///     <para>
    ///         The peer now has to wait for the remaining
    ///         <see cref="OversizedDeclaredLength" /> − <see cref="OversizedSentBytes" /> = 90 bytes
    ///         before it can hand the frame to its packet layer. What it does while waiting — hold
    ///         the connection, time out, or buffer without limit — is the reaction under test.
    ///         Finish the frame with <see cref="SendMissingTailAsync" /> when you are done watching.
    ///     </para>
    ///     <para>
    ///         Precondition, the mirror image of (d): this probe assumes compression is OFF
    ///         (<see cref="MinecraftClient.CompressionThreshold" /> is −1) on both sides. With
    ///         compression on, a conforming peer reads the first body byte as the uncompressed-size
    ///         field of the envelope, so the probe would exercise the decompression path instead of
    ///         the length-waiting path it claims to test.
    ///     </para>
    /// </remarks>
    public static ValueTask SendOversizedLengthAsync(
        MinecraftClient client, CancellationToken cancellationToken = default)
    {
        // VarInt(100) is a single byte; below 0x80 the encoding is the value itself.
        byte[] wire = new byte[1 + OversizedSentBytes];
        wire[0] = OversizedDeclaredLength;
        FillPattern(wire.AsSpan(1), start: 0);
        return client.SendRawBytesAsync(wire, cancellationToken);
    }

    /// <summary>
    ///     Sends the bytes still missing from the frame that
    ///     <see cref="SendOversizedLengthAsync" /> started, so the peer can finally complete it.
    /// </summary>
    /// <param name="client">The client to send through.</param>
    /// <param name="cancellationToken">Token that cancels the send.</param>
    public static ValueTask SendMissingTailAsync(
        MinecraftClient client, CancellationToken cancellationToken = default)
    {
        byte[] tail = new byte[OversizedDeclaredLength - OversizedSentBytes];
        FillPattern(tail, start: OversizedSentBytes);
        return client.SendRawBytesAsync(tail, cancellationToken);
    }

    // ------------------------------------------------------------------ (d) lying envelope

    /// <summary>
    ///     The second way to lie: an honest frame length with a compression envelope that claims
    ///     <see cref="LyingClaimedSize" /> uncompressed bytes for a body of 24.
    /// </summary>
    /// <param name="client">The client to send through. Its <see cref="MinecraftClient.CompressionThreshold" /> must be 0 or more.</param>
    /// <param name="cancellationToken">Token that cancels the send.</param>
    /// <remarks>
    ///     <para>
    ///         <see cref="FrameCompression.Force" /> compresses a body that is below the threshold,
    ///         and <see cref="FrameOptions.ClaimedUncompressedSize" /> writes the size field the
    ///         caller asks for instead of the real one.
    ///     </para>
    ///     <para>
    ///         What this client defends against — precisely, because the sentence is worth getting
    ///         right: a receiver that trusts the claim hands the caller a megabyte-long buffer whose
    ///         tail is whatever the arena held before. This client also rents the claimed size
    ///         (<see cref="RawFrame.Decompress" /> rents first, bounded only by
    ///         <see cref="RawFrame.MaxFrameSize" /> — 8 MiB), so the memory cost of the lie is NOT
    ///         avoided; what it refuses is the result. The real output length is compared with the
    ///         claim and a mismatch throws instead of returning stale bytes. Budget for the
    ///         allocation, not against it.
    ///     </para>
    ///     <para>
    ///         With compression disabled the frame has no size field at all and this call sends an
    ///         ordinary frame — set the threshold first.
    ///     </para>
    /// </remarks>
    public static ValueTask SendLyingCompressionEnvelopeAsync(
        MinecraftClient client, CancellationToken cancellationToken = default)
    {
        byte[] payload = new byte[24];
        FillPattern(payload, start: 0);
        var options = new FrameOptions
        {
            Compression = FrameCompression.Force,
            ClaimedUncompressedSize = LyingClaimedSize,
        };
        return client.SendFrameAsync(payload, options, cancellationToken);
    }

    // ------------------------------------------------------------------ (e) frame inspection

    /// <summary>
    ///     Reads frames as they are on the wire and prints what each one declares. Garbage is data
    ///     here, not a fault: the frame door never unpacks a body for you. This method then asks
    ///     for it explicitly, one frame at a time — which is why the <c>try</c> can sit around that
    ///     one call.
    /// </summary>
    /// <param name="client">The client to read from.</param>
    /// <param name="maxFrames">How many frames to print before returning.</param>
    /// <param name="cancellationToken">Token that stops the enumeration.</param>
    /// <returns>The number of frames read.</returns>
    /// <remarks>
    ///     That placement is the whole lesson: a frame whose envelope lies fails exactly at the
    ///     point where its claim is checked, and the connection keeps running — the next frame is
    ///     read as usual. A <c>try</c> around the loop would have thrown the connection away with
    ///     the bad frame.
    /// </remarks>
    public static async Task<int> InspectFramesAsync(
        MinecraftClient client, int maxFrames, CancellationToken cancellationToken = default)
    {
        int seen = 0;
        await foreach (RawFrame frame in client.ReadFramesAsync(cancellationToken).ConfigureAwait(false))
        {
            Console.WriteLine(
                $"frame #{seen}: declared {frame.DeclaredLength} B, payload {frame.Payload.Length} B, " +
                $"size field {frame.Envelope.HasSizeField}, compressed {frame.IsCompressed}, " +
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
                    // The envelope lied or the body is not zlib. The loop lives on.
                    Console.WriteLine($"  bad envelope: {ex.Message}");
                }
            }

            if (++seen >= maxFrames)
                break;
        }

        return seen;
    }

    // ------------------------------------------------------------------ helpers

    /// <summary>
    ///     The exact bytes the oversized-length pair puts on the wire: the whole
    ///     <see cref="OversizedDeclaredLength" />-byte body a receiver must end up holding once
    ///     <see cref="SendOversizedLengthAsync" /> and <see cref="SendMissingTailAsync" /> have both
    ///     run. Published so a test can compare the delivered payload against it instead of
    ///     trusting its length — the two halves cross a segment boundary in the receive buffer,
    ///     and a slicing bug there yields the right count of wrong bytes.
    /// </summary>
    public static byte[] ExpectedOversizedFrameBody()
    {
        byte[] expected = new byte[OversizedDeclaredLength];
        FillPattern(expected, start: 0);
        return expected;
    }

    // A recognisable byte pattern, so a receiver can tell which part of a split frame it holds.
    private static void FillPattern(Span<byte> destination, int start)
    {
        for (int i = 0; i < destination.Length; i++)
            destination[i] = (byte)((start + i) * 31 + 7);
    }
}
