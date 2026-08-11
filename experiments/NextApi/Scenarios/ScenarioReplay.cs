// EXPERIMENT (McProtoNet.Next) — SCENARIO 5: record and replay traffic.
// Lives next to the old API, changes nothing in src/**.
//
// The task, literally:
//   (record)  wrap the Stream so every raw byte in BOTH directions is copied to a
//             file, while the client keeps working normally — MinecraftClient.Create(shadowStream).
//   (replay)  feed the recorded bytes back as a Stream and read them through
//             ReadFramesAsync / ReadPacketsAsync.
//
// The three questions this scenario had to answer, answered where they bite:
//   (1) Is Create(Stream) a sufficient entry point, or is a hook on the raw bytes
//       BEFORE encryption / AFTER decryption needed?
//   (2) Replaying an ENCRYPTED recording — there is nothing to decrypt it with (the
//       key was ephemeral). So must the DECRYPTED stream be recorded? Where is the tap?
//   (3) Is an IDuplexPipe entry enough instead of a Stream?
//
// Short answers, proven by the code below:
//   (1) Create(Stream) is a sufficient, fully transparent entry point for a WIRE-level
//       capture. The tap (RecordingStream) sits below the client, so it records exactly
//       what crosses the wire. But the encryption boundary lives INSIDE the client
//       (fill pump decrypts after Stream.ReadAsync; drain pump encrypts before
//       Stream.WriteAsync — MinecraftClient.cs:924-925, 966). The Stream therefore only
//       ever sees CIPHERTEXT once EnableEncryption ran. There is no public seam for the
//       plaintext bytes. => central CRACK, see below.
//   (2) A ciphertext capture is un-replayable once the ephemeral key is gone. To be
//       replayable the DECRYPTED stream must be recorded — and the only place those
//       bytes exist is between the pump cipher and the pipe, which no public API exposes.
//       => HARD CRACK, sketched in an #if false block below.
//   (3) There is no Create(IDuplexPipe). Replay is naturally a PipeReader over the
//       recorded bytes plus a throwaway PipeWriter, and the client already builds Pipes
//       internally — but the only public door is Create(Stream), so replay must dress the
//       recorded bytes up as a read-only Stream (ReplayStream). => friction, HARD CRACK
//       block below.

using McProtoNet.Net;            // InputPacket
using McProtoNet.Next.Demo;      // DuplexPipeStream (in-memory loopback, same assembly)

namespace McProtoNet.Next.Scenarios;

/// <summary>
///     Scenario 5: record a session's raw wire bytes through a transparent
///     <see cref="Stream" /> tap, then replay them back into a fresh
///     <see cref="MinecraftClient" /> and read them through both the frame door and the
///     packet door. Network-free: a <see cref="DuplexPipeStream" /> pair stands in for the
///     socket, exactly as the rest of the self-test does.
/// </summary>
/// <remarks>
///     Everything here is transport-level. The VarInt ids used in the demo (0x00, 0x01, …)
///     are opaque frame ids, NOT claims about any real protocol packet — record/replay does
///     not decode, it moves bytes. Where a real protocol fact would be needed it is a
///     parameter, never a literal.
/// </remarks>
public static class ScenarioReplay
{
    // ------------------------------------------------------------------ demonstration

