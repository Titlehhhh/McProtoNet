// McProtoNet.Next experiment — examples, proof. The low-level examples make claims about
// how a peer reacts; these checks make the claims testable without a network, over the same
// in-memory duplex stream pair the rest of the self-test uses (Demo/SelfTest.cs).
//
// Checks (wired into SelfTest.Main as (f) and (g)):
//   (f) a frame whose compression envelope lies about the uncompressed size is VISIBLE to the
//       receiver, the lie is caught inside Decompress, and the read loop survives it;
//   (g) a frame that declares more bytes than were sent does not surface at all — the receiver
//       waits. The evidence is a 1.5 s window with nothing in it; then the missing 90 bytes are
//       sent and the frame arrives whole, byte for byte.

using System.Buffers;
using System.Net;
using McProtoNet.Next.Demo;

namespace McProtoNet.Next.Examples;

/// <summary>
///     Network-free proof that <see cref="LowLevelExample" /> does what its comments say.
/// </summary>
internal static class ExamplesSelfCheck
{
    // How long a frame must fail to arrive before we call it "the receiver is waiting".
    private static readonly TimeSpan WaitWindow = TimeSpan.FromSeconds(1.5);

    // ------------------------------------------------------------------ (f)

    /// <summary>
    ///     A lying compression envelope reaches the receiver, fails only where its claim is
    ///     checked, and does not kill the read loop.
    /// </summary>
    public static async Task CheckLyingEnvelopeVisibleAsync(CancellationToken ct)
    {
        var (senderStream, receiverStream) = DuplexPipeStream.CreatePair();
        await using MinecraftClient sender = MinecraftClient.Create(senderStream);
        await using MinecraftClient receiver = MinecraftClient.Create(receiverStream);

        // A size field only exists on a connection with compression enabled.
        sender.CompressionThreshold = 128;
        receiver.CompressionThreshold = 128;

        await LowLevelExample.SendLyingCompressionEnvelopeAsync(sender, ct);

        // An honest frame right behind it: if the loop died on the lie, this one never arrives.
        byte[] honest = new byte[24];
        await sender.SendFrameAsync(honest, default, ct);

        int index = 0;
        bool aliveAfterLie = false;

        await foreach (RawFrame frame in receiver.ReadFramesAsync(ct))
        {
            if (index == 0)
            {
                Check(frame.IsCompressed, "lying frame: expected a compressed envelope");
                Check(frame.Envelope.ClaimedUncompressedSize == LowLevelExample.LyingClaimedSize,
                    $"lying frame: claimed {frame.Envelope.ClaimedUncompressedSize}, expected {LowLevelExample.LyingClaimedSize}");
                Check(frame.Payload.Length < LowLevelExample.LyingClaimedSize,
                    "lying frame: the body should be far smaller than the claim");

                string? rejection = null;
                try
                {
                    using IMemoryOwner<byte> body = frame.Decompress();
                }
                catch (ProtocolViolationException ex)
                {
                    rejection = ex.Message; // exactly here — not in the loop, not in the transport
                }

                Check(rejection is not null, "lying frame: Decompress accepted a claim it should have rejected");

                // Decompress throws ProtocolViolationException for three unrelated reasons: a claim
                // outside [0, MaxFrameSize], a body that is not zlib, and a real output length that
                // does not match the claim (Frames/RawFrame.cs:122/156/159). Only the third is the
                // lie under test, so pin that branch — catching the bare type would report "the lie
                // was caught" even if the compressor had simply produced garbage.
                Check(rejection!.Contains("but produced", StringComparison.Ordinal),
                    $"lying frame: rejected, but for the wrong reason — {rejection}");
            }
            else
            {
                aliveAfterLie = true;
                Check(frame.Payload.Length == honest.Length,
                    $"frame after the lie: {frame.Payload.Length} B, expected {honest.Length}");
            }

            if (++index == 2)
                break;
        }

        Check(aliveAfterLie, "the read loop did not survive the lying envelope");
    }

