// EXPERIMENT (McProtoNet.Next) — SCENARIO 2: the status checker.
//
// Author-of-the-scenario note. This is a probe of the public surface, not shipped
// code. The task: connect, send a status handshake (intent = 1), ask for the status,
// read the server-description JSON, ping with a timestamp, read the pong, return the
// parsed status plus the round-trip latency. Strictly 4-5 packets, one after another —
// no compression, no encryption, no concurrent read/write.
//
// Every protocol fact below is read from the delivered McProtoNet.Protocol generate,
// not from memory:
//   handshaking.toServer.set_protocol  id 0x00  (ProtocolVersion varint, ServerHost
//                                                 string, ServerPort ushort, NextState
//                                                 varint) — SetProtocolPacket.cs
//   status.toServer.ping_start         id 0x00  (no fields)      — PingStartPacket.cs
//   status.toServer.ping / PingRequest id 0x01  (long Time)      — PingRequestPacket.cs
//   status.toClient.server_info        id 0x00  (string Response)— ServerInfoPacket.cs
//   status.toClient.ping / PongResponse id 0x01 (long Time)      — PongResponsePacket.cs
// intent = 1 (status) is the value the task fixed; the generate models NextState as a
// plain int, it does not name the constant.
//
// THE VERDICT THIS FILE ARGUES: CRACKS on weight. See CRACK 1. A status query drives
// the full MinecraftClient — two background pumps, two pipes (the receive pipe carries a
// 16 MiB pause policy), a send semaphore, six-plus encryption fields, a FrameWriter that
// caches a libdeflate compressor — none of which this four-packet, half-duplex,
// plaintext exchange ever needs. The whole machine idles at 100% provisioning for 0% use.

#nullable enable

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using McProtoNet.Protocol.Packets.Status.Clientbound;
using McProtoNet.Protocol.Packets.Status.Serverbound;

namespace McProtoNet.Next.Scenarios;

/// <summary>
///     The parsed result of a server-list-ping status query.
/// </summary>
/// <param name="Json">The raw status JSON exactly as the server sent it (ServerInfo.Response).</param>
/// <param name="Latency">Wall-clock round trip between our PingRequest and the server's PongResponse.</param>
/// <param name="VersionName">The <c>version.name</c> field, or <c>null</c> when the JSON has no such field.</param>
/// <param name="OnlinePlayers">The <c>players.online</c> field, or <c>null</c>.</param>
/// <param name="MaxPlayers">The <c>players.max</c> field, or <c>null</c>.</param>
public sealed record ServerStatusResult(
    string Json, TimeSpan Latency, string? VersionName, int? OnlinePlayers, int? MaxPlayers);

/// <summary>
///     Scenario 2: query a Minecraft server's status (MOTD, version, player count) and its
///     latency, using the public doors of <see cref="MinecraftClient" />.
/// </summary>
public static class ScenarioStatus
{
    // The task fixed this: handshake NextState = 1 selects the Status phase (2 would be Login).
    private const int StatusIntent = 1;