    /// <summary>
    ///     Runs the whole scenario once over an in-memory loopback: record three plaintext
    ///     frames, then replay the capture twice — once as frames, once as packets — and
    ///     confirm the ids come back in order.
    /// </summary>
    /// <param name="cancellationToken">Token that cancels the run.</param>
    /// <returns>The ids read back on the packet-replay pass, in order.</returns>
    public static async Task<IReadOnlyList<int>> RunAsync(CancellationToken cancellationToken = default)
    {
        // Opaque transport ids — see the class remark. Not protocol facts.
        int[] sentIds = { 0x00, 0x01, 0x02 };

        // The "file" the raw bytes are copied to. A MemoryStream stands in for a FileStream:
        // in real use these two are `File.Create(path)`. Two sinks, one per direction, so the
        // fill pump (inbound) and the drain pump (outbound) never write the same Stream
        // concurrently — Stream is not thread-safe and the pumps run on two tasks.
        using var inboundCapture = new MemoryStream();
        using var outboundCapture = new MemoryStream();

        // ---- record ----------------------------------------------------------------
        // The client is built on the SHADOW stream. It cannot tell the tap is there; every
        // byte it reads or writes is mirrored into the capture on the way past.
        var (serverWire, clientWire) = DuplexPipeStream.CreatePair();
        await using (var server = MinecraftClient.Create(serverWire))
        await using (var client = MinecraftClient.Create(
            new RecordingStream(clientWire, inboundCapture, outboundCapture, leaveInnerOpen: false)))
        {
            // Compression OFF (threshold -1, the default) and encryption OFF: this is the ONLY
            // regime a Create(Stream) capture can faithfully replay — see the cracks below.

            // Drive the client's read loop in the background so its fill pump pulls every byte
            // (and the tap records it). The loop ends when the server closes (clean FIN).
            var received = new List<int>();
            Task consume = Task.Run(async () =>
            {
                await foreach (InputPacket packet in client.ReadPacketsAsync(cancellationToken).ConfigureAwait(false))
                    received.Add(packet.Id);
            }, cancellationToken);

            foreach (int id in sentIds)
                await server.SendPacketAsync(id, MakeBody(48), cancellationToken).ConfigureAwait(false);

            // Close the server end: the client sees a clean FIN, its read loop drains and ends,
            // and by then the fill pump has recorded every inbound byte.
            await server.DisposeAsync().ConfigureAwait(false);
            await consume.ConfigureAwait(false);

            if (received.Count != sentIds.Length)
                throw new InvalidOperationException(
                    $"record pass: read {received.Count} packet(s), expected {sentIds.Length}.");
        }

        // The capture is now a self-contained byte[] — the inbound half of the session, as it
        // crossed the wire. It can be replayed any number of times.
        byte[] recorded = inboundCapture.ToArray();

        // ---- replay pass A: as frames (ReadFramesAsync) ----------------------------
        int frameCount = 0;
        await using (var replay = MinecraftClient.Create(new ReplayStream(recorded)))
        {
            await foreach (RawFrame frame in replay.ReadFramesAsync(cancellationToken).ConfigureAwait(false))
            {
                // Frames come back exactly as recorded: DeclaredLength, envelope, payload window.
                _ = frame.DeclaredLength;
                frameCount++;
            }
        }
        if (frameCount != sentIds.Length)
            throw new InvalidOperationException(
                $"frame replay: read {frameCount} frame(s), expected {sentIds.Length}.");

        // ---- replay pass B: as packets (ReadPacketsAsync) --------------------------
        // A fresh ReplayStream + client: the single read cursor means one enumeration per
        // client, but the recording is just bytes, so replaying again is free.
        var replayedIds = new List<int>();
        await using (var replay = MinecraftClient.Create(new ReplayStream(recorded)))
        {
            // CRACK: the capture carries no compression-threshold metadata, so the replay
            //   client's threshold has to be set by hand to whatever it was at capture time.
            //   Here it was OFF, so nothing to set; a compressed capture would need the number
            //   supplied out of band — and a session that switched compression ON mid-stream
            //   (SetCompression) cannot be replayed by a single client at all, because one
            //   CompressionThreshold value can't follow the switch.
            //   IDEAL: the capture format records the threshold and the offset of each
            //   SetCompression/EnableEncryption transition, and replay honours them.
            // replay.CompressionThreshold = capturedThreshold;   // (no-op here: capture was uncompressed)

            await foreach (InputPacket packet in replay.ReadPacketsAsync(cancellationToken).ConfigureAwait(false))
                replayedIds.Add(packet.Id);
        }

        if (!replayedIds.SequenceEqual(sentIds))
            throw new InvalidOperationException(
                $"packet replay: ids [{string.Join(", ", replayedIds)}] did not match [{string.Join(", ", sentIds)}].");

        return replayedIds;
    }

    // ---------------------------------------------------------------------------------
    // What a Create(Stream) capture CANNOT do, spelled out as the calls we would want but
    // the surface does not offer. These blocks keep the project compiling while documenting
    // the missing APIs precisely.
    // ---------------------------------------------------------------------------------
#if false
    // HARD CRACK (question 1 + 2): no tap for the DECRYPTED byte stream.
    // The encryption boundary is private and internal to the client (fill pump decrypts
    // AFTER Stream.ReadAsync, drain pump encrypts BEFORE Stream.WriteAsync). The only
    // transparent tap — a Stream wrapper — therefore sits below encryption and can record
    // nothing but ciphertext. With the ephemeral key gone after login, that capture is
    // dead weight: ReplayStream would feed encrypted length-prefixes to the frame reader,
    // which rejects them as malformed lengths (ProtocolViolationException).
    // IDEAL: an opt-in plaintext tap at the pump<->pipe boundary, e.g.
    var client = MinecraftClient.Create(stream, new MinecraftClientOptions
    {
        InboundPlaintextTap  = inboundCapture,   // bytes AFTER decrypt on receive
        OutboundPlaintextTap = outboundCapture,  // bytes BEFORE encrypt on send
    });
    // — the replayable layer, captured transparently regardless of encryption.
#endif

#if false
    // HARD CRACK (question 3): no Create(IDuplexPipe) overload.
    // Replay wants to be a PipeReader over the recorded bytes plus a PipeWriter that goes
    // nowhere; the client already builds a receive/send Pipe pair internally. An IDuplexPipe
    // door would let replay skip the Stream costume (ReplayStream) entirely, and would let a
    // recorder tap the pipe boundary without subclassing Stream.
    var replay = MinecraftClient.Create(new ReplayDuplexPipe(recorded));
#endif

