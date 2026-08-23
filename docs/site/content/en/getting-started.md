# First bot in 15 minutes

What you will have at the end: a console bot that connects to an offline-mode server, logs in, answers keep-alives and prints its health. The full program is `examples/MinimalBot/Program.cs` in the repository; this page walks through it.

## 1. Install

```
dotnet add package McProtoNet --prerelease
```

`McProtoNet` pulls the other four packages (`Transport`, `Protocol`, `Primitives`, `NBT`). Target .NET 8 or newer.

## 2. Pick the protocol version

Every send and every decode takes a **protocol version** number (`pv`). It is the wire protocol of the game version you connect to, not the package version.

| Game version | Protocol | Constant |
| --- | --- | --- |
| 1.16 | 735 | `MinecraftVersion.V1_16` |
| 1.18.2 | 758 | `MinecraftVersion.V1_18_2` |
| 1.20.2 | 764 | `MinecraftVersion.V1_20_2` |
| 1.21 – 1.21.1 | 767 | `MinecraftVersion.V1_21_To_1_21_1` |
| 1.21.4 | 769 | `MinecraftVersion.V1_21_4` |
| 1.21.7 – 1.21.8 | 772 | `MinecraftVersion.V1_21_7_To_1_21_8` |
| 1.21.11 | 774 | `MinecraftVersion.V1_21_11` |
| 26.1 – 26.1.2 | 775 | `MinecraftVersion.V26_1_To_26_1_2` |
| 26.2 | 776 | `MinecraftVersion.V26_2` |

The full list (snapshots and pre-releases included) is `MinecraftVersion` in the API reference. Supported range: 735–776. Servers with ViaVersion accept older clients, so 772 is a safe default for testing.

## 3. The four phases

A Minecraft session walks through four **phases**; the same packet id means different things in each, so the bot tracks the phase itself.

| Phase | Who talks | What happens | You send |
| --- | --- | --- | --- |
| `handshaking` | client | one packet: protocol version, host, port, "I want to log in" | `SetProtocolPacket` |
| `login` | both | name, optional encryption, optional compression, success | `LoginStartPacket`, `EncryptionResponsePacket`, `LoginAcknowledgedPacket` |
| `configuration` | both | client settings, registry data, known packs | `ClientInformationPacket`, `SelectKnownPacksPacket`, `FinishConfigurationPacket` |
| `play` | both | the game: keep-alive, positions, chat, inventory | whatever the bot does; at least `KeepAlivePacket` and `TeleportConfirmPacket` |

The library does not run this state machine for you (see [What the library does not do](non-goals.md)). It gives you a generated handler base class with one `On<PacketName>` method per packet and a `Phase` property you switch when the server tells you to.

## 4. The program

```csharp
using McProtoNet;
using McProtoNet.Protocol;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;

const int Pv = 772;                                    // 1.21.8

await using var client = new MinecraftClient(new MinecraftClientOptions { Host = "127.0.0.1", Port = 25565 });
await client.ConnectAsync();

// handshaking → login: two packets, then the server talks
await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, "127.0.0.1", 25565, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket("McProtoBot", V764_Last: new(Guid.NewGuid())), Pv);

var bot = new Bot(client, Pv);
try
{
    await foreach (var packet in client.ReadPacketsAsync())   // raw frames, one by one
    {
        await bot.HandleAsync(in packet, Pv);                  // decode by (phase, id) → On<Name>
        if (bot.Stopped) break;
    }
}
catch (EndOfStreamException)
{
    // the server closed the socket — a normal end of session, not a bug
}
```

The handler is a class that overrides only what it cares about. The phase switches are the important lines:

```csharp
sealed class Bot(MinecraftClient client, int pv) : ClientboundHandler
{
    public bool Stopped { get; private set; }

    // login
    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket p)
    {
        client.CompressionThreshold = p.Threshold;             // from now on frames are compressed
        return default;
    }

    protected override async ValueTask OnEncryptionRequest(LoginCb.EncryptionRequestPacket p)
    {
        if (p.V766_Last is { ShouldAuthenticate: true }) { Stopped = true; return; }   // online-mode server: not supported
        using var rsa = EncryptionHelpers.DecodeRSAPublicKey(p.PublicKey)!;
        var secret = EncryptionHelpers.GenerateAESPrivateKey();
        await client.SendAsync(new LoginSb.EncryptionResponsePacket(rsa.Encrypt(secret, false), rsa.Encrypt(p.VerifyToken, false)), pv);
        client.EnableEncryption(secret);                        // from now on frames are encrypted
    }

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket p)
    {
        await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;                      // ← phase switch
        await client.SendAsync(new ConfSb.ClientInformationPacket("en_us", 2, 0, true, 0x7F, 1, false, true, V768_Last: new(ParticleStatus.All)), pv);
    }

    // configuration
    protected override ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket p)
        => client.SendAsync(new ConfSb.SelectKnownPacksPacket(p.Packs), pv);

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket p)
    {
        await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
        Phase = PacketPhase.Play;                               // ← phase switch
    }

    // play
    protected override ValueTask OnKeepAlive(PlayCb.KeepAlivePacket p)
        => client.SendAsync(new PlaySb.KeepAlivePacket(p.KeepAliveId), pv);

    protected override ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket p)
        => client.SendAsync(new PlaySb.TeleportConfirmPacket(p.TeleportId), pv);

    protected override ValueTask OnUpdateHealth(PlayCb.UpdateHealthPacket p)
    {
        Console.WriteLine($"health {p.Health}, food {p.Food}");
        return default;
    }

    protected override ValueTask OnKickDisconnect(PlayCb.KickDisconnectPacket p)
    {
        Stopped = true;
        return default;
    }
}
```

Run it against a local server in offline mode (`online-mode=false` in `server.properties`) and you will see the health line once you are in the world.

## 5. When it breaks

| You see | It means | Do |
| --- | --- | --- |
| `EndOfStreamException` from `ReadPacketsAsync` | the server closed the connection — after a kick, an idle timeout, or a normal quit | catch it around the loop; read the last `KickDisconnect` / `LoginDisconnect` you handled for the reason |
| `OnLoginDisconnect` / `OnKickDisconnect` called | the server refused you; the packet carries the reason text | print the reason, stop the loop |
| `EncryptionRequest` with `ShouldAuthenticate = true` | online-mode server; it wants a Mojang account | not supported by this library yet — use an offline server |
| `ProtocolNotSupportException` on `SendAsync` | the packet does not exist on that protocol version | check the version table above; use a packet that exists on your `pv` |
| `PacketDecodeException` | the body did not match the spec for that version | wrong `pv`, or a packet the generator does not cover yet — the banner above is honest about this |
| the loop hangs after login | you did not answer `KeepAlive` or did not switch `Phase` | see the phase switches in the code above |

## Where next

- [What the library does not do](non-goals.md) — before you plan pathfinding.
- [Glossary](glossary.md) — frame, phase, ordinal, catalog and other words the API uses.
- [API reference](xref:McProtoNet) — `MinecraftClient` and `ClientboundHandler` first.
