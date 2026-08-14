// EXPERIMENT (McProtoNet.Next) — self-test without a network. Two MinecraftClient
// instances talk over a pair of in-memory duplex streams (one Pipe per direction).
// Run: dotnet run --project McProtoNet/experiments/NextApi
//
// Checks:
//   (a) packet round-trip through SendPacketAsync / ReadPacketsAsync;
//   (b) a Force-compressed frame below the threshold and a frame with a lying
//       ClaimedUncompressedSize — the receiver sees both via ReadFramesAsync and
//       the read loop survives (Decompress throws, the loop does not);
//   (c) enabling AES/CFB8 encryption mid-stream on both sides, then a packet;
//   (d) a >128 KiB packet through encryption — the deadlock trap from the audit
//       (a whole frame must buffer in the receive pipe without pausing the fill pump);
//   (e) receive backpressure — a peer that floods a client which never reads must
//       stall at a bounded buffer, not grow memory without limit (hardening §1 DoS);
//   (f)/(g) the low-level examples keep their promises (Examples/ExamplesSelfCheck.cs):
//       a lying compression envelope is visible and survivable, and an oversized
//       declared length makes the receiver wait for the bytes it was promised.

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using McProtoNet.Net;
using McProtoNet.Next.Examples;

namespace McProtoNet.Next.Demo;

