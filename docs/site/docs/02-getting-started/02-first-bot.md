# First bot

The bot in this chapter connects to a server, reaches the game world, and
stays there: it answers keep-alive, confirms teleports, and prints what
happens to it to the console. It cannot play, but everything needed for play
is already in place.

The server needs `online-mode=false`: the library does not go to Mojang for
a session. The protocol version is an integer: here it is 772, which is
1.21.8. The number for a given version is in the
[Version to protocol](../07-reference/01-version-to-protocol.md).

The full example is in the repository: `examples/MinimalBot`.

## Connection

The client opens a TCP connection, then only moves packets back and forth.

```csharp
const int Pv = 772;

await using var client = new MinecraftClient(new MinecraftClientOptions
{
    Host = "127.0.0.1",
    Port = 25565
});
await client.ConnectAsync();
```

The first two packets go out right away: the handshake with the protocol
number, and the login request. The number 2 at the end of the handshake is
the switch into login.

```csharp
await client.SendAsync(
    new HandshakeSb.SetProtocolPacket(Pv, "127.0.0.1", 25565, 2), Pv);
await client.SendAsync(
    new LoginSb.LoginStartPacket("McProtoBot", V764_Last: new(Guid.NewGuid())),
    Pv);
```

## Handler

A subclass of `ClientboundHandler` parses incoming packets. A method for
each packet is already declared; only the needed ones get overridden.

```csharp
sealed class Bot(MinecraftClient client, int pv) : ClientboundHandler
{
    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket packet)
    {
        client.CompressionThreshold = packet.Threshold;
        return default;
    }

    protected override async ValueTask OnKeepAlive(PlayCb.KeepAlivePacket packet)
    {
        await client.SendAsync(new PlaySb.KeepAlivePacket(packet.KeepAliveId), pv);
    }
}
```

A packet without an overridden method still gets parsed, then quietly
dropped: the server sends a lot that the bot does not need. A packet lands
in `OnUnknown` for a different reason - when its number is not registered
for this version, phase, and direction. That is also a normal state of the
stream, not an error.

## Read loop

Packets arrive as a stream, and the loop that reads them lives in
application code.

```csharp
var bot = new Bot(client, Pv);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

try
{
    await foreach (var packet in client.ReadPacketsAsync(cts.Token))
        await bot.HandleAsync(in packet, Pv);
}
catch (OperationCanceledException)
{
    // stopped on our side, by Ctrl+C
}
catch (EndOfStreamException)
{
    // the server closed the connection
}
```

Without a token, the read waits for the next packet for as long as the
server stays silent. The loop can also be broken from the outside - `Abort`
or `DisposeAsync` from another task - but a token is the most convenient way
to do it.

The end of a session always arrives as an exception. A clean disconnect is
an `EndOfStreamException`. The enumeration never ends quietly.

A packet lives only until the next read: its data is a window into a
buffer, not its own copy. Parse it right away. Do not carry it across an
`await`.

## The bot switches phases

The library itself does not decide when login is over. That happens in
application code, in two places.

```csharp
protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket packet)
{
    await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
    Phase = PacketPhase.Configuration;
}

protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket packet)
{
    await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
    Phase = PacketPhase.Play;
}
```

The server can send the player back to configuration right from the game:
`StartConfiguration` arrives, the bot confirms it, and sets `Phase` back.
For more on the full path, see
[Phase and direction](../05-packets/01-phases-and-direction.md).

## Three replies the bot needs to reach the world

The server waits for a reply to three packets, and staying silent on any of
them ends with the bot stuck in configuration or kicked after spawn.

Right after `LoginAcknowledged`, client settings go out - language, render
distance, visible skin parts:

```csharp
await client.SendAsync(new ConfSb.ClientInformationPacket(
    "en_us", 2, 0, true, 0x7F, 1, false, true,
    V768_Last: new(ParticleStatus.All)), pv);
```

Next the server sends the list of data packs it knows, and waits for the
client to confirm the same list:

```csharp
protected override ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket packet)
    => client.SendAsync(new ConfSb.SelectKnownPacksPacket(packet.Packs), pv);
```

And in play, every teleport - including the first one, at spawn - needs
confirmation by its number, or the server decides the client is frozen:

```csharp
protected override ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket packet)
    => client.SendAsync(new PlaySb.TeleportConfirmPacket(packet.TeleportId), pv);
```

## Encryption

Encryption turns on even on an offline server: an `EncryptionRequestPacket`
arrives, the bot replies with its own key, and turns on the cipher. Since
1.20.5 the server encrypts the stream even without a session check - the
protocol describes this on the
[Encryption](https://minecraft.wiki/w/Java_Edition_protocol/Encryption)
page, in the History section.

```csharp
using var rsa = EncryptionHelpers.DecodeRSAPublicKey(packet.PublicKey)!;
var secret = EncryptionHelpers.GenerateAESPrivateKey();

await client.SendAsync(new LoginSb.EncryptionResponsePacket(
    rsa.Encrypt(secret, false),
    rsa.Encrypt(packet.VerifyToken, false)), pv);

client.EnableEncryption(secret);
```

If the server requires session confirmation with Mojang, the example stops
here.

## Life in play

After spawn, the bot is in the world. From there, it needs three things.

The first is keep-alive. The server sends a number and waits for it back. It
treats a silent client as frozen and closes the connection. The reply
already appeared above, in the Handler section.

The second is chat. A player message arrives as a `PlayerChatPacket`. The
message body sits ready as a string in `PlainMessage`, while the sender name
and formatting sit in separate fields nearby.

```csharp
protected override ValueTask OnPlayerChat(PlayCb.PlayerChatPacket packet)
{
    Console.WriteLine(packet.V770_Last?.PlainMessage);
    return default;
}
```

System strings (join, leave, command replies) arrive as a separate
`SystemChatPacket` in the `OnSystemChat` method, and the text in it sits as
an NBT component. The library returns the component as is. Building a string
out of it is application work.

The bot's own message goes out as a `ChatMessagePacket`. Besides the text,
it carries a timestamp, a salt, and a signature, followed by acknowledgment
of other messages: the offset `Offset`, exactly three bytes of
`Acknowledged`, and a checksum `Checksum`. A server that does not check
signatures accepts zeros.

```csharp
await client.SendAsync(new PlaySb.ChatMessagePacket(
    "hello", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 0, null,
    V770_Last: new(0, new byte[3], 0)), pv);
```

The third is the bot's own position. `PositionPacket` carries it: three
coordinates and movement flags - whether the client is on the ground and
whether it is pressed against a wall. The packet goes out not once but for
as long as the bot moves. Without it, the server keeps the bot where the
last teleport placed it.

```csharp
await client.SendAsync(new PlaySb.PositionPacket(
    x, y, z, V768_Last: new(new MovementFlags(true, false))), pv);
```

## Next

- [The whole bot](03-whole-bot.md)
- [Phase and direction](../05-packets/01-phases-and-direction.md)
- [Packet stream](../04-transport/03-packet-stream.md)
- [From a raw packet](../05-packets/03-from-raw-packet.md)
