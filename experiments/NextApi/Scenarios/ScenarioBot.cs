// McProtoNet.Next experiment — SCENARIO 1 (control): THE BOT.
//
// This is the reference scenario. Examples/BotExample.cs already walks the full
// handshake -> login -> configuration -> play path on the typed floor; this file does
// NOT reimplement it. It is a thin verification wrapper that pins the load-bearing claim
// of the high floor:
//
//     a bot answers the server on the TYPED floor — a switch over packet CLASSES —
//     without ever touching raw bytes and without a manual `id == 0x..` switch.
//
// The proof runs network-free over the same in-memory duplex pair the self-test uses
// (Demo/SelfTest.cs). A fake "server" client sends a clientbound Play keep-alive; the
// "bot" client answers it on the typed floor exactly as BotExample's play case does; the
// server confirms the echo. Every member called here is checked against the real
// experiment files and the generated Protocol — nothing is invented.
//
// The scenario is expected to HOLD. Where the reference still forces friction, that
// friction is reproduced here in compile-checked helpers and marked with
//   // CRACK: <pain> | IDEAL: <how it should be>
// so the "clean reference" claim is honest and the rub is documented in code, not prose.
// A convenience that is missing outright sits in an #if false block marked HARD CRACK.

using System;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Net;          // InputPacket
using McProtoNet.Next;         // MinecraftClient, ReadTypedAsync, SendAsync, Is/Decode
using McProtoNet.Next.Demo;    // DuplexPipeStream (same-assembly test harness)
using McProtoNet.Protocol;     // IPacket, UnknownPacket, PacketPhase, PacketDirection, PacketRegistry
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace McProtoNet.Next.Scenarios;