    // ------------------------------------------------------------------ RecordingStream

    /// <summary>
    ///     A transparent <see cref="Stream" /> tap: forwards every read and write to an inner
    ///     stream and mirrors the bytes into two capture sinks (inbound = bytes read from the
    ///     wire, outbound = bytes written to it). The client wrapped around it cannot tell it
    ///     is there.
    /// </summary>
    /// <remarks>
    ///     The capture is at the WIRE level. If the wrapped client enables encryption, both
    ///     sinks receive ciphertext — see the cracks at the top of the file. Inbound is
    ///     written only from the fill pump and outbound only from the drain pump, so the two
    ///     sinks are each touched by a single task; do not point both at the same Stream.
    /// </remarks>
    private sealed class RecordingStream : Stream
    {
        private readonly Stream _inner;
        private readonly Stream _inboundSink;
        private readonly Stream _outboundSink;
        private readonly bool _leaveInnerOpen;

        public RecordingStream(Stream inner, Stream inboundSink, Stream outboundSink, bool leaveInnerOpen)
        {
            _inner = inner;
            _inboundSink = inboundSink;
            _outboundSink = outboundSink;
            _leaveInnerOpen = leaveInnerOpen;
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;
        public override bool CanSeek => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int read = await _inner.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (read > 0)
                await _inboundSink.WriteAsync(buffer[..read], cancellationToken).ConfigureAwait(false);
            return read;
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (!buffer.IsEmpty)
                await _outboundSink.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            await _inner.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        }

        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);
        public override void Flush() => _inner.Flush();

        // Sync bridges — the pumps use the async doors, but Stream requires these.
        public override int Read(byte[] buffer, int offset, int count)
            => ReadAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();

        public override void Write(byte[] buffer, int offset, int count)
            => WriteAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            // Dispose only the wire; the capture sinks belong to the caller (the "file").
            if (disposing && !_leaveInnerOpen)
                _inner.Dispose();
            base.Dispose(disposing);
        }
    }

    // ------------------------------------------------------------------ ReplayStream

    /// <summary>
    ///     A read-only <see cref="Stream" /> over a recorded byte buffer: hands the bytes back
    ///     in order, then returns 0 (a clean FIN) so the frame layer ends its enumeration
    ///     softly at a frame boundary. Writes are swallowed — a replay has no peer to answer.
    /// </summary>
    private sealed class ReplayStream : Stream
    {
        private readonly ReadOnlyMemory<byte> _recorded;
        private int _position;

        public ReplayStream(ReadOnlyMemory<byte> recorded) => _recorded = recorded;

        public override bool CanRead => true;

        // CRACK: replay is read-only, but Create() rejects a stream unless CanWrite is true too
        //   (MinecraftClient.cs:213). So a replay Stream is forced to advertise a writable side
        //   and provide a no-op sink it will never use.
        //   IDEAL: a read-only replay entry — Create(PipeReader) / Create(IDuplexPipe with a
        //   null writer) — that does not demand a writable half the scenario has no use for.
        public override bool CanWrite => true;
        public override bool CanSeek => false;
        public override long Length => _recorded.Length;
        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return ValueTask.FromCanceled<int>(cancellationToken);
            int remaining = _recorded.Length - _position;
            if (remaining <= 0)
                return ValueTask.FromResult(0); // exhausted → clean FIN
            int count = Math.Min(remaining, buffer.Length);
            _recorded.Span.Slice(_position, count).CopyTo(buffer.Span);
            _position += count;
            return ValueTask.FromResult(count);
        }

        // Swallow: nothing the replayed client tries to send has anywhere to go.
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            => cancellationToken.IsCancellationRequested
                ? ValueTask.FromCanceled(cancellationToken)
                : ValueTask.CompletedTask;

        public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
            => ReadAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();

        public override void Write(byte[] buffer, int offset, int count) { }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }

    // ------------------------------------------------------------------ helpers

    private static byte[] MakeBody(int length)
    {
        byte[] body = new byte[length];
        for (int i = 0; i < body.Length; i++)
            body[i] = (byte)(i * 31 + 7);
        return body;
    }
}
