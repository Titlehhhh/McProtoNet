// McProtoNet.Next experiment — examples: the bot walk against a REAL server, instrumented.
//
// Examples/BotExample.cs stays untouched on purpose: it is the reference example, and a
// reference that carries counters and verdict printing stops being one. This file
// duplicates its loop and adds the instrumentation a probe needs.
//
// Run: dotnet run --project McProtoNet/experiments/NextApi -- live [host] [port] [name] [pv]
//
// Two deliberate differences from the reference:
//   * it reads the RAW floor and dispatches by hand — the typed door hides the payload
//     length, and the largest body seen is the number that says whether the walk carried
//     real chunk traffic or only handshake chatter;
//   * a body that fails to decode is counted, not fatal — a spec defect in one packet must
//     not hide the verdict on the transport, which is what this probe is about.

using McProtoNet.Net;
using McProtoNet.Protocol;
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using ConfSb = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace McProtoNet.Next.Examples;

/// <summary>
///     Runs the bot walk (login → configuration → play) against a live server under a hard
///     time cap and prints one verdict line. Returns a process exit code.
/// </summary>
internal static class LiveBotRun
{
    // Same handshake "next state" the reference uses for a login connection.
    private const int LoginIntent = 2;

    // A healthy bot never returns on its own — the cap is what makes this probe terminate.
    private static readonly TimeSpan RunCap = TimeSpan.FromSeconds(60);