/// <summary>
///     Control scenario: verifies that the typed floor covers the main bot case
///     (answer the server) with a class switch, no raw bytes, no id switch.
/// </summary>
internal static class ScenarioBot
{
    // A concrete protocol version from McProtoNet's honest range 735–772 (McProtoNet/AGENTS.md).
    // Both KeepAlive directions map on it: clientbound 770–772 => 0x26, serverbound 768–770 =>
    // 0x1A (verified in Generated/Packets/Play/{Clientbound,Serverbound}/KeepAlivePacket.cs).
    private const int DefaultProtocolVersion = 770;

    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    /// <summary>Runs the scenario with a watchdog; prints OK/FAIL. Returns true on success.</summary>
    public static async Task<bool> RunAsync(int protocolVersion = DefaultProtocolVersion)
    {
        using var cts = new CancellationTokenSource(Timeout);
        try
        {
            await VerifyKeepAliveReflexAsync(protocolVersion, cts.Token).WaitAsync(Timeout).ConfigureAwait(false);
            Console.WriteLine("OK   scenario-bot: typed keep-alive reflex, no raw / no id switch");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL scenario-bot: {ex.GetType().Name}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    ///     The bot's core reflex on the typed floor: a clientbound keep-alive comes in as a
    ///     CLASS, the bot answers with the serverbound twin. No byte ever crosses the bot's
    ///     hands; the only branch is a type switch. This is the shape BotExample uses in play.
    /// </summary>
    public static async Task VerifyKeepAliveReflexAsync(int protocolVersion, CancellationToken ct)
    {
        var (botStream, serverStream) = DuplexPipeStream.CreatePair();
        await using MinecraftClient bot = MinecraftClient.Create(botStream);
        await using MinecraftClient server = MinecraftClient.Create(serverStream);

        const long token = 0x0123_4567_89AB_CDEFL;

        // Server side: send a clientbound Play keep-alive. SendAsync takes the id from the
        // packet TYPE — the clientbound keep-alive id — so the bot reads it as clientbound.
        await server.SendAsync(new PlayCb.KeepAlivePacket(token), protocolVersion, ct);

        // Bot side: the whole receive step is `switch on the class`. The typed floor decodes
        // inside its own loop; the InputPacket window never reaches here (that is the claim).
        bool answered = false;
        await foreach (IPacket packet in bot.ReadTypedAsync(PacketPhase.Play, protocolVersion, ct))
        {
            switch (packet)
            {
                case PlayCb.KeepAlivePacket keepAlive:
                    await bot.SendAsync(
                        new PlaySb.KeepAlivePacket(keepAlive.KeepAliveId), protocolVersion, ct);
                    answered = true;
                    break;

                // In this fixture nothing else should arrive; an unmapped id is still a normal
                // stream condition, not an error (typed floor yields UnknownPacket for it).
                case UnknownPacket:
                    break;

                default:
                    throw new InvalidOperationException(
                        $"unexpected clientbound packet {packet.GetType().Name}");
            }

            if (answered)
                break;
        }

        Check(answered, "the bot never saw the clientbound keep-alive as a class");

        // Server side confirms the echo. NOTE (not a crack for the bot): the typed floor
        // decodes CLIENTBOUND only (TypedReadExtensions hardcodes PacketDirection.Clientbound),
        // so reading the bot's SERVERBOUND reply is the raw floor's job here. The bot itself
        // never needs this direction — it only ever receives clientbound — so for scenario 1
        // this is correct, not a gap. The raw read below stays inside its window: decode now.
        await foreach (InputPacket raw in server.ReadPacketsAsync(ct))
        {
            Check(raw.Is<PlaySb.KeepAlivePacket>(protocolVersion),
                "the reply id is not the serverbound keep-alive");
            PlaySb.KeepAlivePacket reply = raw.Decode<PlaySb.KeepAlivePacket>(protocolVersion);
            Check(reply.KeepAliveId == token, "the keep-alive token did not round-trip");
            break;
        }
    }

    // ------------------------------------------------------------------ friction, in code

    /// <summary>
    ///     Reproduces the one rub the reference bot cannot avoid: extracting a human-readable
    ///     kick reason from a version-split packet. Compile-checked; mirrors BotExample:107.
    /// </summary>
    internal static string DescribeKickReason(ConfCb.DisconnectPacket disconnect)
    {
        // CRACK: one logical field ("why were we kicked?") is split across version-gated
        //        layers with DIFFERENT types — V764.ReasonJson is a string, V765_Last.Reason
        //        is an NbtTag — so the caller must branch on layers and null-coalesce two
        //        shapes into one. This friction is in the GENERATED packet shape, not in the
        //        Next transport/typed surface; the bot still expresses it, just not prettily.
        // IDEAL: a version-agnostic accessor on the packet — `disconnect.Reason` returning a
        //        single reason type (Chat/Component) regardless of protocol version — so the
        //        consumer reads one field and never names a version layer.
        return disconnect.V764?.ReasonJson
            ?? disconnect.V765_Last?.Reason.ToString()
            ?? "(no reason)";

#if false
        // HARD CRACK: the version-agnostic accessor above does not exist. The desired call is:
        return disconnect.Reason;   // one field, any supported version — not emitted today
#endif
    }

    /// <summary>
    ///     Names an unknown packet for a log line. UnknownPacket carries Id, Phase and
    ///     Direction, so naming it means dropping back to the registry by hand.
    /// </summary>
    internal static string NameUnknown(UnknownPacket unknown, int protocolVersion)
    {
        // CRACK (nit): UnknownPacket carries no resolved name — its Identity reports the
        //              "unknown" placeholder — so a friendly log line forces a second, manual
        //              PacketRegistry.TryResolve round-trip (see BotExample:133). Deliberate per
        //              UnknownPacket's own docs (it holds no raw window), but it is still
        //              boilerplate every consumer that logs unknowns repeats.
        // IDEAL: UnknownPacket exposes a lazy `Name` (or the typed floor resolves it once), so
        //        the consumer logs `unknown.Name` without touching the registry.
        return PacketRegistry.TryResolve(
            unknown.Id, protocolVersion, unknown.Phase, unknown.Direction, out var descriptor)
            ? descriptor.Identity.Name
            : $"0x{unknown.Id:X2}";
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