/// <summary>Entry point of the NextApi self-test. Prints OK/FAIL per check.</summary>
internal static class SelfTest
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(30);

    public static async Task<int> Main(string[] args)
    {
        // Замеры по требованиям (docs/design/transport-requirements-2026-08-10.md):
        // dotnet run --project McProtoNet/experiments/NextApi -- measure
        if (args.Length > 0 && args[0] == "measure")
        {
            await Measurements.ConnectionCost.RunAsync().ConfigureAwait(false);
            Measurements.SendCost.Run();
            return 0;
        }

        if (args.Length > 0 && args[0] == "wire")
        {
            await Wire.WireWriterDemo.RunAsync().ConfigureAwait(false);
            return 0;
        }

        // Замер шифра по вопросу Д5 (docs/design/transport-hardening-plan-2026-08-10.md):
        // dotnet run --project McProtoNet/experiments/NextApi -- cipher
        if (args.Length > 0 && args[0] == "cipher")
        {
            Measurements.CipherCost.Run();
            return 0;
        }

        // The bot walk against a real server — the one claim the network-free checks below
        // cannot make: dotnet run --project McProtoNet/experiments/NextApi -- live [host] [port] [name] [pv]
        if (args.Length > 0 && args[0] == "live")
        {
            return await LiveBotRun
                .RunAsync(Text(args, 1, "127.0.0.1"), Number(args, 2, 25566), Text(args, 3, "NextBot"), Number(args, 4, 772))
                .ConfigureAwait(false);
        }

        // Cipher against a real server, no Mojang account (see Examples/LiveCryptoProbe.cs):
        // dotnet run --project McProtoNet/experiments/NextApi -- login-crypto [host] [port] [name] [pv]
        // Exit codes: 0 = cipher proven, 1 = cipher failure, 2 = server is in offline mode,
        // 3 = inconclusive (the run never reached the cipher question — no server on the port,
        // a socket that died during plaintext login, a refusal before the encryption request).
        if (args.Length > 0 && args[0] == "login-crypto")
        {
            return await LiveCryptoProbe
                .RunAsync(Text(args, 1, "127.0.0.1"), Number(args, 2, 25565), Text(args, 3, "NextBot"), Number(args, 4, 772))
                .ConfigureAwait(false);
        }

        int failures = 0;
        failures += await RunAsync("(a) packet round-trip", TestPacketRoundTripAsync) ? 0 : 1;
        failures += await RunAsync("(b) force-compression + size lie via ReadFramesAsync", TestFrameLiesAsync) ? 0 : 1;
        failures += await RunAsync("(c) mid-stream encryption on both sides", TestMidStreamEncryptionAsync) ? 0 : 1;
        failures += await RunAsync("(d) >128 KiB packet through encryption", TestBigEncryptedPacketAsync) ? 0 : 1;
        failures += await RunAsync("(e) receive backpressure bounds an unread flood", TestReceiveBackpressureAsync) ? 0 : 1;
        failures += await RunAsync("(f) example: lying compression envelope is visible, loop survives",
            ExamplesSelfCheck.CheckLyingEnvelopeVisibleAsync) ? 0 : 1;
        failures += await RunAsync("(g) example: oversized declared length makes the receiver wait",
            ExamplesSelfCheck.CheckOversizedLengthWaitsAsync) ? 0 : 1;
        failures += await RunAsync("(h) AES/CFB8 streamed in any chunking matches the one-shot oracle (both directions)",
            CipherStreamingCheck.RunAsync) ? 0 : 1;
        failures += await RunAsync("(j) a mid-batch break leaves buffered frames readable",
            TestInterruptedReadLoopAsync) ? 0 : 1;
        // (i) is the only check that touches the typed floor: TryDecode -> IPacket stream ->
        // switch over packet classes -> SendAsync through the cached writer, over the same
        // in-memory duplex pair. Checks (a)-(h) all stop at the transport, so without this one
        // nothing here would notice the typed floor breaking. It prints its own line.
        failures += await Scenarios.ScenarioBot.RunAsync().ConfigureAwait(false) ? 0 : 1;

        Console.WriteLine(failures == 0 ? "ALL OK" : $"{failures} check(s) FAILED");
        return failures == 0 ? 0 : 1;
    }

    // Positional arguments of the live probes: absent or empty means "keep the default".
    private static string Text(string[] args, int index, string fallback) =>
        args.Length > index && args[index].Length > 0 ? args[index] : fallback;

    private static int Number(string[] args, int index, int fallback) =>
        args.Length > index && int.TryParse(args[index], out int value) ? value : fallback;

    private static async Task<bool> RunAsync(string name, Func<CancellationToken, Task> test)
    {
        using var cts = new CancellationTokenSource(TestTimeout);
        try
        {
            await test(cts.Token).WaitAsync(TestTimeout).ConfigureAwait(false);
            Console.WriteLine($"OK   {name}");
            return true;
        }
        catch (TimeoutException)
        {
            Console.WriteLine($"FAIL {name}: timed out after {TestTimeout} (deadlock?)");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    // ------------------------------------------------------------------ (a)

    private static async Task TestPacketRoundTripAsync(CancellationToken ct)
    {
        var (streamA, streamB) = DuplexPipeStream.CreatePair();
        await using var a = MinecraftClient.Create(streamA);
        await using var b = MinecraftClient.Create(streamB);

        byte[] body = MakeBody(100);
        await a.SendPacketAsync(0x2A, body, ct);

        var (id, received) = await ReadOnePacketAsync(b, ct);
        Check(id == 0x2A, $"expected id 0x2A, got 0x{id:X}");
        Check(received.AsSpan().SequenceEqual(body), "payload A->B does not match");

        // Echo it back the other way.
        await b.SendPacketAsync(0x2A, received, ct);
        var (idBack, receivedBack) = await ReadOnePacketAsync(a, ct);
        Check(idBack == 0x2A, $"echo: expected id 0x2A, got 0x{idBack:X}");
        Check(receivedBack.AsSpan().SequenceEqual(body), "payload B->A does not match");
    }

    // ------------------------------------------------------------------ (b)

    private static async Task TestFrameLiesAsync(CancellationToken ct)
    {
        var (streamA, streamB) = DuplexPipeStream.CreatePair();
        await using var a = MinecraftClient.Create(streamA);
        await using var b = MinecraftClient.Create(streamB);
        a.CompressionThreshold = 128;
        b.CompressionThreshold = 128;

        const int lieSize = 1_000_000;
        byte[] small = MakeBody(20);   // far below the threshold
        byte[] plain = MakeBody(24);

        // Frame 1: forced compression below the threshold, honest size.
        await a.SendFrameAsync(small, new FrameOptions { Compression = FrameCompression.Force }, ct);
        // Frame 2: forced compression with a lying uncompressed size.
        await a.SendFrameAsync(small,
            new FrameOptions { Compression = FrameCompression.Force, ClaimedUncompressedSize = lieSize }, ct);
        // Frame 3: honest passthrough — proves the read loop survived the lie.
        await a.SendFrameAsync(plain, default, ct);

        int index = 0;
        await foreach (RawFrame frame in b.ReadFramesAsync(ct))
        {
            switch (index)
            {
                case 0:
                    Check(frame.IsCompressed, "frame 1: expected a compressed envelope (Force)");
                    using (var owner = frame.Decompress())
                        Check(owner.Memory.Span.SequenceEqual(small), "frame 1: decompressed payload mismatch");
                    break;

                case 1:
                    Check(frame.IsCompressed, "frame 2: expected a compressed envelope (Force)");
                    Check(frame.Envelope.ClaimedUncompressedSize == lieSize,
                        $"frame 2: claimed size {frame.Envelope.ClaimedUncompressedSize}, expected the lie {lieSize}");
                    bool threw = false;
                    try
                    {
                        using var owner = frame.Decompress();
                    }
                    catch (ProtocolViolationException)
                    {
                        threw = true; // the lie is caught exactly here, not in the read loop
                    }
                    Check(threw, "frame 2: Decompress accepted a lying claimed size");
                    break;

                case 2:
                    Check(frame.Envelope.HasSizeField && !frame.IsCompressed,
                        "frame 3: expected a passthrough envelope");
                    Check(frame.Payload.ToArray().AsSpan().SequenceEqual(plain), "frame 3: payload mismatch");
                    break;
            }

            if (++index == 3)
                break;
        }

        Check(index == 3, $"read {index} frame(s), expected 3 — the loop died on the lie");
    }

    // ------------------------------------------------------------------ (c)

    private static async Task TestMidStreamEncryptionAsync(CancellationToken ct)
    {
        var (streamA, streamB) = DuplexPipeStream.CreatePair();
        await using var a = MinecraftClient.Create(streamA);
        await using var b = MinecraftClient.Create(streamB);

        // Plaintext exchange first, fully drained in both directions, so the wire is
        // quiet at the moment encryption switches on (the protocol's own guarantee).
        byte[] hello = MakeBody(64);
        await a.SendPacketAsync(0x01, hello, ct);
        var (_, gotHello) = await ReadOnePacketAsync(b, ct);
        Check(gotHello.AsSpan().SequenceEqual(hello), "plaintext A->B mismatch");
        await b.SendPacketAsync(0x02, hello, ct);
        _ = await ReadOnePacketAsync(a, ct);

        byte[] secret = MakeBody(16);
        a.EnableEncryption(secret);
        b.EnableEncryption(secret);
        Check(a.IsEncrypted && b.IsEncrypted, "IsEncrypted must be true on both sides");

        byte[] after = MakeBody(200);
        await a.SendPacketAsync(0x03, after, ct);
        var (idAb, gotAb) = await ReadOnePacketAsync(b, ct);
        Check(idAb == 0x03 && gotAb.AsSpan().SequenceEqual(after), "encrypted A->B packet mismatch");

        await b.SendPacketAsync(0x04, after, ct);
        var (idBa, gotBa) = await ReadOnePacketAsync(a, ct);
        Check(idBa == 0x04 && gotBa.AsSpan().SequenceEqual(after), "encrypted B->A packet mismatch");
    }

    // ------------------------------------------------------------------ (d)

    private static async Task TestBigEncryptedPacketAsync(CancellationToken ct)
    {
        var (streamA, streamB) = DuplexPipeStream.CreatePair();
        await using var a = MinecraftClient.Create(streamA);
        await using var b = MinecraftClient.Create(streamB);
        a.CompressionThreshold = 256;
        b.CompressionThreshold = 256;

        byte[] secret = MakeBody(16);
        a.EnableEncryption(secret);
        b.EnableEncryption(secret);

        // Random bytes barely compress, so the frame stays >128 KiB on the wire — the
        // audit's trap: the whole frame must buffer in the receive pipe while the frame
        // reader consumes nothing.
        byte[] big = new byte[200_000];
        new Random(42).NextBytes(big);

        Task<(int Id, byte[] Body)> readTask = Task.Run(() => ReadOnePacketAsync(b, ct), ct);
        await a.SendPacketAsync(0x55, big, ct);
        var (id, got) = await readTask;

        Check(id == 0x55, $"expected id 0x55, got 0x{id:X}");
        Check(got.Length == big.Length, $"received {got.Length} bytes, expected {big.Length}");
        Check(got.AsSpan().SequenceEqual(big), "big payload mismatch after encryption round-trip");
    }

    // ------------------------------------------------------------------ (e)

    private static async Task TestReceiveBackpressureAsync(CancellationToken ct)
    {
        // A hostile fast server against a client that never reads: the flood must stall at
        // a bounded buffer (receive-pipe pause threshold + the wire's own buffer), never
        // grow without limit. Before the fix (pauseWriterThreshold: 0) this wrote forever.
        var (streamA, streamB) = DuplexPipeStream.CreatePair();
        await using var client = MinecraftClient.Create(streamB);
        // No enumeration is started on `client` — its fill pump runs, the consumer never reads.

        // The content does not matter (nothing parses it before the frame floor is pulled);
        // 64 KiB chunks flood the wire from the test side.
        byte[] chunk = new byte[64 * 1024];
        new Random(7).NextBytes(chunk);

        // Bound: 2 × MaxFrameSize pause threshold (16 MiB) + wire pipe slack. Give ×2 headroom.
        const long ceiling = 64L * 1024 * 1024;
        long written = 0;
        bool stalled = false;
        while (written < ceiling)
        {
            Task write = streamA.WriteAsync(chunk, ct).AsTask();
            Task first = await Task.WhenAny(write, Task.Delay(TimeSpan.FromSeconds(2), ct)).ConfigureAwait(false);
            if (first != write)
            {
                stalled = true; // backpressure reached the wire — the flood is bounded
                break;
            }
            await write.ConfigureAwait(false);
            written += chunk.Length;
        }

        Check(stalled, $"wrote {written} bytes into a client nobody reads and never stalled — receive backpressure is off");
    }

    // ------------------------------------------------------------------ helpers

    private static async Task<(int Id, byte[] Body)> ReadOnePacketAsync(MinecraftClient client, CancellationToken ct)
    {
        await foreach (InputPacket packet in client.ReadPacketsAsync(ct))
        {
            // The data window dies on the next read — copy before leaving the loop.
            return (packet.Id, packet.Data.ToArray());
        }

        throw new InvalidOperationException("The stream ended before a packet arrived.");
    }

    // ------------------------------------------------------------------ (j)

    // The skeptic's objection to moving the transport into src/ (queue task 46): a consumer
    // that walks away in the middle of a batch. The reader marks the whole read buffer
    // examined but only the handed-out frames consumed, so a frame already sitting in the
    // pipe can stay invisible until the wire delivers another byte. Both frames go in with a
    // single flush on purpose — that is the only arrangement where the two positions differ.
    private static async Task TestInterruptedReadLoopAsync(CancellationToken ct)
    {
        var pipe = new Pipe();
        var reader = new FrameReader(pipe.Reader);

        byte[] first = MakeBody(8);
        byte[] second = MakeBody(12);
        WriteUncompressedFrame(pipe.Writer, first);
        WriteUncompressedFrame(pipe.Writer, second);
        await pipe.Writer.FlushAsync(ct).ConfigureAwait(false);

        int seen = 0;
        await foreach (RawFrame frame in reader.ReadFramesAsync(ct).ConfigureAwait(false))
        {
            Check(frame.Payload.ToArray().AsSpan().SequenceEqual(first), "first pass: wrong payload");
            seen++;
            break; // leave mid-batch, with the second frame still buffered
        }

        Check(seen == 1, "first pass yielded no frame at all");

        // Not one new byte is written from here on: the second frame must come out of what the
        // pipe already holds. A wait here is the bug, so cap it well below the check timeout.
        using var idle = CancellationTokenSource.CreateLinkedTokenSource(ct);
        idle.CancelAfter(TimeSpan.FromSeconds(3));
        try
        {
            await foreach (RawFrame frame in reader.ReadFramesAsync(idle.Token).ConfigureAwait(false))
            {
                Check(frame.Payload.ToArray().AsSpan().SequenceEqual(second), "second pass: wrong payload");
                seen++;
                break;
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                "the second frame was already buffered, but the reader waited for new bytes from the wire");
        }

        Check(seen == 2, "the buffered second frame never arrived");
    }

    private static void WriteUncompressedFrame(PipeWriter writer, ReadOnlySpan<byte> body)
    {
        Span<byte> span = writer.GetSpan(5 + body.Length);
        int header = 0;
        int remaining = body.Length;
        do
        {
            byte piece = (byte)(remaining & 0x7F);
            remaining >>= 7;
            if (remaining != 0)
                piece |= 0x80;
            span[header++] = piece;
        } while (remaining != 0);

        body.CopyTo(span[header..]);
        writer.Advance(header + body.Length);
    }

    private static byte[] MakeBody(int length)
    {
        byte[] body = new byte[length];
        for (int i = 0; i < body.Length; i++)
            body[i] = (byte)(i * 31 + 7);
        return body;
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}

/// <summary>
/// In-memory bidirectional stream for the self-test: one <see cref="Pipe" /> per direction.
/// Disposing an endpoint completes its outbound pipe, which the peer observes as a clean FIN.
/// </summary>
internal sealed class DuplexPipeStream : Stream
{
    private readonly PipeReader _input;
    private readonly PipeWriter _output;
    private volatile bool _disposed;

    private DuplexPipeStream(PipeReader input, PipeWriter output)
    {
        _input = input;
        _output = output;
    }

    /// <summary>Creates two connected endpoints: everything written to one is read from the other.</summary>
    public static (DuplexPipeStream A, DuplexPipeStream B) CreatePair()
    {
        var options = new PipeOptions(useSynchronizationContext: false);
        var aToB = new Pipe(options);
        var bToA = new Pipe(options);
        return (new DuplexPipeStream(bToA.Reader, aToB.Writer),
                new DuplexPipeStream(aToB.Reader, bToA.Writer));
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
        ReadResult result = await _input.ReadAsync(cancellationToken).ConfigureAwait(false);
        if (result.IsCanceled)
        {
            cancellationToken.ThrowIfCancellationRequested();
            throw new ObjectDisposedException(nameof(DuplexPipeStream));
        }

        ReadOnlySequence<byte> sequence = result.Buffer;
        if (sequence.IsEmpty)
        {
            _input.AdvanceTo(sequence.End);
            return 0; // clean FIN from the peer
        }

        int count = (int)Math.Min(sequence.Length, buffer.Length);
        ReadOnlySequence<byte> slice = sequence.Slice(0, count);
        slice.CopyTo(buffer.Span[..count]);
        _input.AdvanceTo(slice.End);
        return count;
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        FlushResult result = await _output.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        if (result.IsCompleted)
            throw new IOException("The peer closed the connection.");
    }

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
        => ReadAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();

    public override void Write(byte[] buffer, int offset, int count)
        => WriteAsync(buffer.AsMemory(offset, count)).AsTask().GetAwaiter().GetResult();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            _output.Complete();          // peer sees a clean FIN
            _input.CancelPendingRead();  // wake our own pending read; it reports disposal
        }

        base.Dispose(disposing);
    }
}
