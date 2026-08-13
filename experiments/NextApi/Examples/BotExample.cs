// McProtoNet.Next experiment — examples, TOP floor: a live bot written against the
// typed doors of the new API. Nothing here touches src/**.
//
// The whole login → configuration → play walk is one loop and one switch over packet
// classes: no visitor, no handler base class, no manual id switch. Compare with
// examples/MinimalBot/Program.cs, which walks the same login → configuration → play
// sequence on the old API. Its chat and arm swing are left out here — those packets live
// in that sample's own local namespace — so this bot only ever answers, never acts.
//
// This file compiles as part of the experiment project. Running it needs a real server,
// so the self-test (Demo/SelfTest.cs) does not call it.

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
///     A minimal working bot on the typed floor of <see cref="MinecraftClient" />: connect,
///     handshake, login, configuration, play — keep-alives answered and teleports confirmed.
/// </summary>
public static class BotExample
{
    // The handshake "next state" the old sample uses for a login connection
    // (examples/MinimalBot/Program.cs).
    private const int LoginIntent = 2;

    /// <summary>
    ///     Runs the bot until the server disconnects it or <paramref name="cancellationToken" />
    ///     fires.
    /// </summary>
    /// <param name="host">Server host name or address.</param>
    /// <param name="port">Server port.</param>
    /// <param name="name">Player name to log in with (offline mode).</param>
    /// <param name="protocolVersion">Protocol version that defines wire ids and body layouts.</param>
    /// <param name="cancellationToken">Token that stops the bot.</param>
    public static async Task RunAsync(
        string host, int port, string name, int protocolVersion, CancellationToken cancellationToken = default)
    {
        await using MinecraftClient client =
            await MinecraftClient.ConnectAsync(host, port, cancellationToken: cancellationToken);

        // The phase is a plain local. ReadTypedAsync samples it before decoding each packet,
        // so moving the connection forward is one assignment inside the loop.
        PacketPhase phase = PacketPhase.Login;
        HashSet<(PacketPhase Phase, int Id)> unknownSeen = [];

        await client.SendAsync(
            new HandshakeSb.SetProtocolPacket(protocolVersion, host, port, LoginIntent), protocolVersion, cancellationToken);
        await client.SendAsync(
            new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), protocolVersion, cancellationToken);
        Console.WriteLine($"[login] handshake + login start sent as {name}");

        await foreach (IPacket packet in client.ReadTypedAsync(() => phase, protocolVersion, cancellationToken))
        {
            switch (packet)
            {
                // ---------------------------------------------------------------- login
                case LoginCb.LoginCompressPacket compress:
                    client.CompressionThreshold = compress.Threshold;
                    Console.WriteLine($"[login] compression on, threshold {compress.Threshold}");
                    break;

                case LoginCb.LoginSuccessPacket success:
                    await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), protocolVersion, cancellationToken);
                    phase = PacketPhase.Configuration; // the next packet is decoded in this phase
                    await client.SendAsync(
                        new ConfSb.ClientInformationPacket(
                            "en_us", 8, 0, true, 0x7F, 1, false, true, V768_Last: new(0)),
                        protocolVersion, cancellationToken);
                    Console.WriteLine($"[login] success: {success.Username} {success.Uuid} -> configuration");
                    break;

                case LoginCb.LoginDisconnectPacket kick:
                    Console.WriteLine($"[login] kicked: {kick.Reason}");
                    return;

                // -------------------------------------------------------- configuration
                case ConfCb.KeepAlivePacket configKeepAlive:
                    await client.SendAsync(
                        new ConfSb.KeepAlivePacket(configKeepAlive.KeepAliveId), protocolVersion, cancellationToken);
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
                    Console.WriteLine("[play] configuration finished");
                    break;

                case ConfCb.DisconnectPacket disconnect:
                    Console.WriteLine($"[config] kicked: {disconnect.V764?.ReasonJson ?? disconnect.V765_Last?.Reason.ToString()}");
                    return;

                // ----------------------------------------------------------------- play
                case PlayCb.KeepAlivePacket playKeepAlive:
                    await client.SendAsync(
                        new PlaySb.KeepAlivePacket(playKeepAlive.KeepAliveId), protocolVersion, cancellationToken);
                    Console.WriteLine($"[play] keep-alive {playKeepAlive.KeepAliveId} answered");
                    break;

                case PlayCb.PlayerPositionPacket position:
                    await client.SendAsync(
                        new PlaySb.TeleportConfirmPacket(position.TeleportId), protocolVersion, cancellationToken);
                    Console.WriteLine($"[play] at x={position.X:F1} y={position.Y:F1} z={position.Z:F1}, teleport confirmed");
                    break;

                case PlayCb.UpdateHealthPacket health:
                    Console.WriteLine($"[play] health {health.Health}, food {health.Food}");
                    break;

                // An id the registry does not map in this phase is a normal stream condition,
                // not an error. Report each (phase, id) once and keep going — by name, from the
                // registry, not as a hex number the reader has to look up by hand.
                case UnknownPacket unknown:
                    if (unknownSeen.Add((unknown.Phase, unknown.Id)))
                    {
                        string packetName = PacketRegistry.TryResolve(
                            unknown.Id, protocolVersion, unknown.Phase, PacketDirection.Clientbound, out var descriptor)
                            ? descriptor.Identity.Name
                            : $"0x{unknown.Id:X2}";
                        Console.WriteLine(
                            $"  .. [{unknown.Phase}] skipped {packetName} — and every later packet with this id");
                    }

                    break;
            }
        }

        Console.WriteLine("connection closed");
    }
}