    // ------------------------------------------------------------------ (g)

    /// <summary>
    ///     A declared length larger than the body makes the receiver wait for the missing bytes;
    ///     once they arrive the frame surfaces whole.
    /// </summary>
    public static async Task CheckOversizedLengthWaitsAsync(CancellationToken ct)
    {
        var (senderStream, receiverStream) = DuplexPipeStream.CreatePair();
        await using MinecraftClient sender = MinecraftClient.Create(senderStream);
        await using MinecraftClient receiver = MinecraftClient.Create(receiverStream);

        await LowLevelExample.SendOversizedLengthAsync(sender, ct);

        // The enumeration runs under its own token. On every exit — success, failed Check, or the
        // 10 s timeout — the finally cancels it and DRAINS the MoveNextAsync that is still in
        // flight before disposing the enumerator: disposing an async iterator while a MoveNextAsync
        // is pending is unsupported, and doing it would hang the self-test, so SelfTest would print
        // "timed out (deadlock?)" in place of the real failure message.
        using var enumeration = CancellationTokenSource.CreateLinkedTokenSource(ct);
        IAsyncEnumerator<RawFrame> frames =
            receiver.ReadFramesAsync(enumeration.Token).GetAsyncEnumerator(enumeration.Token);
        Task<bool> arrival = frames.MoveNextAsync().AsTask();
        try
        {
            // Evidence of waiting: while its declared length is short, the frame must not surface.
            // 1.5 s without it is what we take as evidence — not proof; nothing here can rule out a
            // receiver that would have surfaced it in two.
            // (The delay carries no token on purpose — it is discarded, and a cancelled discard
            // would become an unobserved exception.)
            Task first = await Task.WhenAny(arrival, Task.Delay(WaitWindow)).ConfigureAwait(false);
            Check(first != arrival,
                "the receiver surfaced a frame before its declared length had arrived");

            // Now finish the frame. The receiver was holding 10 of the declared 100 bytes.
            await LowLevelExample.SendMissingTailAsync(sender, ct);

            bool moved;
            try
            {
                moved = await arrival.WaitAsync(TimeSpan.FromSeconds(10), ct).ConfigureAwait(false);
            }
            catch (TimeoutException)
            {
                // Translate, do not propagate. SelfTest.RunAsync treats ANY TimeoutException as its
                // own 30 s watchdog and prints "timed out (deadlock?)", so a frame that simply never
                // came would be reported as a deadlock in the transport.
                throw new InvalidOperationException(
                    "the frame did not arrive within 10 s of the missing bytes being sent");
            }

            Check(moved, "the read enumeration ended instead of yielding the completed frame");

            RawFrame frame = frames.Current;
            Check(frame.DeclaredLength == LowLevelExample.OversizedDeclaredLength,
                $"declared {frame.DeclaredLength} B, expected {LowLevelExample.OversizedDeclaredLength}");
            Check(frame.Payload.Length == LowLevelExample.OversizedDeclaredLength,
                $"payload {frame.Payload.Length} B, expected the full {LowLevelExample.OversizedDeclaredLength}");

            // The bytes themselves, not just the count. This is the one frame in the suite that was
            // assembled from two separate writes, so its payload is the likeliest to be
            // multi-segment: a cross-segment slicing or ordering bug in the frame reader would hand
            // back 100 bytes of the wrong content and every length check above would still pass.
            byte[] delivered = frame.Payload.ToArray();
            byte[] expected = LowLevelExample.ExpectedOversizedFrameBody();
            Check(delivered.AsSpan().SequenceEqual(expected),
                "the reassembled frame carries the wrong bytes: the two halves did not join as sent");
        }
        finally
        {
            enumeration.Cancel();
            try
            {
                await arrival.ConfigureAwait(false);
            }
            catch
            {
                // The drain only has to finish. Its outcome was already judged above, and on a
                // failure path the real message is the one on its way out of this method.
            }

            await frames.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