    /// <summary>
    ///     Runs the status handshake against <paramref name="host" />:<paramref name="port" /> and
    ///     returns the parsed status and the ping latency.
    /// </summary>
    public static async Task<ServerStatusResult> QueryAsync(
        string host, int port, int protocolVersion, CancellationToken cancellationToken = default)
    {
        // CRACK 1 (wrong-weight, the headline): this one line stands up two background pumps
        // (fill + drain, started in the constructor and running for the whole lifetime), two
        // System.IO.Pipelines Pipes, a SemaphoreSlim send gate, and the entire encryption
        // apparatus (_decryptor/_encryptor/_pendingEncryptor/_encryptorInstalled/_cipherOwner).
        // A status query is 4 packets, strictly half-duplex, never compressed, never encrypted.
        // Every one of those facilities is provisioned and never used.
        // IDEAL: a light door ServerStatus.QueryAsync straight on the socket — see the
        //        HARD CRACK block at the bottom of this method.
        await using MinecraftClient client =
            await MinecraftClient.ConnectAsync(host, port, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        // Packet 1 — handshake into the Status phase. The id (0x00) comes from the type.
        await client.SendAsync(
            new SetProtocolPacket(protocolVersion, host, port, StatusIntent), protocolVersion, cancellationToken)
            .ConfigureAwait(false);

        // Packet 2 — "send me your status".
        await client.SendAsync(new PingStartPacket(), protocolVersion, cancellationToken).ConfigureAwait(false);

        // CRACK 2 (missing-api, friction): the exchange is a strict request -> response ladder,
        // but the only read door is an IAsyncEnumerable that owns the single read cursor for its
        // whole lifetime. To read ONE packet, then send, then read ONE more, there is no
        // ReadOneAsync / ReadAsync<T>(). I have to drive the enumerator by hand — MoveNextAsync
        // then a pattern match — because a plain `await foreach ... break` would dispose the
        // enumerator (releasing the read door) between the two reads, forcing a re-entry.
        // IDEAL: ValueTask<T> ReadAsync<T>(phase, pv, ct) for a known one-shot reply, or a
        //        non-generic ReadOnePacketAsync that yields exactly one decoded packet.
        await using var packets = client
            .ReadTypedAsync(PacketPhase.Status, protocolVersion, cancellationToken)
            .GetAsyncEnumerator(cancellationToken);

        // Packet 3 — the server-description JSON.
        ServerInfoPacket info = await ReadNextAsync<ServerInfoPacket>(packets).ConfigureAwait(false);

        // Packet 4 — ping with a timestamp token; measure the round trip on the wall clock and
        // also verify the server echoes our token back unchanged (the protocol's own contract).
        long token = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long startTicks = Stopwatch.GetTimestamp();
        await client.SendAsync(new PingRequestPacket(token), protocolVersion, cancellationToken).ConfigureAwait(false);

        // Packet 5 — the pong.
        PongResponsePacket pong = await ReadNextAsync<PongResponsePacket>(packets).ConfigureAwait(false);
        TimeSpan latency = Stopwatch.GetElapsedTime(startTicks);

        if (pong.Time != token)
            throw new ProtocolViolationException(
                $"PongResponse echoed {pong.Time}, expected our token {token}.");

        var (versionName, online, max) = ParseStatusJson(info.Response);
        return new ServerStatusResult(info.Response, latency, versionName, online, max);

#if false
        // HARD CRACK: there is NO lightweight status door in the surface, and it cannot be
        // built from the outside — MinecraftClient owns the socket and never exposes it, and
        // its pumps start unconditionally in the constructor. The exchange this scenario needs
        // is so constrained (half-duplex, uncompressed, unencrypted, 4 packets) that the honest
        // shape is a static helper that opens a socket, writes two length-prefixed frames, reads
        // one, writes one, reads one, and closes — no pumps, no pipes, no gate, no cipher state.
        // The whole thing would be ~60 lines against NetworkStream + a VarInt reader/writer.
        //
        // The ideal call site, with the transport that the experiment does not provide:
        ServerStatusResult status = await ServerStatus.QueryAsync(host, port, protocolVersion, cancellationToken);
        return status;
#endif
    }

    // Drives the typed enumerator to the next packet of type <typeparamref name="T" />, tolerating
    // (and skipping) the UnknownPacket markers a status stream should not produce but might.
    //
    // CRACK 3 (forced-ugliness, part of CRACK 2): the stream now yields IPacket, so the helper
    // is type-safe and its constraint is honest — but a conversation whose exact packet sequence
    // is known at compile time still needs a MoveNextAsync + pattern-match dance per reply.
    // What is missing is a one-shot read door, not a better element type.
    private static async ValueTask<T> ReadNextAsync<T>(IAsyncEnumerator<IPacket> packets)
        where T : class, IPacket
    {
        while (await packets.MoveNextAsync().ConfigureAwait(false))
        {
            switch (packets.Current)
            {
                case T typed:
                    return typed;
                case UnknownPacket:
                    continue; // not expected in a status exchange; skip rather than fault
                default:
                    throw new ProtocolViolationException(
                        $"Expected {typeof(T).Name}, got {packets.Current.GetType().Name}.");
            }
        }

        throw new EndOfStreamException(
            $"The connection closed before a {typeof(T).Name} arrived.");
    }

    // Pulls the three fields the caller most often wants out of the status JSON. Best-effort:
    // a server may omit any of them, and a malformed body is reported as (null, null, null)
    // rather than thrown — the raw JSON is returned alongside for callers who want more.
    private static (string? VersionName, int? Online, int? Max) ParseStatusJson(string json)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            string? versionName = null;
            if (root.TryGetProperty("version", out JsonElement version) &&
                version.ValueKind == JsonValueKind.Object &&
                version.TryGetProperty("name", out JsonElement name) &&
                name.ValueKind == JsonValueKind.String)
            {
                versionName = name.GetString();
            }

            int? online = null;
            int? max = null;
            if (root.TryGetProperty("players", out JsonElement players) &&
                players.ValueKind == JsonValueKind.Object)
            {
                if (players.TryGetProperty("online", out JsonElement onlineEl) &&
                    onlineEl.ValueKind == JsonValueKind.Number)
                {
                    online = onlineEl.GetInt32();
                }

                if (players.TryGetProperty("max", out JsonElement maxEl) &&
                    maxEl.ValueKind == JsonValueKind.Number)
                {
                    max = maxEl.GetInt32();
                }
            }

            return (versionName, online, max);
        }
        catch (JsonException)
        {
            return (null, null, null);
        }
    }
}
