# Joining a server

The connection opens through
[`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md) and
[`MinecraftClientOptions`](../08-api-reference/McProtoNet/MinecraftClientOptions.md).
The options set the address, the timeouts, and, if needed, a proxy.
`ConnectAsync` resolves the address and opens the socket. Sending the first
packets - handshaking and login - stays the job of the calling code.

## Options and ConnectAsync

`MinecraftClientOptions.Host` is required. `Port` defaults to 25565.
`ConnectTimeout` (30 seconds by default) limits the whole `ConnectAsync` call,
including the SRV lookup and the socket open. `LocalEndPoint` binds the outgoing
socket to a specific interface and port. `NoDelay` (on by default) disables the
Nagle algorithm. Both fields apply only to a direct TCP connection, not through
a proxy.

```csharp
var options = new MinecraftClientOptions
{
    Host = "play.example.com",
    ConnectTimeout = TimeSpan.FromSeconds(15),
};

await using var client = new MinecraftClient(options);
await client.ConnectAsync(cancellationToken);
```

A cancellation token stops the attempt before `ConnectTimeout`. When
`ConnectAsync` returns, no packets have gone out yet - the client has only
opened the connection.

## Finding a server by SRV record

A server rarely listens on port 25565 on the domain itself. Usually a DNS
record, `_minecraft._tcp.<host>`, points to the real host and port. The vanilla
client looks up this record before connecting, and `ConnectAsync` does the same
when `UseSrv` is on (the default), `Port` is still 25565, and `Host` is not an
IP literal. The lookup is limited by `SrvTimeout` (5 seconds by default, no
longer than `ConnectTimeout`). If the record does not exist or the lookup times
out, `ConnectAsync` connects to `Host:Port` as given in the options - this is
not an error.

The same lookup is also available on its own, through
[`SrvResolver`](../08-api-reference/McProtoNet/SrvResolver.md):

```csharp
var record = await SrvResolver.ResolveAsync("play.example.com");
if (record is { } srv)
    Console.WriteLine($"{srv.Target}:{srv.Port}");
```

[`SrvResult`](../08-api-reference/McProtoNet/SrvResult.md) carries `Target`,
`Port`, `Priority`, and `Weight` - the four fields from RFC 2782. When several
records exist, `ConnectAsync` and `ResolveAsync` pick one on their own: first by
the lowest `Priority`, then by a weighted pick on `Weight` within that group.

## Proxy

The socket does not have to open directly. `MinecraftClientOptions.Proxy`
accepts an `IProxyClient`, and `ConnectAsync` asks it for a stream to the
already resolved host and port instead of opening a `TcpClient` itself.
`NoDelay` and `LocalEndPoint` do not apply in this case - the proxy client owns
its socket and configures it on its own. Implementations come from
[QuickProxyNet](https://github.com/Titlehhhh/QuickProxyNet), a separate library
with no dependencies. It supports HTTP CONNECT and SOCKS4/4a/5, and, among newer
protocols, VLESS, VMess, and Trojan. It does not support QUIC or Shadowsocks.

```csharp
var options = new MinecraftClientOptions
{
    Host = "play.example.com",
    Proxy = proxyClient,
};
```

## Discovering servers on a local network

A world open on a local network broadcasts an announcement to 224.0.2.60:4445
every second and a half, in the format `[MOTD]…[/MOTD][AD]…[/AD]`.
[`LanServerDetector`](../08-api-reference/McProtoNet/LanServerDetector.md)
listens on this group and parses the announcements into
[`LanServer`](../08-api-reference/McProtoNet/LanServer.md) - the MOTD and the
address where the server accepts connections.

```csharp
var found = await LanServerDetector.DiscoverAsync(TimeSpan.FromSeconds(3));
foreach (var server in found)
    Console.WriteLine($"{server.Motd} -> {server.EndPoint}");
```

`DiscoverAsync` listens for a set time window and removes duplicate
announcements from the same world by address. `ListenAsync` yields announcements
as they arrive, without deduplication - for a list that must update on the fly.

## What goes to the server right after connecting

The first packet out is the handshaking packet with the protocol version and the
server address and port. The second is the login start request. A code example
is in [First bot](../02-getting-started/02-first-bot.md).

## One connection per client

`MinecraftClient` is single-use. The connected flag is set on the first
successful `ConnectAsync` and is cleared only when the connection attempt fails.
After a disconnect, a second `ConnectAsync` call on the same instance throws
`InvalidOperationException`. Reconnecting means a new client, and with it a new
handler - the old one still remembers the phase where the last session broke
off.

From here the server answers in login: with compression, with encryption, or
with an immediate success. The packet order and the transitions between phases
are in [Phase and direction](../05-packets/01-phases-and-direction.md). Turning
on encryption and compression is in
[Encryption and compression](05-encryption-and-compression.md). The full
example, including the key exchange and the first move into play, is on the
[First bot](../02-getting-started/02-first-bot.md) page.
