# Phase and direction: a packet's address

A packet number means nothing on its own: the same number in different phases
and directions is a different packet. The packet type comes not from the number
alone, but from the triple phase-direction-number, and also depends on the
protocol version: the same triple can point to a different packet in a different
version, or point to nothing at all. `PacketRegistry.TryResolve` takes exactly
this triple plus the version - parsing a number without a phase and a direction
makes no sense.

## Phases and directions

The [`PacketPhase`](../08-api-reference/McProtoNet/Protocol/PacketPhase.md) enum
in `McProtoNet.Protocol` lists the phases, in the order a connection lives
through them:

- `Handshaking` - the first packet of the session, chooses the next phase;
- `Status` - server query (ping, player list in the MOTD);
- `Login` - authentication, encryption, compression;
- `Configuration` - exchange of client data and server registries;
- `Play` - the game.

[`PacketDirection`](../08-api-reference/McProtoNet/Protocol/PacketDirection.md)
sets the direction: `Clientbound` - the packet goes from the server to the
client, `Serverbound` - from the client to the server. The same phase carries
two independent packet sets, one per direction. The order of phases and packets
when joining a server is described in the protocol on the
[Protocol FAQ](https://minecraft.wiki/w/Java_Edition_protocol/FAQ) page.

## Catalog and ordinal

A (phase, direction) pair is a catalog: `PacketRegistry.Catalog(phase, dir)`
returns it in full, as a list of `PacketDescriptor`. Inside a catalog, a packet
carries a dense number, `Ordinal` -
[`PacketIdentity`](../08-api-reference/McProtoNet/Protocol/PacketIdentity.md)
carries it along with the name and the key:

```csharp
public readonly record struct PacketIdentity(
    string Key,
    string Name,
    PacketPhase Phase,
    PacketDirection Direction,
    ushort Ordinal);
```

`Ordinal` is dense only within its own catalog, and it is not the same as the
protocol packet number (the `Id` field on
[`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md)):
the number arrives from the server and can change between versions, while
`Ordinal` stays stable across builds and serves as an index into the parsing
tables.

Getting `Ordinal` from a packet number needs the full triple plus the protocol
version - this is how `PacketRegistry.TryResolve` in `MinimalBot` finds the name
of an unfamiliar packet:

```csharp
var packetName = PacketRegistry.TryResolve(
    raw.Id, pv, Phase, Direction, out var desc)
    ? desc.Identity.Name
    : $"0x{raw.Id:X2}";
```

## Who holds the phase

The library does not infer the phase on its own - only the application code that
talks to the server knows it.
[`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
and
[`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md)
each expose a `Phase` property, and the application sets it in response to
packets that signal a transition. Until `Phase` is set,
[`PacketRegistry`](../08-api-reference/McProtoNet/Protocol/PacketRegistry.md)
keeps looking up the type by the old phase - and so, by the old catalog.

One of the transitions in `MinimalBot` happens after a successful login:

```csharp
protected override async ValueTask OnLoginSuccess(
    LoginCb.LoginSuccessPacket packet)
{
    await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
    Phase = PacketPhase.Configuration;
    ...
}
```

## All transitions

Switching phases is not the library's concern: it carries packets, and the
application code drives the sequence. But knowing the sequence matters, so here
it is in full, for the client.

| From phase | Server signal | Application sends | New phase |
| --- | --- | --- | --- |
| - | - | `SetProtocol` with `NextState = 2` | `Login` |
| - | - | `SetProtocol` with `NextState = 1` | `Status` |
| `Login` | `LoginSuccess` | `LoginAcknowledged` | `Configuration` |
| `Configuration` | `FinishConfiguration` | `FinishConfiguration` | `Play` |
| `Play` | `StartConfiguration` | `ConfigurationAcknowledged` | `Configuration` |

The first transition is set before any phase begins: the `NextState` field in
the handshake tells the server where to go next. The other three are a reply to
a specific packet, after which the application sets `Phase` on the handler.

The last row is not a typo: the server sends the player back from play to
configuration when it changes the resource pack or the registries. A handler
that does not know about this keeps reading packets by the play catalog and
drowns in `Unknown`.

Compression and encryption turn on inside `Login` and do not change the phase -
see
[Encryption and compression](../04-transport/05-encryption-and-compression.md).
For the whole path as code, see
[First bot](../02-getting-started/02-first-bot.md).

## If the phase is not set

If `Phase` is not set in time, incoming packets keep parsing against the old
phase - and the address comes out wrong: what the server sends as a play packet,
`PacketRegistry` looks up among configuration packets. The result is either
`Unknown` (the number is not found in the wrong catalog) or a packet parsed by
the wrong description.

## Next

- [From a raw packet](03-from-raw-packet.md) - how to get a packet's name and
  parsed object from its number
- [Handlers and unknown packets](04-handlers.md) - how to subscribe to a
  specific packet
- [Joining a server](../04-transport/01-joining-a-server.md) - the whole path
  from TCP to the game world