    /// <summary>Walks the connection, then prints what was observed. 0 = OK, 1 = FAIL.</summary>
    public static async Task<int> RunAsync(string host, int port, string name, int protocolVersion)
    {
        using var cts = new CancellationTokenSource(RunCap);
        var stats = new Stats();

        // Trailing bytes never reach an exception (PacketFlow raises a hook instead), so the
        // only way to notice a suspect spec during a live walk is to subscribe.
        TrailingBytesHook hook = (_, _, _) => stats.TrailingBytes++;
        PacketFlow.OnTrailingBytes += hook;

        // The client is owned here, not by the walk: a dying pump reaches the read loop as a
        // bare cancellation, and the exception that caused it lives only in Completion —
        // which cannot be read from inside a walk that owns and disposes its own client.
        MinecraftClient? client = null;
        string? failure = null;
        try
        {
            client = await MinecraftClient.ConnectAsync(host, port, cancellationToken: cts.Token)
                .ConfigureAwait(false);
            await WalkAsync(client, host, port, name, protocolVersion, stats, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            stats.HitTimeCap = true; // the expected end of a successful run — unless a pump died
        }
        catch (Exception ex)
        {
            failure = $"{ex.GetType().Name}: {ex.Message}";
        }
        finally
        {
            // Disposal is what stops the pumps; only after it is Completion final.
            if (client is not null)
                await client.DisposeAsync().ConfigureAwait(false);
            PacketFlow.OnTrailingBytes -= hook;
        }

        return Report(host, port, name, protocolVersion, stats, failure, TransportRootCause(client));
    }

    // Read strictly AFTER DisposeAsync: only then have the pumps stopped and Completion is
    // final. DisposeAsync already observed a faulted Completion, so touching it here is safe.
    private static string? TransportRootCause(MinecraftClient? client)
    {
        if (client?.Completion is not { IsFaulted: true } completion)
            return null;
        return string.Join("; ", completion.Exception!.Flatten().InnerExceptions
            .Select(inner => $"{inner.GetType().Name}: {inner.Message}"));
    }

    private static async Task WalkAsync(
        MinecraftClient client, string host, int port, string name, int protocolVersion, Stats stats,
        CancellationToken cancellationToken)
    {
        PacketPhase phase = PacketPhase.Login;

        await client.SendAsync(
            new HandshakeSb.SetProtocolPacket(protocolVersion, host, port, LoginIntent), protocolVersion, cancellationToken);
        await client.SendAsync(
            new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), protocolVersion, cancellationToken);
        Console.WriteLine($"[login] handshake + login start sent as {name}");

        await foreach (InputPacket raw in client.ReadPacketsAsync(cancellationToken))
        {
            stats.Packets++;
            if (raw.Data.Length > stats.BiggestBody)
                stats.BiggestBody = raw.Data.Length;

            // Decode now, synchronously: raw.Data is a window into the transport buffer and
            // dies on the next iteration. A body that fails to decode is a false plus a
            // reason, so counting it costs no exception handling at all.
            if (!PacketFlow.TryDecode(in raw, protocolVersion, phase, PacketDirection.Clientbound,
                    out IPacket? packet, out DecodeError error))
            {
                stats.DecodeErrors++;
                stats.FirstDecodeError ??= $"id 0x{raw.Id:X2} in {phase} — {error}";
                continue;
            }

            switch (packet)
            {
                // ---------------------------------------------------------------- login
                case LoginCb.LoginCompressPacket compress:
                    client.CompressionThreshold = compress.Threshold;
                    stats.Threshold = compress.Threshold;
                    Console.WriteLine($"[login] compression on, threshold {compress.Threshold}");
                    break;

                case LoginCb.LoginSuccessPacket success:
                    await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), protocolVersion, cancellationToken);
                    phase = PacketPhase.Configuration;
                    await client.SendAsync(
                        new ConfSb.ClientInformationPacket(
                            "en_us", 8, 0, true, 0x7F, 1, false, true, V768_Last: new(0)),
                        protocolVersion, cancellationToken);
                    stats.LoginSuccess = true;
                    Console.WriteLine($"[login] success: {success.Username} {success.Uuid} -> configuration");
                    break;

                case LoginCb.LoginDisconnectPacket kick:
                    stats.Disconnect = $"login: {kick.Reason}";
                    return;

                // -------------------------------------------------------- configuration
                case ConfCb.KeepAlivePacket configKeepAlive:
                    await client.SendAsync(
                        new ConfSb.KeepAlivePacket(configKeepAlive.KeepAliveId), protocolVersion, cancellationToken);
                    stats.KeepAlives++;
                    break;

                case ConfCb.PingPacket ping:
                    await client.SendAsync(new ConfSb.PongPacket(ping.Id), protocolVersion, cancellationToken);
                    break;

                case ConfCb.SelectKnownPacksPacket packs:
                    await client.SendAsync(
                        new ConfSb.SelectKnownPacksPacket(packs.Packs), protocolVersion, cancellationToken);
                    Console.WriteLine($"[config] known packs: {packs.Packs.Length}, echoed back");
                    break;

                case ConfCb.FinishConfigurationPacket:
                    await client.SendAsync(new ConfSb.FinishConfigurationPacket(), protocolVersion, cancellationToken);
                    phase = PacketPhase.Play;
                    stats.ConfigurationFinished = true;
                    Console.WriteLine("[play] configuration finished");
                    break;

                case ConfCb.DisconnectPacket disconnect:
                    stats.Disconnect =
                        $"config: {disconnect.V764?.ReasonJson ?? disconnect.V765_Last?.Reason.ToString()}";
                    return;

                // ----------------------------------------------------------------- play
                case PlayCb.KeepAlivePacket playKeepAlive:
                    await client.SendAsync(
                        new PlaySb.KeepAlivePacket(playKeepAlive.KeepAliveId), protocolVersion, cancellationToken);
                    stats.KeepAlives++;
                    Console.WriteLine($"[play] keep-alive {playKeepAlive.KeepAliveId} answered");
                    break;

                case PlayCb.PlayerPositionPacket position:
                    await client.SendAsync(
                        new PlaySb.TeleportConfirmPacket(position.TeleportId), protocolVersion, cancellationToken);
                    stats.Teleports++;
                    Console.WriteLine($"[play] at x={position.X:F1} y={position.Y:F1} z={position.Z:F1}, teleport confirmed");
                    break;

                // An id the registry does not map in this phase is a normal stream condition.
                // Live traffic carries many of them; count the distinct ones, do not narrate.
                case UnknownPacket unknown:
                    stats.Unknown.Add((unknown.Phase, unknown.Id));
                    break;
            }
        }

        stats.ServerClosed = true;
    }

    private static int Report(
        string host, int port, string name, int protocolVersion, Stats stats, string? failure,
        string? transportRootCause)
    {
        // OK is decided by the transport's own job: the walk reached play AND stayed there
        // until the cap. A server close before the cap is a drop, not a success — the Play
        // catalog has no disconnect packet, so a play-phase kick arrives as an unmapped id
        // and the only trace of it is the close that follows. A faulted Completion is a dead
        // pump, which reaches the loop as a bare cancellation and would otherwise pass as
        // "hit the cap". Decode errors and trailing bytes belong to the packet layer —
        // reported, but not a transport verdict.
        bool ok = failure is null
            && transportRootCause is null
            && stats.LoginSuccess
            && stats.ConfigurationFinished
            && !stats.ServerClosed;

        Console.WriteLine();
        Console.WriteLine($"--- live bot: {host}:{port} as {name}, protocol {protocolVersion} ---");
        Console.WriteLine($"packets read           : {stats.Packets}");
        Console.WriteLine($"biggest packet body    : {stats.BiggestBody} bytes (decompressed, id excluded)");
        Console.WriteLine($"compression threshold  : {(stats.Threshold is { } t ? t.ToString() : "not received")}");
        Console.WriteLine($"login success          : {YesNo(stats.LoginSuccess)}");
        Console.WriteLine($"configuration finished : {YesNo(stats.ConfigurationFinished)}");
        Console.WriteLine($"keep-alives answered   : {stats.KeepAlives}");
        Console.WriteLine($"teleports confirmed    : {stats.Teleports}");
        Console.WriteLine($"unmapped ids skipped   : {stats.Unknown.Count} distinct");
        Console.WriteLine($"decode errors          : {stats.DecodeErrors}"
            + (stats.FirstDecodeError is null ? string.Empty : $" (first: {stats.FirstDecodeError})"));
        Console.WriteLine($"trailing-byte packets  : {stats.TrailingBytes}");
        Console.WriteLine($"ended by               : {EndReason(stats, failure)}");
        Console.WriteLine(ok
            ? "LIVE OK: the new transport carried the full login -> configuration -> play walk"
            : $"LIVE FAIL: {FailureReason(stats, failure, transportRootCause)}");
        if (transportRootCause is not null)
            Console.WriteLine($"transport root cause   : {transportRootCause}");
        return ok ? 0 : 1;
    }

    private static string EndReason(Stats stats, string? failure) =>
        failure is not null ? $"error — {failure}"
        : stats.Disconnect is not null ? $"server disconnect — {stats.Disconnect}"
        : stats.ServerClosed ? "server closed the connection"
        : stats.HitTimeCap ? $"the {RunCap.TotalSeconds:F0} s cap"
        : "the loop returned";

    private static string FailureReason(Stats stats, string? failure, string? transportRootCause) =>
        failure is not null ? failure
        : stats.Disconnect is not null ? $"the server disconnected the bot — {stats.Disconnect}"
        : !stats.LoginSuccess ? "login never succeeded"
        : !stats.ConfigurationFinished ? "configuration never finished"
        : stats.ServerClosed ? "the server closed the connection before the cap — the walk reached play "
            + "and was then dropped (a play-phase kick has no packet in the Play catalog: it shows up as "
            + "an unmapped id followed by this close)"
        : transportRootCause is not null ? "the transport died mid-walk"
        : "the walk ended without reaching the cap";

    private static string YesNo(bool value) => value ? "yes" : "no";

    /// <summary>Everything the run observed; printed as one block at the end.</summary>
    private sealed class Stats
    {
        public readonly HashSet<(PacketPhase Phase, int Id)> Unknown = [];
        public int Packets;
        public long BiggestBody;
        public int? Threshold;
        public bool LoginSuccess;
        public bool ConfigurationFinished;
        public int KeepAlives;
        public int Teleports;
        public int DecodeErrors;
        public int TrailingBytes;
        public string? FirstDecodeError;
        public string? Disconnect;
        public bool ServerClosed;
        public bool HitTimeCap;
    }
}
